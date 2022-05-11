using System;
using System.ComponentModel.Composition;
using System.Text.Json;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using OngekiFumenEditor.Modules.FumenVisualEditor.Kernel;
using OngekiFumenEditor.Utils;
using OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel;

namespace OngekiFumenEditor.Kernel.MiscMenu.Commands
{
    [CommandHandler]
    public class ScriptExecuteCommandHandler : CommandHandlerBase<ScriptExecuteCommandDefinition>
    {
        public override void Update(Command command)
        {
            base.Update(command);
            command.Enabled = IoC.Get<IEditorDocumentManager>().CurrentActivatedEditor is not null;
        }

        public override Task Run(Command command)
        {
            var str = @"
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
        public override string ScriptName => ""ExampleScript"";

        public override void OnEditorUndo(ScriptExecutionContext context)
        {
            context.Editor.Toast.ShowMessage(""call OnEditorUndo()"");
        }

        public override void OnExecuteOrEditorRedo(ScriptExecutionContext context)
        {
            context.Editor.Toast.ShowMessage(""call OnExecuteOrEditorRedo()"");
        }
    }
}
";

            IoC.Get<IEditorScriptExecutor>().Execute(str, IoC.Get<IEditorDocumentManager>().CurrentActivatedEditor);
            return TaskUtility.Completed;
        }
    }
}