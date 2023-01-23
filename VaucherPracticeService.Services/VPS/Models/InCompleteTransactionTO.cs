using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaucherPracticeService.Services.VPS.Models
{
    public class InCompleteTransactionTO
    {
        public CompleteTransactionRequest Request { get; set; }
    }

    public class CompleteTransactionRequest
    {
        public string TransactionId { get; set; }
        public bool Approved { get; set; }
        public string ApprovalCode { get; set; }
        public float ApprovedAmount { get; set; }
        public float ApprovedCashback { get; set; }
        public float ApprovedTip { get; set; }
        public string ReferenceNumber { get; set; }
        public string ErrorMessage { get; set; }
        public string MaskedCardNumber { get; set; }
        public string CardType { get; set; }
    }
}
