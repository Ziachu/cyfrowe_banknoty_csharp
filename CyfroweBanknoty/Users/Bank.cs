using CyfroweBanknoty.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyfroweBanknoty.Objects;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;

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

        private Tools.RSA rsa;

        private Connection alice_connection;
        public List<HiddenBanknote> hidden_banknotes;

        public Bank()
        {
            rsa = new Tools.RSA(true);
            hidden_banknotes = new List<HiddenBanknote>();
        }

        public void SendPublicKeyTo(string to)
        {
            if (to == "Alice")
            {
                alice_connection.Send(0, Helper.GetBytes(rsa.GetPublicKey()));
                //Console.WriteLine("\t[debug]: publick_key_in_xml: " + rsa.GetPublicKey());
            } else if (to == "Vendor")
            {
                // ...
            }
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
         
        public void ReceiveHiddenBanknotes()
        {
            int no_banknotes = BitConverter.ToInt32(alice_connection.Receive(0), 0);
            Console.WriteLine("[info] Waiting for {0} banknotes.", no_banknotes);

            for (int i = 0; i < no_banknotes; i++)
            {
                var banknote = new HiddenBanknote();
                banknote.Receive(alice_connection);
                hidden_banknotes.Add(banknote);
                //Console.WriteLine("\t[debug]: Received and acknowledged!");
            }
        }
           
        public void pickOneBanknote()
          {
            Random rand = new Random();
            int banknote_index = rand.Next(0,99);
            alice_connection.Send(1, Helper.GetBytes(Helper.GetIntBinaryString(banknote_index)));
          }

        public void checkBanknotes()
        {
            //
            Console.WriteLine("I'm waiting for 99 banknotes");
            List<Banknote> banknotes = new List<Banknote>();
            List<Series> s_series;
            List<Series> b_series;
            List<Series> l_series;

            List<Series> t_series;
            List<Series> c_series;
            List<Series> r_series;

            //Sprawdzamy czy kwota sie zgadza na kazdym banknocie
            if (banknotes.Count == 99)
            {
                for (int i = 0; i < 99; i++)
                {
                    for (int j = 0; j < 99; j++)
                    {
                        if (banknotes[i].amount == banknotes[j].amount) { }
                        else
                        {
                            Console.WriteLine("Kwoty sie nie zgadzaja, Alice dopuscila sie proby oszustwa.");
                            break;
                        }
                    }
                }

                //Sprawdzamy czy kazdy banknot ma różne identyfikatory
                if (banknotes.Count == 99)
                {
                    for (int i = 0; i < 99; i++)
                    {
                        for (int j = 0; j < 99; j++)
                        {
                            if (banknotes[i].id == banknotes[j].id)
                            {

                                Console.WriteLine("Identyfikatory sie pokrywaja, Alice dopuscila sie proby oszustwa.");
                                break;
                            }
                            else { continue; }
                        }
                    }
                }
                //Weryfikujemy zobowiazania bitowe
                if (banknotes.Count == 99)
                {

                }
            }
        }

        public void sendRSAPublicKey(Tools.RSA rsa)
        {
            var rsa_key = rsa.GetPublicKey();
            byte[] rsa_kbytes = Helper.GetBytes(rsa_key);
            alice_connection.Send(1, Helper.GetBytes(rsa_key));
        }
        }
    }
