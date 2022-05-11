using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngekiFumenEditorPlugins.EditorScriptExecutor.Kernel
{
    public struct ExecuteResult
    {
        public ExecuteResult(bool isSuccess,string errorMessage = default)
        {
            this.Success = isSuccess;
            this.ErrorMessage = errorMessage;
        }

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
