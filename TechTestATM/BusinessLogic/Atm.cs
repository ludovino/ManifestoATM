using System;
using System.Collections.Generic;
using System.Text;

namespace ATM
{
    public class Atm : IAtm
    {
        private int _cash;

        private ISession _session;
        private bool _invalidPin;

        private const string ACCOUNT_ERR = "ACCOUNT_ERR";
        private const string FUNDS_ERR = "FUNDS_ERR";
        private const string ATM_ERR = "ATM_ERR";
        public Atm(int cash)
        {
            _cash = cash;
        }
        public void BeginSession(int accountPin, int pinEntered, int balance, int overdraft)
        {

            if (_session != null)
            {
                throw new InvalidOperationException("Cannot start a new session before the previous session has ended");
            }

            _invalidPin = pinEntered != accountPin;

            _session = new Session(balance, overdraft);
        }

        public void EndSession()
        {
            _session = null;
        }

        public string GetBalance()
        {
            if (_invalidPin)
            {
                return ACCOUNT_ERR;
            }

            return _session.GetBalance().ToString();
        }

        public string WithdrawCash(int amount)
        {
            if(amount <= 0)
            {
                throw new ArgumentOutOfRangeException("cannot withdraw a negative amount");
            }
            if (_invalidPin)
            {
                return ACCOUNT_ERR;
            }

            if (amount > _cash)
            {
                return ATM_ERR;
            }

            if (_session.HasSufficientBalance(amount))
            {
                var result = _session.WithdrawCash(amount).ToString();
                _cash -= amount;
                return result;
            }
            else
            {
                return FUNDS_ERR;
            }
        }
    }
}
