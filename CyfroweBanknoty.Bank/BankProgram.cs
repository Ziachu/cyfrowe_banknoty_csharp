using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyfroweBanknoty.Users;

namespace CyfroweBanknoty.BankProgram
{
    public class BankProgram
    {
        static void Main(string[] args)
        {
            Bank bank = new Bank();
            Console.WriteLine("[info] Bank instance initialized.");

            Console.WriteLine("[info] Estabilishin connection with Alice...");
            bank.EstablishConnectionWithAlice();
        }
    }
}
