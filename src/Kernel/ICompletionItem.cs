using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel
{
    public interface ICompletionItem
    {
        string Name { get; }
        string Description { get; }
        int Priority { get; }
    }
}
