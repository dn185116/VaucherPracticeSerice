using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaucherPracticeService.Services.VPS.Models
{
    public class InGetAlphaNumericTO
    {
        public RequestGetAlphaNumeric Request { get; set; }
    }
    public class RequestGetAlphaNumeric
    {
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string MinLength { get; set; }
        public string MaxLength { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
