﻿using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Documents.ViewModels
{
    public class DefaultCompletionDataModel : ICompletionData
    {
        private readonly ICompletionItem item;

        public DefaultCompletionDataModel(ICompletionItem item)
        {
            this.item = item;
        }

        public ImageSource Image => default;

        public string Text => item.Name;

        public object Content => item.Name;

        public object Description => item.Description;

        public double Priority => item.Priority;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
