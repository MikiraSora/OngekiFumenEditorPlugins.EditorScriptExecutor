﻿using Microsoft.CodeAnalysis;
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
        public BuildResult(IEnumerable<Diagnostic> errors, IEnumerable<Diagnostic> warnings)
        {
            EntryPoint = default;
            Assembly = default;
            Errors = errors;
            Warnings = warnings;
        }

        public BuildResult(EmitResult emitResult)
            : this(emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList(),
                  emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning).ToList())
        {

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

        public bool IsSuccess => Errors.IsEmpty();

        public Assembly Assembly { get; set; }
    }
}
