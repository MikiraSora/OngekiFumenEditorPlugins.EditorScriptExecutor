using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
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

        public void Dispose()
        {
            WorkSpace?.Dispose();
            WorkSpace = default;
            Document = default;
            CompletionService = default;
        }
    }
}
