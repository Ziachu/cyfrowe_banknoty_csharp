using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyfroweBanknoty.Users;
using CyfroweBanknoty.Objects;

namespace CyfroweBanknoty.AliceProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            // --- step 0.
            // --- --- generating unique Alice identifiers

            Console.WriteLine("[info]: Creating Alice user instance.");
            Alice alice = new Alice();

            // --- step 1. (generating 100 banknotes)
            // --- --- hiding unique identifiers behind "l_secret" and "r_secret" series
            // --- --- 
            alice.GenerateBanknotes(123.45, 10);
            alice.banknotes[0].VisualizeBanknote();
            // --- printing Alice ids

            //Console.WriteLine("[info]: Alice ids:");
            //foreach(Series s in alice.alice_ids)
            //{
            //    Console.WriteLine(s);
            //}

            // --- reading Alice ids from file (saved to file during creation)

            //Console.WriteLine("\n[info]: Alice ids (read from file):");
            //foreach(Series s in alice.transmitter.ReadSeriesFromFile("alice_ids.txt"))
            //{
            //    Console.WriteLine(s);
            //}

            alice.EstablishConnectionWithBank();

            Console.WriteLine("[info]: Getting public key from Bank...");
            alice.GetPublicKeyFromBank();

            Console.WriteLine("[info]: Hiding banknotes...");
            alice.HideBanknotes();

            // --- visualizing hidden banknotes

            alice.hidden_banknotes[0].VisualizeHiddenBanknote();

            Console.WriteLine("[info]: Sending hidden banknotes to Bank...");
            alice.SendHiddenBanknotes();
            Console.WriteLine("[info]: {0} banknotes sent.", alice.hidden_banknotes.Count());

            Console.WriteLine("[info]: Waiting for Bank to choose single banknote....");
            alice.ReceiveSelectedBanknoteIndex();
            Console.WriteLine("[info]: Received decision from Bank: " + alice.selected_banknote_index);

            Console.WriteLine("[info]: Sending to Bank elements required to reveal banknotes...");
            alice.RevealBanknotes();
            Console.ReadLine();
        }
    }
}
