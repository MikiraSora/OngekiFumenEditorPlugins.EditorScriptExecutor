using Caliburn.Micro;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp;
using OngekiFumenEditor;
using OngekiFumenEditor.Modules.FumenVisualEditor.Base;
using OngekiFumenEditor.Modules.FumenVisualEditor.ViewModels;
using OngekiFumenEditor.Utils;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel.DefaultImpl
{
    [Export(typeof(IEditorScriptExecutor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DefaultEditorScriptExecutor : IEditorScriptExecutor
    {
        private readonly List<MetadataReference> assemblyReferenceList;
        private readonly CSharpCompilationOptions compilationOptions;
        private readonly CSharpParseOptions parserOption;

        public DefaultEditorScriptExecutor()
        {
            assemblyReferenceList = new List<MetadataReference>() {
                MetadataReference.CreateFromFile(typeof(CodeCompiler).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(App).Assembly.Location),
            };

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            assemblyReferenceList.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")));
            assemblyReferenceList.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")));
            assemblyReferenceList.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")));
            assemblyReferenceList.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")));

            compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: new[]{
                "System",
                "System.IO",
            })
                .WithOptimizationLevel(OptimizationLevel.Debug)
                .WithPlatform(Platform.AnyCpu);

            parserOption = CSharpParseOptions.Default.WithKind(SourceCodeKind.Script).WithLanguageVersion(LanguageVersion.Preview);
        }

        public Task<BuildResult> Build(BuildParam param)
        {
            Log.LogDebug($"-------BEGIN SCRIPT BUILD--------");
            var overrideAssemblyLocations = assemblyReferenceList.ToList();
            foreach (var newLoc in param.AssemblyLocations)
                overrideAssemblyLocations.Add(MetadataReference.CreateFromFile(newLoc));
            overrideAssemblyLocations.DistinctSelf();

            overrideAssemblyLocations.ForEach(x => Log.LogDebug($"asmLoc: {x.Display}"));

            var encoding = Encoding.UTF8;

            var assemblyName = Path.GetRandomFileName();
            var sourceCodePath = Path.ChangeExtension(assemblyName, "cs");

            var buffer = encoding.GetBytes(param.Script);
            var sourceText = SourceText.From(buffer, buffer.Length, encoding, canBeEmbedded: true);

            return Task.Run(() =>
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, parserOption, sourceCodePath);

                var comp = CSharpCompilation.CreateScriptCompilation(
                    assemblyName,
                    syntaxTree,
                    overrideAssemblyLocations,
                    compilationOptions
                );

                var symbolsName = Path.ChangeExtension(assemblyName, "pdb");

                var emitOptions = new EmitOptions(
                            debugInformationFormat: DebugInformationFormat.PortablePdb,
                            pdbFilePath: symbolsName);

                var embeddedTexts = new List<EmbeddedText>
                {
                    EmbeddedText.FromSource(sourceCodePath, sourceText),
                };

                using var peStream = new MemoryStream();
                using var pdbStream = new MemoryStream();

                var emitResult = comp.Emit(peStream, pdbStream, embeddedTexts: embeddedTexts, options: emitOptions);
                var diagnostics = emitResult.Diagnostics.ToArray();

                if (!emitResult.Success)
                    return new BuildResult(emitResult);

                var assembly = Assembly.Load(peStream.ToArray(), pdbStream.ToArray());

                Log.LogDebug($"-------END SCRIPT BUILD--------");
                return new BuildResult(comp)
                {
                    Assembly = assembly,
                    EntryPoint = comp.GetEntryPoint(default),
                };
            });
        }

        public async Task<ExecuteResult> Execute(BuildParam param, FumenVisualEditorViewModel targetEditor)
        {
            var buildResult = await Build(param);

            if (!buildResult.IsSuccess)
                return new(false, "无法编译脚本:" + Environment.NewLine + string.Join(Environment.NewLine, buildResult.Errors));

            var assembly = buildResult.Assembly;
            if (assembly is null)
                return new(false, "failed to generate assembly : " + Environment.NewLine + string.Join(Environment.NewLine, buildResult.Errors));

            var result = await Execute(assembly, buildResult.EntryPoint);
            return result;
        }

        private async Task<ExecuteResult> Execute(Assembly assembly, IMethodSymbol ep)
        {
            var epType = assembly.GetType($"{ep.ContainingNamespace.MetadataName}.{ep.ContainingType.MetadataName}");
            var epMethod = epType.GetMethod(ep.MetadataName);
            var name = $"{epType.GetTypeName()}.{epMethod.Name}()";

            Log.LogDebug($"Script endpoint : {name}");

            try
            {
                var func = epMethod.CreateDelegate(typeof(Func<object[], Task<object>>)) as Func<object[], Task<object>>;
                Log.LogDebug($"Script begin call : {name}");
                var obj = await func(new object[2]);
                Log.LogDebug($"Script end call : {name}");
                return new(true, null, obj);
            }
            catch (Exception e)
            {
                return new(false, e.Message);
            }
        }
    }
}
