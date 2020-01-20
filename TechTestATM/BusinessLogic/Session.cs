using System;

namespace ATM
{
    internal class Session : ISession
    {
        private int _balance;
        private int _overdraft;

        public Session(int balance, int overdraft)
        {
            _balance = balance;
            _overdraft = overdraft;
        }
        public int GetBalance()
        {
            return _balance;
        }

        public bool HasSufficientBalance(int amount)
        {
            var availableBalance = checked(_balance + _overdraft);
            return availableBalance >= amount;
        }

        public int WithdrawCash(int amount)
        {
            if(_balance + _overdraft < amount)
            {
                throw new ArgumentException("account does not have sufficient balance");
            }
            _balance -= amount;
            return _balance;
        }
    }
}