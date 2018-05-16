using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using WalletService.Models;

namespace WalletService.Controllers
{
    [RoutePrefix("api/wallet")]
    public class WalletController : ApiController
    {
        private static double money = 100;
        private static double credit = 50;

        private static Wallet Response =>
            new Wallet()
            {
                Money = money.ToString("0.00"),
                Credit = credit.ToString("0.00")
            };

        [Route("reset")]
        [HttpGet]
        public IHttpActionResult Reset()
        {
            money = 100;
            credit = 50;
            return new OkNegotiatedContentResult<Wallet>(Response, this);
        }

        [Route("reset/{money}/{credit}")]
        [HttpGet]
        public IHttpActionResult ResetWithParameters(double money, double credit)
        {
            WalletController.money = money;
            WalletController.credit = credit;
            return new OkNegotiatedContentResult<Wallet>(Response, this);
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return new OkNegotiatedContentResult<Wallet>(Response, this);
        }

        [Route("{money}")]
        [HttpPost]
        public IHttpActionResult Post(double money)
        {
            if (money < 1)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)400,
                    Content = new StringContent("You can't put negative or zero amount of money in wallet.")
                });
            }

            if (WalletController.money + money > 1000)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)422,
                    Content = new StringContent($"Requested amount exceeds max limit, requested amount: ${money:0.00}, current amount = ${WalletController.money:0.00}, max limit: $1000")
                });
            }

            WalletController.money += money;

            return new OkNegotiatedContentResult<Wallet>(Response, this);
        }

        [Route("credit/{credit}")]
        [HttpPost]
        public IHttpActionResult PostCredit(double credit)
        {
            if (credit < 1)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)400,
                    Content = new StringContent("You can't put negative or zero amount of money on credit.")
                });
            }

            if (WalletController.credit + credit > 50)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)422,
                    Content = new StringContent($"Requested amount exceeds limit, requested amount: ${credit:0.00}, current amount of credit: ${WalletController.credit:0.00}, credit limit: $50.")
                });
            }

            WalletController.credit += credit;

            return new OkNegotiatedContentResult<Wallet>(Response, this);
        }

        [HttpDelete]
        [Route("{money}")]
        public IHttpActionResult Delete(double money)
        {
            if (money < 1)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)400,
                    Content = new StringContent("You can't remove negative or zero amount of money from wallet.")
                });
            }

            if (WalletController.money < money)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)405,
                    Content = new StringContent($"Not enough money in wallet, requested amount: ${money:0.00}, current amount: ${WalletController.money:0.00}.")
                });
            }

            WalletController.money -= money;

            return new OkNegotiatedContentResult<Wallet>(Response, this);
        }

        [HttpDelete]
        [Route("credit/{credit}")]
        public IHttpActionResult DeleteCredit(double credit)
        {
            if (credit < 1)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)400,
                    Content = new StringContent("You can't remove negative or zero amount of money from credit.")
                });
            }

            if (WalletController.credit < credit)
            {
                return new ResponseMessageResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode)422,
                    Content = new StringContent($"Not enough credit in wallet, requested amount: ${credit:0.00}, current credit amount: ${WalletController.credit:0.00}.")
                });
            }

            WalletController.credit -= credit;

            return new OkNegotiatedContentResult<Wallet>(Response, this);
        }
    }
}
