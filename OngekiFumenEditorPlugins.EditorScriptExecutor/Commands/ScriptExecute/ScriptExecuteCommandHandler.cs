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

        public override async Task Run(Command command)
        {

        }
    }
}