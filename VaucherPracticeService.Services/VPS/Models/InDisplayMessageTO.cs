using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaucherPracticeService.Services.VPS.Models
{
    public class InDisplayMessageTO
    {
        public DisplayMessageRequest Request { get; set; }
    }

    public class DisplayMessageRequest
    {
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
