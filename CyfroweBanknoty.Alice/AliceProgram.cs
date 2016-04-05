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

            //// --- --- establishing connection with Bank
            //alice.EstablishConnectionWithBank();

            //Console.WriteLine("[info]: Getting public key from Bank...");
            //alice.GetPublicKeyFromBank();

            // --- --- establishing connection with herself (creating keys)
            alice.rsa = new RSA(true);

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

            //// --- step 3. Alice sends hidden banknotes over to Bank
            //// --- --- sending hidden banknotes to Bank
            //Console.WriteLine("[info]: Sending hidden banknotes to Bank...");
            //alice.SendHiddenBanknotes();
            //Console.WriteLine("[info]: {0} banknotes sent.", alice.hidden_banknotes.Count());

            //// --- --- receiving index of selected banknote
            //Console.WriteLine("[info]: Waiting for Bank to choose single banknote....");
            //alice.ReceiveSelectedBanknoteIndex();
            //Console.WriteLine("[info]: Received decision from Bank: " + alice.selected_banknote_index);

            //// --- step 5. Alice reveals hidden banknotes
            //Console.WriteLine("[info]: Sending to Bank elements required to reveal banknotes...");
            //alice.RevealBanknotes();

            // --- --- revealing hidden banknotes
            List<Banknote> revealed_banknotes = new List<Banknote>();

            for (int i = 0; i < alice.hidden_banknotes.Count(); i++)
            {
                    var r = alice.secrets[i];
                    var y = alice.hidden_banknotes[i];
                    var m = new Banknote();

                    m.amount = BitConverter.ToInt64(alice.rsa.UnblindObject(y.amount, r), 0);
                    m.id = BitConverter.ToInt64(alice.rsa.UnblindObject(y.id, r), 0);

                    Console.WriteLine("\t[debug]: Revealed banknote {0}\t\twith secret {1}.", m.id, r);

                    m.s_series = new List<Series>();
                    m.t_series = new List<Series>();
                    m.u_hashes = new List<byte[]>();
                    m.w_hashes = new List<byte[]>();

                    for (int j = 0; j < y.s_series.Count(); j++)
                    {
                        m.s_series.Add(new Series(alice.rsa.UnblindObject(y.s_series[j], r)));
                        m.t_series.Add(new Series(alice.rsa.UnblindObject(y.t_series[j], r)));
                        m.u_hashes.Add(alice.rsa.UnblindObject(y.u_hashes[j], r));
                        m.w_hashes.Add(alice.rsa.UnblindObject(y.w_hashes[j], r));
                    }

                    revealed_banknotes.Add(m);

                    //Console.WriteLine("\t[debug]: Banknote number {0}. revealed.", revealed_banknotes[i].id);
            }

            

            //Console.ReadLine();

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

        }
    }
}
