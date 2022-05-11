using Caliburn.Micro;
using Microsoft.CodeAnalysis.CSharp;
using OngekiFumenEditor.Modules.FumenVisualEditor.Base;
using OngekiFumenEditor.Modules.FumenVisualEditor.Kernel;
using OngekiFumenEditor.Modules.FumenVisualEditor.ViewModels;
using OngekiFumenEditor.Utils;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel.DefaultImpl
{
    [Export(typeof(IEditorScriptExecutor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DefaultEditorScriptExecutor : IEditorScriptExecutor
    {
        private CompilerParameters compilerParameters;

        public DefaultEditorScriptExecutor()
        {
            compilerParameters = new CompilerParameters()
            {
                IncludeDebugInformation = true,
                GenerateInMemory = true
            };

            var refAssemblies = new List<string> { "System.dll", "System.Core.dll", "Microsoft.CSharp.dll" };

            foreach (var asm in AssemblySource.Instance)
                refAssemblies.Add(asm.GetAssemblyName());

            refAssemblies = refAssemblies.Distinct().ToList();

            compilerParameters.ReferencedAssemblies.AddRange(refAssemblies.ToArray());
        }

        public ExecuteResult Execute(string script, FumenVisualEditorViewModel targetEditor)
        {
            var compiler = IoC.Get<ICodeCompiler>();
            var result = compiler.CompileAssemblyFromSource(compilerParameters, script);

            if (result.Errors.HasErrors)
                return new(false, "无法编译脚本:" + Environment.NewLine + string.Join(Environment.NewLine, result.Errors));

            var assembly = result.CompiledAssembly;

            var scriptClass = assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(EditorScriptBase)));
            if (scriptClass is null)
                return new(false, "无法找到EditorScriptBase类型");

            var scriptObj = LambdaActivator.CreateInstance(scriptClass) as EditorScriptBase;
            if (scriptObj is null)
                return new(false, $"无法构造{scriptClass.Name}类型的对象");

            return Execute(scriptObj, targetEditor);
        }

        public ExecuteResult Execute(EditorScriptBase script, FumenVisualEditorViewModel targetEditor)
        {
            if (targetEditor is null)
                return new(false, "需要指定编辑器");

            var ctx = new ScriptExecutionContext();
            ctx.ScriptExecutor = this;
            ctx.Editor = targetEditor;

            var redo = new System.Action(() =>
            {
                script.OnExecuteOrEditorRedo(ctx);
            });

            var undo = new System.Action(() => script.OnEditorUndo(ctx));

            targetEditor.UndoRedoManager.ExecuteAction(LambdaUndoAction.Create($"执行脚本:{script.ScriptName}", redo, undo));

            return new(true);
        }
    }
}
