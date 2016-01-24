using CyfroweBanknoty.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyfroweBanknoty.Objects;

namespace CyfroweBanknoty.Users
{
    public class Bank
    {
        // ------
        // PART I
        // ------
        // step 4. Bank picks one banknote and demands disclosure of the rest
        // step 6. Bank verifies 99 banknotes
        // step 7. Bank signs picked banknote
        // step 8. Bank sends signed banknote over to Alice

        // --------
        // PART III
        // --------
        // step 15. Bank verifies signature under banknote
        // step 16. Bank verifies if banknote was not used before

        private RSA rsa;

        private Connection alice_connection;

        public Bank()
        {
            rsa = new RSA();
        }

        public void EstablishConnectionWithAlice()
        {
            alice_connection = new Connection();
            alice_connection.socket.Bind(alice_connection.ipEndPoint);

            Console.WriteLine("[info]: Waiting for connection...");
            alice_connection.socket.Listen(1);

            alice_connection.handler = alice_connection.socket.Accept();
            Console.WriteLine("[info]: Connection accepted.");
        }

        public void XORSecrets(List<Series> r_secret, List<Series> l_secret)
        {
            List<Series> alice_ids_in_theory = new List<Series>();

            if (r_secret.Count() == l_secret.Count())
            {
                var length = r_secret[0].length;
                for (int i = 0; i < r_secret.Count(); i++)
                {
                    byte[] values = new byte[length];

                    for (int j = 0; j < length; j++)
                        values[j] = (byte)(r_secret[i].values[j] ^ l_secret[i].values[j]);

                    alice_ids_in_theory.Add(new Series(length, values));
                }

                //var same = true;

                //for (int i = 0; i < alice_ids.Count(); i++)
                //{
                //    if (!alice_ids[i].values.SequenceEqual(alice_ids_in_theory[i].values))
                //    {
                //        same = false;
                //        Console.WriteLine("orig: {0}\nfake: {1}", alice_ids[i], alice_ids_in_theory[i]);
                //    }
                //}

                //if (same)
                //{
                //    Console.WriteLine("They're equal!");
                //}
            }
            else
            {
                Console.WriteLine("Secrets cannot be XORed (because of different lengths).");
            }
        }
    }
}
