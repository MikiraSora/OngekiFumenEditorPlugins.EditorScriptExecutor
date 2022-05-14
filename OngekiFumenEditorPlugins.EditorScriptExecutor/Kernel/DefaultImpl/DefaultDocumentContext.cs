using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel.DefaultImpl
{
    public class DefaultDocumentContext : IDocumentContext
    {
        public AdhocWorkspace WorkSpace { get; set; }
        public Document Document { get; set; }
        public CompletionService CompletionService { get; set; }

        public async IAsyncEnumerable<ICompletionItem> CompleteCode(string str, int cursorPos, bool insertOrDelete)
        {
            var document = Document.WithText(SourceText.From(str));
            var result = await CompletionService.GetCompletionsAsync(document, cursorPos);
            if (result is null)
                yield break;
            foreach (var item in result.Items)
            {
                var desc = await CompletionService.GetDescriptionAsync(document, item);
                yield return new DefaultCompletionItem()
                {
                    Description = desc.Text,
                    Name = item.FilterText,
                    Priority = 0
                };
            }
        }

        public BuildParam CreateBuildParam()
        {
            return new BuildParam();
        }

        public bool GenerateProjectFile(string genProjOutputDirPath, string scriptFilePath, out string projFilePath)
        {
            void AddItems(ProjectRootElement elem, string groupName, params string[] items)
            {
                var group = elem.AddItemGroup();
                foreach (var item in items)
                {
                    group.AddItem(groupName, item);
                }
            }

            var root = ProjectRootElement.Create();
            root.Sdk = "Microsoft.NET.Sdk";

            var projCommonGroup = root.AddPropertyGroup();
            projCommonGroup.AddProperty("TargetFramework", "net5.0-windows");
            projCommonGroup.AddProperty("OutputType", "Exe");


            var refGroup = root.AddItemGroup();
            //add references
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.GetName().Name;

                //filter System.*
                if (name.StartsWith("System."))
                    continue;

                try
                {
                    if (!File.Exists(assembly.Location))
                        continue;
                }
                catch
                {
                    continue;
                }



                var refElement = refGroup.AddItem("Reference", name);
                var hintPathElement = root.CreateMetadataElement("HintPath", assembly.Location);
                refElement.AppendChild(hintPathElement);
            }

            projFilePath = Path.Combine(genProjOutputDirPath, "Script.csproj");
            root.Save(projFilePath);

            return true;
        }

        public void Dispose()
        {
            WorkSpace?.Dispose();
            WorkSpace = default;
            Document = default;
            CompletionService = default;
        }
    }
}
