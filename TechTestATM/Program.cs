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
            // first arg is the input file name
            var commands = File.ReadAllLines(args[0]);
            
            //TODO: IOC framework is a bit overboard for a demo.
            var commandReader = new AtmCommandReader(x => new Atm(x));

            var result = commandReader.Read(commands);

            Console.WriteLine(result);
        }
    }
}

