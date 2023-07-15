﻿using Gemini.Framework;
using Gemini.Framework.Services;
using OngekiFumenEditorPlugins.EditorScriptExecutor.Documents.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Documents
{
    [Export(typeof(IEditorProvider))]
    [Export(typeof(IEditorScriptDocumentProvider))]
    public class EditorScriptDocumentProvider : IEditorScriptDocumentProvider
    {
        public const string FILE_EXTENSION_NAME = ".nyagekiScript";

        public IEnumerable<EditorFileType> FileTypes
        {
            get
            {
                yield return new EditorFileType("Editor Script File", FILE_EXTENSION_NAME);
            }
        }


        public bool CanCreateNew => true;

        public IDocument Create()
        {
            return new EditorScriptDocumentViewModel();
        }

        public bool Handles(string path) => Path.GetExtension(path).Equals(FILE_EXTENSION_NAME, StringComparison.OrdinalIgnoreCase);

        public async Task New(IDocument document, string name) => await (document as EditorScriptDocumentViewModel)?.New(name);

        public async Task Open(IDocument document, string path) => await (document as EditorScriptDocumentViewModel)?.Load(path);
    }
}
