﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel.DefaultImpl
{
    public class DefaultCompletionItem : ICompletionItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }
    }
}
