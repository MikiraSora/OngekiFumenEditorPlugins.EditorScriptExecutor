﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel
{
    public struct ExecuteResult
    {
        public ExecuteResult(bool isSuccess, string errorMessage = default, object result = default)
        {
            this.Success = isSuccess;
            this.ErrorMessage = errorMessage;
            Result = result;
        }

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public object Result { get; }
    }
}
