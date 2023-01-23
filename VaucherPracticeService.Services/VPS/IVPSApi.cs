using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaucherPracticeService.Services.VPS.Models;

namespace VaucherPracticeService.Services.VPS
{
    public interface IVPSApi
    {
        Task<OutGetTransactionsTO> GetTransactions();
        Task<bool> CompleteTransaction(InCompleteTransactionTO model);
        Task<bool> DisplayMessage(InDisplayMessageTO model);

        Task<bool> DisplayConfirmation(InDisplayConfirmationTO model);

        Task<OutPromptCashierTO> PromptCashier(InPromptCashierTO model);
        Task<OutGetAlphaNumeric> GetAlphaNumeric(InGetAlphaNumericTO model);
    }
}
