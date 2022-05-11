using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using OngekiFumenEditor.Kernel.MiscMenu.Commands;
using OngekiFumenEditor.Modules.AudioPlayerToolViewer.Commands;
using OngekiFumenEditor.Modules.FumenBulletPalleteListViewer.Commands;
using OngekiFumenEditor.Modules.FumenMetaInfoBrowser.Commands;

namespace OngekiFumenEditor.Kernel.MiscMenu
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuDefinition ScriptMenu = new MenuDefinition(Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar, 7, "脚本 (_S)");

        [Export]
        public static MenuItemGroupDefinition ScriptMenuMenuGroup = new MenuItemGroupDefinition(ScriptMenu, 0);

        [Export]
        public static MenuItemDefinition ScriptExecuteMenuItem = new CommandMenuItemDefinition<ScriptExecuteCommandDefinition>(ScriptMenuMenuGroup, 0);
    }
}