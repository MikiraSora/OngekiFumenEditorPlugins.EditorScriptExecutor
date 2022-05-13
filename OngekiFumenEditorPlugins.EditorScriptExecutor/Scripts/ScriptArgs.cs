using Caliburn.Micro;
using OngekiFumenEditor.Modules.FumenVisualEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Scripts
{
    public static class ScriptArgs
    {
        public static FumenVisualEditorViewModel ViewModel => ScriptArgsGlobalStore.GetCurrentEditor(Assembly.GetCallingAssembly());
    }
}
