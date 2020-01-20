namespace ATM
{
    internal interface ISession
    {
        int GetBalance();
        bool HasSufficientBalance(int amount);
        int WithdrawCash(int amount);

    }
}
