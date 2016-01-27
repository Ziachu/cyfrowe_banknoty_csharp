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

            Console.WriteLine("[info]: Estabilishing connection with Alice...");
            bank.EstablishConnectionWithAlice();
            Console.WriteLine("[info]: Sending public key to Alice.");
            bank.SendPublicKeyTo("Alice");

            Console.WriteLine("[info]: Waiting for banknotes from Alice...");
            bank.ReceiveHiddenBanknotes();
            Console.WriteLine("[info]: {0} banknotes received.", bank.hidden_banknotes.Count());

            // --- visualizing banknotes
            bank.hidden_banknotes[0].VisualizeHiddenBanknote();

            Console.WriteLine("[info]: Picking single banknote to sign...");
            bank.PickOneBanknote();
            Console.WriteLine("[info]: I've selected banknote with index {0}, decision sent to Alice.\n\tWaiting for disclosure of the rest...", bank.banknote_index);

            bank.ReceiveMissingBanknotesPartsFromAlice();
            Console.WriteLine("[info]: Received from Alice {0} secrets (to reveal banknotes).", bank.secrets.Count() - 1);
            Console.ReadLine();
        }
    }
}
