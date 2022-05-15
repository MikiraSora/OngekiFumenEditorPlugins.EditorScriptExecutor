using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Operations;
using OngekiFumenEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel
{
    public struct BuildResult
    {
        public BuildResult()
        {
            EntryPoint = default;
            Assembly = default;
            Errors = Enumerable.Empty<Diagnostic>();
            Warnings = Enumerable.Empty<Diagnostic>();
        }

        public BuildResult(EmitResult emitResult) : this()
        {
            Errors = emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Warnings = emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning).ToList();
        }

        public BuildResult(CSharpCompilation comp) : this()
        {
            var digs = comp.GetDiagnostics();
            Errors = digs.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Warnings = digs.Where(d => d.Severity == DiagnosticSeverity.Warning).ToList();
        }

        public IEnumerable<Diagnostic> Errors { get; set; }
        public IEnumerable<Diagnostic> Warnings { get; set; }

        public IMethodSymbol EntryPoint { get; set; }

        public bool IsSuccess => Errors.None();

        public Assembly Assembly { get; set; }
    }
}
