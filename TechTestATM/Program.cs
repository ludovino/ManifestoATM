using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace ATM
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex accountDetails = new Regex(@"^\d+ \d+ \d+.$");
            Regex initialBalance = new Regex(@"^\d+ \d+$");
            Regex balanceEnquiry = new Regex(@"^B$");
            Regex withdrawal = new Regex(@"^W \d+$");

            var commands = new Queue<string>(File.ReadAllLines(args[0]).Select(s => s.Trim()));
            
            // expect a single string of digits followed by an empty line
            int startingCash;
            if(!int.TryParse(commands.Dequeue(), out startingCash))
            {
                Console.WriteLine("Invalid Input. Expected initial cash as a single integer");
                return;
            }
            if (!string.IsNullOrWhiteSpace(commands.Dequeue()))
            {
                Console.WriteLine("Invalid Input. Expected empty line after initial ATM cash");
                return;
            }

            // initialise the atm with a balance of cash
            IAtm atm = new Atm(startingCash);

            while (commands.Count > 0)
            {
                List<int> account;
                List<int> balances;
                
                if(!accountDetails.IsMatch(commands.Peek()))
                {
                    Console.WriteLine("Invalid input. Expected [account number] [pin] [user input pin] as integers separated by spaces");
                    return;
                }
                account = commands.Dequeue().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

                if (!initialBalance.IsMatch(commands.Peek()))
                {
                    Console.WriteLine("Invalid input. Expected [balance] [overdraft] as integers separated by a space");
                    return;
                }
                balances = commands.Dequeue().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
                
                // parameters are pin, user input pin, balance, overdraft. account number is not used.
                atm.BeginSession(account[1], account[2], balances[0], balances[1]);
                
                while (commands.Count > 0)
                {
                    string command = commands.Dequeue();

                    if (string.IsNullOrWhiteSpace(command))
                    {
                        atm.EndSession();
                        break;
                    }

                    if (command == "B")
                    {
                        Console.WriteLine(atm.GetBalance());
                    }

                    else if (withdrawal.IsMatch(command))
                    {
                        int amount = int.Parse(command.Split(' ')[1]);
                        Console.WriteLine(atm.WithdrawCash(amount));
                    } else
                    {
                        Console.WriteLine("Command not recognised");
                        return;
                    }
                }
            }
        }
    }
}

