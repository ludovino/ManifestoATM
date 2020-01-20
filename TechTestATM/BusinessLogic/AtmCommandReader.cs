using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ATM
{
    public class AtmCommandReader
    {
        private readonly Func<int, IAtm> _atmFactory;
        private IAtm _atm;
        private Queue<string> _commandQueue;
        private StringBuilder _output;

        private const string _accountPattern = @"^\d+ \d+ \d+.$";
        private const string _balanceOverdraftPattern = @"^\d+ \d+$";
        private const string _withdrawalPattern = @"^W \d+$";

        public object atm { get; private set; }

        public AtmCommandReader(Func<int, IAtm> atmFactory)
        {
            _atm = null;
            _atmFactory = atmFactory;
            _output = new StringBuilder();
            _commandQueue = new Queue<string>();
        }

        public string Read(IEnumerable<string> commands)
        {
            foreach (string command in commands)
            {
                _commandQueue.Enqueue(command.Trim());
            }

            if (_atm == null)
            {
                _initializeAtm();
            }

            while (_commandQueue.Count > 0)
            {
                _beginSession();

                // drop out of loop if there are no more commands, or an empty line indicates session end
                while (_commandQueue.Count > 0 && !string.IsNullOrEmpty(_commandQueue.Peek()))
                {
                    _executeSessionCommands();
                }
                _endSession();
            }
            string result = _output.ToString();
            _output.Clear();
            return result;
        }

        private void _initializeAtm()
        {
            int initialBalance;
            // expect an initial cash value followed by an empty line
            if (int.TryParse(_commandQueue.Dequeue(), out initialBalance) && string.IsNullOrWhiteSpace(_commandQueue.Dequeue()))
            {
                _atm = _atmFactory(initialBalance);
            }
            else
            {
                throw new InvalidOperationException("Invalid Input. Expected initial cash as a single integer followed by a blank line");
            }
        }

        private void _beginSession()
        {
            string accountLine = _commandQueue.Dequeue();
            string balanceLine = _commandQueue.Dequeue();
            if (new Regex(_accountPattern).IsMatch(accountLine) && new Regex(_balanceOverdraftPattern).IsMatch(balanceLine))
            {
                var acc = accountLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
                var bal = balanceLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

                _atm.BeginSession(acc[1], acc[2], bal[0], bal[1]);
            }
            else
            {
                throw new InvalidOperationException("Invalid Input. Expected [account number] [pin] [user input pin] then next line [balance] [overdraft]");
            }
        }
        private void _executeSessionCommands()
        {
            // add new line after first output
            if(_output.Length > 0)
            {
                _output.AppendLine();
            }

            var command = _commandQueue.Dequeue();
            if (command == "B")
            {
                _output.Append(_atm.GetBalance());
            }
            else if (new Regex(_withdrawalPattern).IsMatch(command))
            {
                int amount = int.Parse(command.Split(' ')[1]);
                _output.Append(_atm.WithdrawCash(amount));
            } 
            else
            {
                throw new InvalidOperationException("Invalid Input. Expecte B for balance request or W [withdrawalAmount] for withdrawal");
            }

        }
        private void _endSession()
        {
            _commandQueue.TryDequeue(out _);
            _atm.EndSession();
        }
    }
}
