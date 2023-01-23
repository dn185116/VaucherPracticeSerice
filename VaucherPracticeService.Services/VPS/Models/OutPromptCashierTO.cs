using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaucherPracticeService.Services.VPS.Models
{
    [ExcludeFromCodeCoverage]
    public class OutPromptCashierTO
    {
        public OutCashierResult Result { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class OutCashierResult
    {
        public string Result { get; set; }
    }
}
