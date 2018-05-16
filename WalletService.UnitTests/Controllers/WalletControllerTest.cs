using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using NUnit.Framework;
using WalletService.Models;
using Newtonsoft.Json;
using NUnit.Framework.Internal;

namespace WalletService.UnitTests.Controllers
{
    [TestFixture]
    public class WalletControllerTest : BaseTest
    {
        private static HttpClient HttpClient => new HttpClient()
        {
            BaseAddress = new Uri("http://restservicewallet.azurewebsites.net/api/account/")
        };

        [Test]
        public async Task GetResetShouldReturn200AndDefaultValueWithTwoDigitsPrecision()
        {
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}/reset");
            var response = await HttpClient.GetAsync("reset");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress}/reset status: {response.StatusCode}");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            var content = JsonConvert.DeserializeObject<Wallet>(await response.Content.ReadAsStringAsync());

            Assert.IsTrue(content.Money == "100.00", "Wrong amount of money is returned after reset.");
            Assert.IsTrue(content.Credit == "50.00", "Wrong amount of credit is returned after reset.");
        }


        [Test]
        public async Task GetShouldReturn200()
        {
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}");
            var response = await HttpClient.GetAsync("");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after get account.");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.PostTestCases))]
        public async Task<string> PostShouldAddToAmount(double value)
        {
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}");
            var response = await HttpClient.GetAsync("reset");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            Test.Log(Status.Info, $"Request POST {HttpClient.BaseAddress}");
            response = await HttpClient.PostAsync(value.ToString(), null);
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress} status: {response.StatusCode}");
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress} content: {await response.Content.ReadAsStringAsync()}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after post.");

            var content = JsonConvert.DeserializeObject<Wallet>(await response.Content.ReadAsStringAsync());

            return content.Money;
        }

        [Test]
        public async Task PostOverMaxShouldReturn422()
        {
            const int value = 1000;
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}");
            var response = await HttpClient.GetAsync("reset");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            Test.Log(Status.Info, $"Request POST {HttpClient.BaseAddress}");
            response = await HttpClient.PostAsync(value.ToString(), null);
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress} status: {response.StatusCode}");

            Assert.AreEqual((HttpStatusCode)422, response.StatusCode, "Wrong status code is returned after posting over max amount.");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.PostOrDeleteInvalidAmountTestCases))]
        public async Task PostInvalidAmountShouldReturn400(double value)
        {
            Test.Log(Status.Info, $"Request POST {HttpClient.BaseAddress}/{value}");
            var response = await HttpClient.PostAsync(value.ToString(), null);
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress}/{value} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Wrong status code is returned after posting invalid amount.");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.DeleteTestCases))]
        public async Task<string> DeleteShouldSubtractFrom_Amount(int value)
        {
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}");
            var response = await HttpClient.GetAsync("reset");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            Test.Log(Status.Info, $"Request DELETE {HttpClient.BaseAddress}/{value}");
            response = await HttpClient.DeleteAsync(value.ToString());
            Test.Log(Status.Info, $"Response DELETE {HttpClient.BaseAddress}/{value} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after delete.");

            var content =JsonConvert.DeserializeObject<Wallet>(await response.Content.ReadAsStringAsync());

            return content.Money;
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.PostOrDeleteInvalidAmountTestCases))]
        public async Task DeleteInvalidAmountShouldReturn400(double value)
        {
            var response = await HttpClient.DeleteAsync(value.ToString());

            Test.Log(Status.Info, $"Request DELETE {HttpClient.BaseAddress}/{value}");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Wrong status code is returned after deleting invalid amount.");
            Test.Log(Status.Info, $"Response DELETE {HttpClient.BaseAddress}/{value} status: {response.StatusCode}");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.PostCreditTestCases))]
        public async Task<string> PostCeditShouldAddToCredit(double value)
        {
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}");
            var response = await HttpClient.GetAsync("reset/100/0");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress}reset/100/0 status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            Test.Log(Status.Info, $"Request POST {HttpClient.BaseAddress}/credit/{value}");
            response = await HttpClient.PostAsync($"credit/{value}", null);
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress}/credit/{value} status: {response.StatusCode}");
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress}/credit/{value} content: {await response.Content.ReadAsStringAsync()}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after post.");

            var content = JsonConvert.DeserializeObject<Wallet>(await response.Content.ReadAsStringAsync());

            return content.Credit;
        }

        [Test]
        public async Task PostCreditOverMaxShouldReturn422()
        {
            const int value = 1000;
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}");
            var response = await HttpClient.GetAsync("reset/25/25");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress}reset/25/25 status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            Test.Log(Status.Info, $"Request POST {HttpClient.BaseAddress}/credit/{value}");
            response = await HttpClient.PostAsync($"credit/{value}", null);
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress}/credit/{value} status: {response.StatusCode}");

            Assert.AreEqual((HttpStatusCode)422, response.StatusCode, "Wrong status code is returned after posting over max amount.");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.PostOrDeleteInvalidAmountTestCases))]
        public async Task PostCreditInvalidAmountShouldReturn400(double value)
        {
            Test.Log(Status.Info, $"Request POST {HttpClient.BaseAddress}/credit/{value}");
            var response = await HttpClient.PostAsync($"credit/{value}", null);
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress}/credit/{value} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Wrong status code is returned after posting invalid amount.");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.DeleteCreditTestCases))]
        public async Task<string> DeleteCreditShouldSubtractFromAmount(double value)
        {
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}/reset");
            var response = await HttpClient.GetAsync("reset");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress}/reset status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            Test.Log(Status.Info, $"Request DELETE {HttpClient.BaseAddress}/credit/{value}");
            response = await HttpClient.DeleteAsync($"credit/{value}");
            Test.Log(Status.Info, $"Response DELETE {HttpClient.BaseAddress}/credit/{value} status: {response.StatusCode}");
            Test.Log(Status.Info, $"Response POST {HttpClient.BaseAddress}/credit/{value} content: {await response.Content.ReadAsStringAsync()}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after delete credit.");

            var content = JsonConvert.DeserializeObject<Wallet>(await response.Content.ReadAsStringAsync());

            return content.Credit;
        }

        [Test]
        public async Task DeleteCreditOverMaxAmountShouldReturn422()
        {
            const int value = 1000;
            Test.Log(Status.Info, $"Request GET {HttpClient.BaseAddress}/reset");
            var response = await HttpClient.GetAsync("reset");
            Test.Log(Status.Info, $"Response GET {HttpClient.BaseAddress}/reset status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Wrong status code is returned after reset.");

            Test.Log(Status.Info, $"Request DELETE {HttpClient.BaseAddress}/credit/{value}");
            response = await HttpClient.DeleteAsync($"credit/{value}");
            Test.Log(Status.Info, $"Response DELETE {HttpClient.BaseAddress}/credit/{value} status: {response.StatusCode}");

            Assert.AreEqual((HttpStatusCode)422, response.StatusCode, "Wrong status code is returned after deleting credit over max amount.");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesProvider), nameof(TestCasesProvider.PostOrDeleteInvalidAmountTestCases))]
        public async Task DeleteCreditInvalidAmountShouldReturn400(double value)
        {
            Test.Log(Status.Info, $"Request DELETE {HttpClient.BaseAddress}/credit/{value}");
            var response = await HttpClient.DeleteAsync($"credit/{value}");
            Test.Log(Status.Info, $"Response DELETE {HttpClient.BaseAddress}/credit/{value} status: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Wrong status code is returned after deleting invalid amount of credit.");
        }
    }
}