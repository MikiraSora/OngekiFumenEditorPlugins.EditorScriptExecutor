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
        Task<BuildResult> Build(BuildParam param);
        Task<ExecuteResult> Execute(BuildParam param, FumenVisualEditorViewModel targetEditor);
    }
}
