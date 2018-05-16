using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace WalletService.UnitTests
{
    [SetUpFixture]
    public abstract class BaseTest
    {
        protected ExtentReports Extent { get; set; }
        protected ExtentTest Test { get; set; }

        [OneTimeSetUp]
        protected void Setup()
        {
            var dir = TestContext.CurrentContext.TestDirectory + "\\";
            var fileName = GetType() + ".html";
            var htmlReporter = new ExtentHtmlReporter(dir + fileName)
            {
                AppendExisting = true,
            };

            Extent = new ExtentReports();
            Extent.AttachReporter(htmlReporter);
        }

        [OneTimeTearDown]
        protected void TearDown()
        {
            Extent.Flush();
        }

        [SetUp]
        public void BeforeTest()
        {
            Test = Extent.CreateTest(TestContext.CurrentContext.Test.Name);
            Test.Log(Status.Info, $"Test {TestContext.CurrentContext.Test.FullName} started.");
        }

        [TearDown]
        public void AfterTest()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                ? ""
                : $"{TestContext.CurrentContext.Result.StackTrace}";
            Status logstatus;

            switch (status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    break;
                case TestStatus.Inconclusive:
                    logstatus = Status.Warning;
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    break;
                default:
                    logstatus = Status.Pass;
                    break;
            }

            Test.Log(logstatus,
                $"Test {TestContext.CurrentContext.Test.FullName} ended with " + logstatus + stacktrace);
            Extent.Flush();
        }
    }
}
