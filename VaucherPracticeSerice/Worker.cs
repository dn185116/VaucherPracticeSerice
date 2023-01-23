using Microsoft.Extensions.Configuration;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var getTransactions = await _vpsApi.GetTransactions();
                    if (getTransactions != null && getTransactions.Result.Count > 0)
                    {
                        foreach (var transaction in getTransactions.Result)
                        {
                            InDisplayConfirmationTO confirmationTO = new InDisplayConfirmationTO
                            {
                                Request = new DisplayConfirmationRequest
                                {
                                    TransactionId = transaction.TransactionId,
                                    Message = "With this voucher you got discount of 1$",
                                    TimeoutSeconds = 60,
                                }
                            };

                            InPromptCashierTO inPrompt = new InPromptCashierTO
                            {
                                Request = new PromptCashierRequest
                                {
                                    TransactionId = transaction.TransactionId,
                                    Message = "Do you want to use vaucher code?",
                                    ButtonTexts = new string[] { "Yes", "No" },
                                    DefaultResult = "No",
                                    TimeoutSeconds = 60

                                }
                            };

                            InCompleteTransactionTO inComplete = new InCompleteTransactionTO
                            {
                                Request = new CompleteTransactionRequest
                                {
                                    TransactionId = transaction.TransactionId,
                                    Approved = true,
                                    ApprovalCode = "aHello",
                                    ApprovedAmount = transaction.Amount,
                                    ApprovedCashback = transaction.Cashback,
                                    ApprovedTip = transaction.Tip,
                                    ReferenceNumber = transaction.ReferenceNumber,
                                    ErrorMessage = "errorMessage"
                                }
                            };

                            var promptCashier = await _vpsApi.PromptCashier(inPrompt);

                            if (promptCashier.Result.Result == "Yes")
                            {
                                InGetAlphaNumericTO inGetAlpha = new InGetAlphaNumericTO
                                {
                                    Request = new RequestGetAlphaNumeric
                                    {
                                        TransactionId = transaction.TransactionId,
                                        Message = "Enter voucher code:",
                                        MinLength = "2",
                                        MaxLength = "10",
                                        TimeoutSeconds = 60
                                    }
                                };
                                var getetAlphaNumeric = await _vpsApi.GetAlphaNumeric(inGetAlpha);

                                if (getetAlphaNumeric.Result.Result.ToLower() == "c1")
                                {
                                  
                                   var displayMessage =  await _vpsApi.DisplayConfirmation(confirmationTO);

                                    if (displayMessage)
                                    {
                                        await _vpsApi.CompleteTransaction(inComplete);
                                    }
                                    
                                }
                                else
                                {

                                    confirmationTO.Request.Message = "invalid Voucher";
                              
                                    await _vpsApi.DisplayConfirmation(confirmationTO);

                                    inComplete.Request.Approved = false;
                                    inComplete.Request.ErrorMessage = "Invalid Transaction";

                                    await _vpsApi.CompleteTransaction(inComplete);
                                }
                            }
                            else
                            {
                                await _vpsApi.CompleteTransaction(inComplete);
                            }

                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                await Task.Delay(_pollingPeriodMilis, stoppingToken);
            }
        }
    }
}