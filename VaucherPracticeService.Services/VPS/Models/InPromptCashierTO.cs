using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaucherPracticeService.Services.VPS.Models
{
    [ExcludeFromCodeCoverage]
    public class InPromptCashierTO
    {
        public PromptCashierRequest Request { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class PromptCashierRequest
    {
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string[] ButtonTexts { get; set; }
        public string DefaultResult { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
