using ATM;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATMTests
{
    class AtmCommandReaderTests
    {
        private int initialBalance;
        private int accountNumber;
        private int pinNumber;
        private int userPin;
        private int balance;
        private int overdraft;
        private int withdrawal;

        private string[] validInputSingleSessionBalanceEnquiry;
        private string[] validInputSingleSessionWithdrawal;
        private string[] validInputSingleSessionBalanceWithdrawal;
        private string[] validInputMultipleSessions;

        [SetUp]
        protected void setup()
        {
            initialBalance = 8000;
            accountNumber = 12345678;
            pinNumber = 1111;
            userPin = 5555;
            balance = 100;
            overdraft = 50;
            withdrawal = 10;

            validInputSingleSessionBalanceEnquiry = new string[]
            { 
                $"{initialBalance}",
                "",
                $"{accountNumber} {pinNumber} {userPin}",
                $"{balance} {overdraft}",
                "B",
            };
            validInputSingleSessionWithdrawal = new string[]
            {
                $"{initialBalance}",
                "",
                $"{accountNumber} {pinNumber} {userPin}",
                $"{balance} {overdraft}",
                $"W {withdrawal}",
            };
            validInputSingleSessionBalanceWithdrawal = new string[]
            {
                $"{initialBalance}",
                "",
                $"{accountNumber} {pinNumber} {userPin}",
                $"{balance} {overdraft}",
                "B",
                $"W {withdrawal}",
            };
            validInputMultipleSessions = new string[]
            {
                $"{initialBalance}",
                "",
                $"{accountNumber} {pinNumber} {userPin}",
                $"{balance} {overdraft}",
                "B",
                $"W {withdrawal}",
                "",
                $"{accountNumber} {pinNumber} {userPin}",
                $"{balance} {overdraft}",
                $"W {withdrawal}",
                "B"
            };
        }

        [Test]
        public void Read_ValidInput_AtmInitialized()
        {
            var mockAtm = new Mock<IAtm>();
            var mockAtmFactory = new Mock<Func<int, IAtm>>();
            mockAtmFactory.Setup(x => x(It.IsAny<int>())).Returns(mockAtm.Object);

            var reader = new AtmCommandReader(mockAtmFactory.Object);

            reader.Read(validInputSingleSessionBalanceEnquiry);

            mockAtmFactory.Verify(x => x(initialBalance), Times.Once);
        }

        [Test]
        public void Read_ValidInput_Balance()
        {
            var mockAtm = new Mock<IAtm>();
            var bal = "BALANCE";
            mockAtm.Setup(atm => atm.GetBalance()).Returns(bal);

            var reader = new AtmCommandReader((x) => mockAtm.Object);
            var output = reader.Read(validInputSingleSessionBalanceEnquiry);
            Assert.AreEqual(bal, output);
        }

        [Test]
        public void Read_ValidInput_BeginSession()
        {
            var mockAtm = new Mock<IAtm>();
            var reader = new AtmCommandReader((x) => mockAtm.Object);

            var output = reader.Read(validInputSingleSessionBalanceEnquiry);

            mockAtm.Verify(x => x.BeginSession(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Read_ValidInput_EndSession()
        {
            var mockAtm = new Mock<IAtm>();
            var reader = new AtmCommandReader((x) => mockAtm.Object);

            var output = reader.Read(validInputSingleSessionBalanceEnquiry);

            mockAtm.Verify(x => x.EndSession(), Times.Once);
        }

        [Test]
        public void Read_ValidInput_Withdrawal()
        {
            var mockAtm = new Mock<IAtm>();
            var wid = "WITHDRAWAL";
            mockAtm.Setup(atm => atm.WithdrawCash(It.IsAny<int>())).Returns(wid);

            var reader = new AtmCommandReader((x) => mockAtm.Object);
            var output = reader.Read(validInputSingleSessionWithdrawal);
            Assert.AreEqual(wid, output);
        }

        [Test]
        public void Read_ValidInput_BalanceThenWithdrawal()
        {
            var mockAtm = new Mock<IAtm>();
            var bal = "BALANCE";
            var wid = "WITHDRAWAL";

            mockAtm.Setup(atm => atm.WithdrawCash(It.IsAny<int>())).Returns(wid);
            mockAtm.Setup(atm => atm.GetBalance()).Returns(bal);

            var reader = new AtmCommandReader((x) => mockAtm.Object);
            var output = reader.Read(validInputSingleSessionBalanceWithdrawal);
            Assert.AreEqual("BALANCE\r\nWITHDRAWAL", output);
        }

        [Test]
        public void Read_ValidInput_MultipleSessionsOutput()
        {
            var mockAtm = new Mock<IAtm>();
            var bal = "BALANCE";
            var wid = "WITHDRAWAL";

            mockAtm.Setup(atm => atm.WithdrawCash(It.IsAny<int>())).Returns(wid);
            mockAtm.Setup(atm => atm.GetBalance()).Returns(bal);

            var reader = new AtmCommandReader((x) => mockAtm.Object);
            var output = reader.Read(validInputMultipleSessions);
            Assert.AreEqual("BALANCE\r\nWITHDRAWAL\r\nWITHDRAWAL\r\nBALANCE", output);
        }
    }
}
