using ATM;
using NUnit.Framework;
using System.Collections;

namespace ATMTests
{
    public class ATMTests
    {
        private static IEnumerable ValidWithdrawals
        {
            get
            {
                // input balance, overdraft, withdrawal amount, return remaining balance.
                yield return new TestCaseData(2, 0, 1).Returns("1");
                yield return new TestCaseData(1, 0, 1).Returns("0");
                yield return new TestCaseData(1, 1, 1).Returns("0");
                yield return new TestCaseData(1, 1, 2).Returns("-1");
                yield return new TestCaseData(0, 1, 1).Returns("-1");
            }
        }

        [TestCaseSource("ValidWithdrawals")]
        public string Withdraw_HasSufficientBalance_returnRemainingBalance(int balance, int overdraft, int withdrawalAmount)
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(0, 0, balance, overdraft);
            var result = atm.WithdrawCash(withdrawalAmount);
            atm.EndSession();
            return result;
        }

        private static IEnumerable InvalidWithdrawals
        {
            get
            {
                // input balance, overdraft, withdrawal amount, return Error.
                yield return new TestCaseData(0, 0, 1);
                yield return new TestCaseData(1, 0, 2);
                yield return new TestCaseData(0, 1, 2);
            }
        }

        [TestCaseSource("InvalidWithdrawals")]
        public void Withdraw_InsufficientBalance_ReturnFundsError(int balance, int overdraft, int withdrawalAmount)
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(0, 0, balance, overdraft);
            var result = atm.WithdrawCash(withdrawalAmount);
            Assert.AreEqual(result, "FUNDS_ERR");
        }

        [Test]
        public void Withdraw_InsufficientCash_ReturnAtmError()
        {
            var atm = new Atm(0);
            atm.BeginSession(0, 0, 100, 100);
            var result = atm.WithdrawCash(100);
            Assert.AreEqual(result, "ATM_ERR");
        }

        [Test]
        public void Withdraw_IncorrectPin_ReturnAccountError()
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(1111, 0, 1, 1);

            var result = atm.WithdrawCash(1);

            Assert.AreEqual(result, "ACCOUNT_ERR");
        }

        [Test]
        public void Withdraw_SufficientBalance_BalanceUpdated()
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(0, 0, 1, 0);
            atm.WithdrawCash(1);

            var result = atm.GetBalance();

            Assert.AreEqual(result, "0");
        }

        [Test]
        public void Withdraw_MultipleWithdrawals_ReturnRemainingBalance()
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(0, 0, 1000, 0);
            atm.WithdrawCash(10);

            var result = atm.WithdrawCash(10);

            Assert.AreEqual(result, "980");
        }

        [Test]
        public void Withdraw_InsufficientBalance_BalanceUnchanged()
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(1111, 1111, 1, 0);

            atm.WithdrawCash(1000);

            Assert.AreEqual(atm.GetBalance(), "1");
        }
        private static IEnumerable Balances
        {
            get
            {
                yield return new TestCaseData(1).Returns("1");
                yield return new TestCaseData(0).Returns("0");
                yield return new TestCaseData(-1).Returns("-1");
            }
        }

        [TestCaseSource("Balances")]
        public string Balance_CorrectPin_ReturnBalance(int balance)
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(1111, 1111, balance, 1);

            var result = atm.GetBalance();

            return result;
        }

        [Test]
        public void Balance_IncorrectPin_ReturnAccountError()
        {
            var atm = new Atm(int.MaxValue);
            atm.BeginSession(1111, 0, 1, 1);

            var result = atm.GetBalance();

            Assert.AreEqual(result, "ACCOUNT_ERR");
        }
    }
}