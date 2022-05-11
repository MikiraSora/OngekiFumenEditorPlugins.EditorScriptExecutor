using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel
{
    public abstract class EditorScriptBase
    {
        public abstract string ScriptName { get; }

        public abstract void OnExecuteOrEditorRedo(ScriptExecutionContext context);
        public abstract void OnEditorUndo(ScriptExecutionContext context);
    }
}
