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
        // step 4. Bank picks one banknote and demands disclosure of the rest       ✓
        // step 6. Bank verifies banknotes
        // step 7. Bank signs picked banknote
        // step 8. Bank sends signed banknote over to Alice

        // --------
        // PART III
        // --------
        // step 15. Bank verifies signature under banknote
        // step 16. Bank verifies if banknote was not used before

        private Tools.RSA rsa;
        List<Banknote> used_banknotes = new List<Banknote>();

        private Connection alice_connection;
        public List<HiddenBanknote> hidden_banknotes;
        public List<Banknote> revealed_banknotes;
        public List<BigInteger> secrets;

        public List<Series> l_secret;
        public List<Series> r_secret;

        public List<Series> t_series;
        public List<Series> c_series;
        public List<Series> s_series;
        public List<Series> b_series;
        public List<byte[]> w_hashes;
        public List<byte[]> u_hashes;

        public int banknote_index;

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

                // every two banknotes...
                if (i % 2 == 0)
                {
                    // send acknowledge
                    alice_connection.Send(0, Helper.GetBytes("ack"));
                }
            }
        }
           
        public void PickOneBanknote()
        { 
            Random rand = new Random();
            banknote_index = rand.Next(0, hidden_banknotes.Count());

            alice_connection.Send(0, BitConverter.GetBytes(banknote_index));
        }

        public void ReceiveMissingBanknotesPartsFromAlice()
        {
            int no_secrets = BitConverter.ToInt32(alice_connection.Receive(0), 0);
            alice_connection.Send(0, new byte[1]);

            Console.WriteLine("\t[debug]: Waiting for {0} secrets.", no_secrets - 1);

            secrets = new List<BigInteger>();

            for (int i = 0; i < no_secrets; i++)
            {
                if (i != banknote_index)
                {
                    secrets.Add(new BigInteger(alice_connection.Receive(0)));
                    alice_connection.Send(0, new byte[1]);
                    Console.WriteLine("\t[debug]: Secret {0} received: {1}", i, secrets[i]);
                } else
                {
                    secrets.Add(new BigInteger("1"));
                    Console.WriteLine("\t[debug]: Secret for index {0} ommited.", i);
                }
            }

            Console.WriteLine("[info]: Received from Alice {0} secrets (to reveal banknotes).", secrets.Count() - 1);

            int no_series = BitConverter.ToInt32(alice_connection.Receive(0), 0);
            alice_connection.Send(0, new byte[1]);

            Console.WriteLine("\t[debug]: Waiting for {0} (S, B, L, T, C, R) series.", no_series * 6);

            t_series = new List<Series>();
            c_series = new List<Series>();
            s_series = new List<Series>();
            b_series = new List<Series>();

            l_secret = new List<Series>();
            r_secret = new List<Series>();

            var tmp = new Series();

            for (int i = 0; i < no_series; i++)
            {
                tmp.Receive(alice_connection);
                s_series.Add(tmp);

                tmp.Receive(alice_connection);
                b_series.Add(tmp);

                tmp.Receive(alice_connection);
                l_secret.Add(tmp);

                tmp.Receive(alice_connection);
                t_series.Add(tmp);

                tmp.Receive(alice_connection);
                c_series.Add(tmp);

                tmp.Receive(alice_connection);
                r_secret.Add(tmp);

                Console.WriteLine("\t[debug]: {0} received.", (i + 1) * 6);
            }

            Console.WriteLine("[info]: {0} series (S, B, L, T, C, R) received.", s_series.Count() * 6);
        }

        public void RevealHiddenBanknotes()
        {
            revealed_banknotes = new List<Banknote>();

            for (int i = 0; i < hidden_banknotes.Count(); i++)
            {
                if (i != banknote_index)
                {
                    var r = secrets[i];
                    var y = hidden_banknotes[i];
                    var m = new Banknote();

                    m.amount = BitConverter.ToInt64(rsa.UnblindObject(y.amount, r), 0);
                    m.id = BitConverter.ToInt64(rsa.UnblindObject(y.id, r), 0);

                    Console.WriteLine("\t[debug]: Revealing banknote {0}\t\twith secret {1}.", m.id, r);

                    m.s_series = new List<Series>();
                    m.t_series = new List<Series>();
                    m.u_hashes = new List<byte[]>();
                    m.w_hashes = new List<byte[]>();

                    for (int j = 0; j < y.s_series.Count(); j++)
                    {
                        m.s_series.Add(new Series(rsa.UnblindObject(y.s_series[j], r)));
                        m.t_series.Add(new Series(rsa.UnblindObject(y.t_series[j], r)));
                        m.u_hashes.Add(rsa.UnblindObject(y.u_hashes[j], r));
                        m.w_hashes.Add(rsa.UnblindObject(y.w_hashes[j], r));
                    }

                    revealed_banknotes.Add(m);

                    //Console.WriteLine("\t[debug]: Banknote number {0}. revealed.", revealed_banknotes[i].id);
                } else
                {
                    revealed_banknotes.Add(null);
                }
            }
        }

        public void checkBanknotes()
        {
            // sprawdzanie kwot
            foreach (Banknote banknoteX in revealed_banknotes)
            {
                foreach (Banknote banknoteY in revealed_banknotes)
                {
                    if (banknoteX != null && banknoteY != null)
                    {
                        if (banknoteX.amount != banknoteY.amount)
                        {
                            Console.WriteLine("\t[debug] {0} <--> {1}, kwoty sie nie zgadzaja, Alice dopuscila sie proby oszustwa.",
                                banknoteX.id, banknoteY.id);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\t[debug] {0}, kwoty są ok.", banknoteX.id);
                        }
                    }
                }
            }

            // sprawdzanie id
            foreach (Banknote banknoteX in revealed_banknotes)
            {
                foreach (Banknote banknoteY in revealed_banknotes)
                {
                    if (banknoteX != null && banknoteY != null)
                    {
                        if (banknoteX.id == banknoteY.id)
                        {
                            Console.WriteLine("\t[debug] {0} <--> {1}, id są takie same, Alice dopuscila sie proby oszustwa.",
                                banknoteX.id, banknoteY.id);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\t[debug] {0}, id są ok.", banknoteX.id);
                        }
                    }
                }
            }

            // sprawdzanie hash'y
            var sha1 = new SHA1CryptoServiceProvider();
            int no_series = s_series.Count();
            u_hashes = new List<byte[]>();
            w_hashes = new List<byte[]>();

            // najpierw obliczamy hash'e z otrzymanych odkrytych danych
            for (int x = 0; x < no_series; x++)
            {
                byte[] data = Helper.CombineByteArrays(s_series[x].values, b_series[x].values);
                data = Helper.CombineByteArrays(data, l_secret[x].values);
                var u_hash = sha1.ComputeHash(data);

                data = Helper.CombineByteArrays(t_series[x].values, c_series[x].values);
                data = Helper.CombineByteArrays(data, r_secret[x].values);
                var w_hash = sha1.ComputeHash(data);

                u_hashes.Add(u_hash);
                w_hashes.Add(w_hash);
            }

            // następnie porównujemy je do tych, które dostaliśmy w banknotach
            foreach (Banknote banknote in revealed_banknotes)
            {
                Console.WriteLine("Liczba ciągów w banknocie: {0}\nLiczba ciągów: {1}\nDługość tablicy hash'y: {2}", 
                    banknote.u_hashes.Count(),
                    no_series,
                    u_hashes.Count());

                if (banknote != null)
                {
                    for (int x = 0; x < no_series; x++)
                    {
                        if (banknote.u_hashes[x] != u_hashes[x])
                        {
                            Console.WriteLine("\t[debug] {0}, hashes different...", banknote.id);
                        }
                        else
                        {
                            Console.WriteLine("\t[debug] {0}, hashes OK!", banknote.id);
                        }
                    }
                }
            }
        }

        public void sendRSAPublicKey(Tools.RSA rsa)
        {
            var rsa_key = rsa.GetPublicKey();
            byte[] rsa_kbytes = Helper.GetBytes(rsa_key);
            alice_connection.Send(1, Helper.GetBytes(rsa_key));
        }

        public bool checkIfBanknoteIsNotUsed(Banknote banknote)
        {
            int length = used_banknotes.Count;
            for (int i = 0; i < length; i++)
            {
                if (banknote.id != used_banknotes[i].id && banknote.u_hashes != used_banknotes[i].u_hashes && banknote.w_hashes != used_banknotes[i].w_hashes)
                {
                    for (int j = 0; j < 99; j++)
                    {
                        if(banknote.s_series[j].values != used_banknotes[i].s_series[j].values && banknote.t_series[j].values != used_banknotes[i].t_series[j].values)
                        {

                        }
                        else { return false; }
                    }
                }
                else { return false; }
            }
            return true;
        }

        }
    }
