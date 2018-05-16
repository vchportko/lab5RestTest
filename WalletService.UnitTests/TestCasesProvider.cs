using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WalletService.UnitTests
{
    public static class TestCasesProvider
    {
        public static IEnumerable PostTestCases
        {
            get
            {
                yield return new TestCaseData(10).Returns("110.00");
                yield return new TestCaseData(20).Returns("120.00");
            }
        }

        public static IEnumerable PostCreditTestCases
        {
            get
            {
                yield return new TestCaseData(10).Returns("10.00");
                yield return new TestCaseData(20).Returns("20.00");
            }
        }

        public static IEnumerable PostOrDeleteInvalidAmountTestCases
        {
            get
            {
                yield return new TestCaseData(-1);
                yield return new TestCaseData(0);
            }
        }

        public static IEnumerable DeleteTestCases
        {
            get
            {
                yield return new TestCaseData(10).Returns("90.00");
                yield return new TestCaseData(20).Returns("80.00");
            }
        }

        public static IEnumerable DeleteCreditTestCases
        {
            get
            {
                yield return new TestCaseData(10).Returns("40.00");
                yield return new TestCaseData(20).Returns("30.00");
            }
        }
    }
}
