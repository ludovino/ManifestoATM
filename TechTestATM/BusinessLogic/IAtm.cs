namespace ATM
{
    public interface IAtm
    {
        void BeginSession(int accountPin, int pinEntered, int accountBalance, int accountOverdraft);
        void EndSession();
        string GetBalance();
        string WithdrawCash(int amount);
    }
}
