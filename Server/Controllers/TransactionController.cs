#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server.Controllers
{
    public class TransactionController
    {
        private readonly List<SubmitBasketRequest> _transactions = new List<SubmitBasketRequest>();
        public POSController? PosController { get; set; }

        public void AddSubmitBasketRequestToPool(SubmitBasketRequest request)
        {
            _transactions.Add(request);
        }

        public async Task PayBasket()
        {
            if (_transactions.Count > 0)
            {
                SubmitBasketRequest basketTransaction = _transactions[0];
                // NOT THREAD SAFE
                _transactions.RemoveAt(0);

                Guid requestID = Guid.NewGuid();

                PaymentInfo paymentInformation = new PaymentInfo
                {
                    Amount = 50.00m,
                    AuthorizationCode = "AUTH420",
                    LastFourDigits = "3629"
                };

                BasketPaidRequest payload = new BasketPaidRequest
                {
                    ID = requestID.ToString(),
                    POSTransactionNumber = basketTransaction.POSTransactionNumber,
                    Data = basketTransaction.Data,
                    PaymentInformation = paymentInformation
                };

                if (PosController != null)
                {
                    await PosController.SendTo(payload).ConfigureAwait(false);
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
        }
    }
}
