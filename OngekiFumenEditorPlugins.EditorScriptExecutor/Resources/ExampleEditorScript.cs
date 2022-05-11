using OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Resources
{
    public class ExampleEditorScript : EditorScriptBase
    {
        public override string ScriptName => "ExampleScript";

        public override void OnEditorUndo(ScriptExecutionContext context)
        {
            context.Editor.Toast.ShowMessage("call OnEditorUndo()");
        }

        public override void OnExecuteOrEditorRedo(ScriptExecutionContext context)
        {
            context.Editor.Toast.ShowMessage("call OnExecuteOrEditorRedo()");
        }
    }
}
