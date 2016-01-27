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
            Console.WriteLine("[info]: Bank instance initialized.");

            Console.WriteLine("[info]: Estabilishing connection with Alice...");
            bank.EstablishConnectionWithAlice();
            Console.WriteLine("[info]: Sending public key to Alice.");
            bank.SendPublicKeyTo("Alice");

            Console.WriteLine("[info]: Waiting for banknotes from Alice...");
            bank.ReceiveHiddenBanknotes();
            Console.WriteLine("[info]: {0} banknotes received.", bank.hidden_banknotes.Count());

            Console.WriteLine("[info]: Picking single banknote to sign...");
            bank.PickOneBanknote();
            Console.ReadLine();
        }
    }
}
