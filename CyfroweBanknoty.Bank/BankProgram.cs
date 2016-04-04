using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyfroweBanknoty.Users;
using Org.BouncyCastle.Crypto;

namespace CyfroweBanknoty.BankProgram
{
    public class BankProgram
    {
        static void Main(string[] args)
        {
            Bank bank = new Bank();
            Console.WriteLine("[info]: Bank instance initialized.");

            // --- --- establishing connection with Alice
            Console.WriteLine("[info]: Estabilishing connection with Alice...");
            bank.EstablishConnectionWithAlice();

            // --- --- sending public key to Alice
            Console.WriteLine("[info]: Sending public key to Alice.");
            bank.SendPublicKeyTo("Alice");

            // --- --- receiving hidden banknotes from Alice
            Console.WriteLine("[info]: Waiting for banknotes from Alice...");
            bank.ReceiveHiddenBanknotes();

            // step 4. Bank picks one banknote and demands disclosure of the rest
            Console.WriteLine("[info]: {0} banknotes received.", bank.hidden_banknotes.Count());

            // --- --- visualizing hidden banknote
            bank.hidden_banknotes[0].VisualizeHiddenBanknote();

            // --- --- picking single banknote (to sign later)
            Console.WriteLine("[info]: Picking single banknote to sign...");
            bank.PickOneBanknote();
            Console.WriteLine("[info]: I've selected banknote with index {0}, decision sent to Alice.\n\tWaiting for disclosure of the rest...", bank.banknote_index);

            // --- --- receiving missing banknote parts
            bank.ReceiveMissingBanknotesPartsFromAlice();

            Console.WriteLine("[info]: Revealing hidden banknotes...");
            bank.RevealHiddenBanknotes();
            Console.WriteLine("[info]: {0} banknotes revealed.", bank.revealed_banknotes.Count() - 1);

            // --- step 6. Bank verifies banknotes


            Console.ReadLine();
        }
    }
}
