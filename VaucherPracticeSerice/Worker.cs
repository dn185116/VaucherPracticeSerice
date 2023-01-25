using Serilog;
using VaucherPracticeService.Services.VPS;
using VaucherPracticeService.Services.VPS.Models;

namespace VaucherPracticeSerice
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IVPSApi _vpsApi;

        private readonly int _pollingPeriodMilis;
        public Worker(ILogger<Worker> logger, IVPSApi vpsApi, IConfiguration configuration)
        {
            _logger = logger;
            _vpsApi = vpsApi;

            _pollingPeriodMilis = int.Parse(configuration["AppSettings:PollingPeriodMilis"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ProcessTransactions(stoppingToken);
        }

        private async Task ProcessTransactions(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var getTransactions = await _vpsApi.GetTransactions();
                    if (getTransactions != null && getTransactions.Result.Count > 0)
                    {
                        foreach (var transaction in getTransactions.Result)
                        {
                            try
                            {
                                var useVoucher = await PromptCashier("Do you want to use vaucher code?", transaction.TransactionId);
                                if (useVoucher)
                                {
                                    var voucherCode = await GetAlphaNumeric("Enter voucher code:", transaction.TransactionId, 2, 10);
                                    if (voucherCode.ToLower() == "c1")
                                    {
                                        await DisplayConfirmation("With this voucher you got discount of 1$", transaction.TransactionId);
                                        await CompleteTransaction(transaction.TransactionId, true, "aHello", transaction.Amount, transaction.Cashback, transaction.Tip, transaction.ReferenceNumber, "errorMessage");
                                    }
                                    else
                                    {
                                        await DisplayConfirmation("Invalid Voucher", transaction.TransactionId);
                                    }
                                }
                            }
                            catch (VpsApiException e)
                            {
                                Log.Error(e, "VPS API call failed for transaction {transactionId}", transaction.TransactionId);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Error processing transaction {transactionId}", transaction.TransactionId);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error getting transactions");
                }
            }
        }

        private async Task<bool> PromptCashier(string message, string transactionId)
        {
            try
            {
                var promptCashier = await _vpsApi.PromptCashier(new InPromptCashierTO
                {
                    Request = new PromptCashierRequest
                    {
                        TransactionId = transactionId,
                        Message = message,
                        ButtonTexts = new string[] { "Yes", "No" },
                        DefaultResult = "No",
                        TimeoutSeconds = 60
                    }
                });
                return promptCashier.Result.Result == "Yes";
            }
            catch (VpsApiException e)
            {
                Log.Error(e, "VPS API call failed for transaction {transactionId}", transactionId);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error prompted cashier for transaction {transactionId}", transactionId);
                throw;
            }
        }
        private async Task<string> GetAlphaNumeric(string message, string transactionId, int minLength, int maxLength)
        {
            try
            {
                var alphaNumeric = await _vpsApi.GetAlphaNumeric(new InGetAlphaNumericTO
                {
                    Request = new RequestGetAlphaNumeric
                    {
                        TransactionId = transactionId,
                        Message = message,
                        MinLength = minLength.ToString(),
                        MaxLength = maxLength.ToString(),
                        TimeoutSeconds = 60
                    }
                });
                return alphaNumeric.Result.Result;
            }
            catch (VpsApiException e)
            {
                Log.Error(e, "VPS API call failed for transaction {transactionId}", transactionId);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error getting alpha-numeric input for transaction {transactionId}", transactionId);
                throw;
            }
        }

        private async Task DisplayConfirmation(string message, string transactionId)
        {
            try
            {
                await _vpsApi.DisplayConfirmation(new InDisplayConfirmationTO
                {
                    Request = new DisplayConfirmationRequest
                    {
                        TransactionId = transactionId,
                        Message = message,
                        TimeoutSeconds = 60
                    }
                });
            }
            catch (VpsApiException e)
            {
                Log.Error(e, "VPS API call failed for transaction {transactionId}", transactionId);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error displaying confirmation message for transaction {transactionId}", transactionId);
                throw;
            }
        }

        private async Task CompleteTransaction(string transactionId, bool approved, string approvalCode, float approvedAmount, float approvedCashback, float approvedTip, string referenceNumber, string errorMessage)
        {
            try
            {
                await _vpsApi.CompleteTransaction(new InCompleteTransactionTO
                {
                    Request = new CompleteTransactionRequest
                    {
                        TransactionId = transactionId,
                        Approved = approved,
                        ApprovalCode = approvalCode,
                        ApprovedAmount = approvedAmount,
                        ApprovedCashback = approvedCashback,
                        ApprovedTip = approvedTip,
                        ReferenceNumber = referenceNumber,
                        ErrorMessage = errorMessage
                    }
                });
            }
            catch (VpsApiException e)
            {
                Log.Error(e, "VPS API call failed for transaction {transactionId}", transactionId);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error completing transaction {transactionId}", transactionId);
                throw;
            }
        }

    }
}
   