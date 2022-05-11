using OngekiFumenEditor.Modules.FumenVisualEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel
{
    public interface IEditorScriptExecutor
    {
        ExecuteResult Execute(string script, FumenVisualEditorViewModel targetEditor);
        ExecuteResult Execute(EditorScriptBase script, FumenVisualEditorViewModel targetEditor);
    }
}
