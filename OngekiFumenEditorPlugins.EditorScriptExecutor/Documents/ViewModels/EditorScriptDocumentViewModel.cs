using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using ICSharpCode.AvalonEdit.Document;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Documents.ViewModels
{
    [Export(typeof(EditorScriptDocumentViewModel))]
    [MapToView(ViewType = typeof(EditorScriptDocumentView))]
    public class EditorScriptDocumentViewModel : PersistedDocument
    {
        public TextDocument ScriptDocument { get; set; } = new TextDocument();
        public ObservableCollection<FumenVisualEditorViewModel> CurrentEditors { get; } = new ObservableCollection<FumenVisualEditorViewModel>();

        private FumenVisualEditorViewModel currentSelectedEditor = default;
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

        public void Init()
        {
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

        private BuildParam GetBuildParam() => new BuildParam
        {
            Script = ScriptDocument.Text,
            DisplayFileName = FileName
        };

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
    }
}
