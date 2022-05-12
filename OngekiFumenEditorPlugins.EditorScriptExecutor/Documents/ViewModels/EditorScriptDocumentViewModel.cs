using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using ICSharpCode.AvalonEdit.Document;
using OngekiFumenEditor.Modules.FumenVisualEditor.Kernel;
using OngekiFumenEditor.Modules.FumenVisualEditor.ViewModels;
using OngekiFumenEditor.Utils.Attributes;
using OngekiFumenEditorPlugins.EditorScriptExecutor.Documents.Views;
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
            set => Set(ref currentSelectedEditor, value);
        }

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

        public void OnCheckButtonClicked()
        {

        }

        public void OnRunButtonClicked()
        {

        }

        public void OnManageDLLsButtonClicked()
        {

        }
    }
}
