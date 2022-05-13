using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;
using OngekiFumenEditor.Modules.FumenVisualEditor.Kernel;
using OngekiFumenEditor.Modules.FumenVisualEditor.ViewModels;
using OngekiFumenEditor.Utils;
using OngekiFumenEditor.Utils.Attributes;
using OngekiFumenEditorPlugins.EditorScriptExecutor.Documents.Views;
using OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Documents.ViewModels
{
    [Export(typeof(EditorScriptDocumentViewModel))]
    [MapToView(ViewType = typeof(EditorScriptDocumentView))]
    public class EditorScriptDocumentViewModel : PersistedDocument
    {
        public ICSharpCode.AvalonEdit.Document.TextDocument ScriptDocument { get; set; } = new();
        public ObservableCollection<FumenVisualEditorViewModel> CurrentEditors { get; } = new ObservableCollection<FumenVisualEditorViewModel>();

        private FumenVisualEditorViewModel currentSelectedEditor = default;

        CompletionWindow completionWindow = default;
        private IDocumentContext documentContext;

        public FumenVisualEditorViewModel CurrentSelectedEditor
        {
            get => currentSelectedEditor;
            set
            {
                Set(ref currentSelectedEditor, value);
                NotifyOfPropertyChange(() => IsEnableRun);
            }
        }

        public bool IsEnableRun => CurrentSelectedEditor is not null;

        public async void Init()
        {
            documentContext?.Dispose();
            documentContext = await IoC.Get<IEditorScriptExecutor>().InitDocumentContext();

            var editorManager = IoC.Get<IEditorDocumentManager>();
            editorManager.OnNotifyCreated += UpdateCurrentEditorList;
            editorManager.OnNotifyDestoryed += UpdateCurrentEditorList;
            UpdateCurrentEditorList();
        }

        private void UpdateCurrentEditorList(FumenVisualEditorViewModel _ = default)
        {
            CurrentEditors.Clear();
            foreach (var editor in IoC.Get<IEditorDocumentManager>().GetCurrentEditors())
            {
                CurrentEditors.Add(editor);
            }
            if (!CurrentEditors.Contains(CurrentSelectedEditor))
                CurrentSelectedEditor = default;
        }

        protected override async Task DoLoad(string filePath)
        {
            ScriptDocument.Text = await File.ReadAllTextAsync(filePath);
            Init();
        }

        protected override Task DoNew()
        {
            Init();
            return TaskUtility.Completed;
        }

        protected override async Task DoSave(string filePath)
        {
            try
            {
                using var _ = StatusBarHelper.BeginStatus("Fumen saving : " + filePath);
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    await DoSaveAs(this);
                    return;
                }
                await File.WriteAllTextAsync(filePath, ScriptDocument.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show($"无法保存脚本 , {e.Message}");
            }
        }

        public void OnTextChanged()
        {
            IsDirty = true;
        }

        private BuildParam GetBuildParam()
        {
            var buildParam = documentContext.CreateBuildParam();

            buildParam.Script = ScriptDocument.Text;
            buildParam.DisplayFileName = FileName;

            return buildParam;
        }

        public async void OnCheckButtonClicked()
        {
            using var _ = StatusBarHelper.BeginStatus("Script is building ...");
            var buildResult = await IoC.Get<IEditorScriptExecutor>().Build(GetBuildParam());

            if (buildResult.IsSuccess)
            {
                MessageBox.Show("编译成功");
                return;
            }

            var errorMsg = string.Join(Environment.NewLine, buildResult.Errors);
            MessageBox.Show($"编译失败:\n{errorMsg}");
        }

        public async void OnRunButtonClicked()
        {
            using var _ = StatusBarHelper.BeginStatus("Script is building ...");
            var buildResult = await IoC.Get<IEditorScriptExecutor>().Build(GetBuildParam());

            if (!buildResult.IsSuccess)
            {
                var errorMsg = string.Join(Environment.NewLine, buildResult.Errors);
                MessageBox.Show($"编译失败:\n{errorMsg}");
                return;
            }

            if (MessageBox.Show("编译成功，是否执行?", default, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using var _2 = StatusBarHelper.BeginStatus("Script is executing ...");
            var executeResult = await IoC.Get<IEditorScriptExecutor>().Execute(buildResult, CurrentSelectedEditor);

            MessageBox.Show($"执行{(executeResult.Success ? "成功" : $"失败,原因:{executeResult.ErrorMessage}")}");
        }

        public async void OnReloadFileButtonClicked()
        {
            if (File.Exists(FilePath))
                await DoLoad(FilePath);
        }

        internal async void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "." || e.Text == " ")
            {
                var textArea = sender as TextArea;

                completionWindow = new CompletionWindow(textArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                await foreach (var comp in documentContext.CompleteCode(ScriptDocument.Text, textArea.Caret.Offset, true))
                {
                    data.Add(new DefaultCompletionDataModel(comp));
                }

                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };

                completionWindow.Show();
            }
        }

        internal void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }
    }
}
