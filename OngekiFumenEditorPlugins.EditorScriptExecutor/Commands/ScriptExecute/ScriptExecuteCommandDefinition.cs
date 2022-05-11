using Gemini.Framework.Commands;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace OngekiFumenEditor.Kernel.MiscMenu.Commands
{
    [CommandDefinition]
    public class ScriptExecuteCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Script.Execute";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "执行代码"; }
        }

        public override string ToolTip
        {
            get { return Text; }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<ScriptExecuteCommandDefinition>(new(Key.F5));
    }
}