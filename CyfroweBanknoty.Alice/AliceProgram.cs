using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyfroweBanknoty.Users;
using CyfroweBanknoty.Objects;
using CyfroweBanknoty.Tools;
using Org.BouncyCastle.Math;

namespace CyfroweBanknoty.AliceProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            // --- step 1. Alice prepares banknotes
            
            // --- --- generating unique Alice identifiers
            Console.WriteLine("[info]: Creating Alice user instance.");
            Alice alice = new Alice();

            // --- --- generating 100 banknotes
            alice.GenerateBanknotes(123, 10);
            alice.banknotes[0].VisualizeBanknote();

            Console.WriteLine("[info]: Banknote IDs:");
            foreach (Banknote banknote in alice.banknotes)
            {
                Console.WriteLine("\t[debug]: {0}", banknote.id);
            }
            
            // --- --- printing Alice ids
            Console.WriteLine("[info]: Alice ids:");
            foreach (Series s in alice.alice_ids)
            {
                Console.WriteLine(s);
            }

            // --- --- reading Alice ids from file (saved to file during creation)
            Console.WriteLine("\n[info]: Alice ids (read from file):");
            foreach (Series s in alice.transmitter.ReadSeriesFromFile("alice_ids.txt"))
            {
                Console.WriteLine(s);
            }

            // --- --- establishing connection with Bank
            alice.EstablishConnectionWithBank();

            Console.WriteLine("[info]: Getting public key from Bank...");
            alice.GetPublicKeyFromBank();

            // --- step 2. Alice hides created banknotes
            Console.WriteLine("[info]: Hiding banknotes...");
            alice.HideBanknotes();

            // --- --- visualizing hidden banknotes
            alice.hidden_banknotes[0].VisualizeHiddenBanknote();

            Console.WriteLine("\n[debug]: Hidden banknote IDs in BigIntegers:");
            foreach (HiddenBanknote hidden_bank in alice.hidden_banknotes)
            {
                Console.WriteLine("\t" + hidden_bank.id);
            }

            // --- step 3. Alice sends hidden banknotes over to Bank
            // --- --- sending hidden banknotes to Bank
            Console.WriteLine("[info]: Sending hidden banknotes to Bank...");
            alice.SendHiddenBanknotes();
            Console.WriteLine("[info]: {0} banknotes sent.", alice.hidden_banknotes.Count());

            // --- --- receiving index of selected banknote
            Console.WriteLine("[info]: Waiting for Bank to choose single banknote....");
            alice.ReceiveSelectedBanknoteIndex();
            Console.WriteLine("[info]: Received decision from Bank: " + alice.selected_banknote_index);

            // --- step 5. Alice reveals hidden banknotes
            Console.WriteLine("[info]: Sending to Bank elements required to reveal banknotes...");
            alice.RevealBanknotes();


            //RSA rsa = new RSA(true);
            //BigInteger r = rsa.DrawR();
            ////Console.WriteLine("r: " + r);

            //string msg1 = "Hello World!";
            //byte[] bytes = Helper.GetBytes(msg1);
            //BigInteger m = new BigInteger(bytes);

            //BigInteger y = rsa.BlindObject(bytes, r);
            ////Console.WriteLine("y: " + y);

            //rsa.CheckEquality(m, r, y);

            //Console.ReadKey();

            //byte[] res = rsa.UnblindObject(y, r);

            //Console.WriteLine("equal? : " + res.Equals(bytes));
            //string msg2 = Encoding.UTF8.GetString(res);
            //Console.WriteLine("msg: " + msg2);

            //Console.WriteLine("equal? : " + msg1.Equals(msg2));
            Console.WriteLine(alice.s_series.Count());
        }
    }
}
