using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaucherPracticeService.Services.VPS.Models
{
    public class OutGetTransactionsTO
    {
        public List<GetTransactionsResult> Result { get; set; }
    }

    public class GetTransactionsResult
    {
        public string TransactionId { get; set; }
        public float Amount { get; set; }
        public float Cashback { get; set; }
        public int CheckNumber { get; set; }
        public List<object> CustomAttributes { get; set; }
        public string CustomCommand { get; set; }
        public string DayOfBusiness { get; set; }
        public int EmployeeId { get; set; }
        public string Operation { get; set; }
        public string ReferenceNumber { get; set; }
        public int TenderId { get; set; }
        public int TerminalId { get; set; }
        public string Time { get; set; }
        public float Tip { get; set; }
    }
}
