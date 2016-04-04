using CyfroweBanknoty.Objects;
using CyfroweBanknoty.Tools;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace CyfroweBanknoty.Users
{
    public class Alice
    {
        // main protocol implementation (Alice)

        // ------
        // PART I
        // ------
        // step 1. Alice prepares banknotes                             ✓
        // step 2. Alice hides created banknotes                        ✓
        // step 3. Alice sends hidden banknotes over to Bank            ✓
        // [jump to Bank.cs][step 4.]
        // step 5. Alice reveals hidden banknotes                       ✓
        // [jump to Bank.cs][steps 6.-8.]

        // -------
        // PART II
        // -------
        // step 9. Alice pays (sends over banknote) Vendor for sth
        // [jump to Vendor.cs][steps 10.-11.]
        // step 12. Alice partially reveals her id series to Vendor

        // --------
        // PART III
        // --------
        // [jump to Vendor.cs][steps 13.-14.]
        // [jump to Bank.cs][step 15.-16.]

        // ---------------------------------------------------------

        public List<Banknote> banknotes;
        public List<HiddenBanknote> hidden_banknotes;
        public int selected_banknote_index;

        // xor alice_ids;
        public List<Series> l_secret;
        public List<Series> r_secret;

        // commitment scheme (pl. zobowiązanie bitowe)
        public List<Series> t_series;
        public List<Series> c_series;
        public List<Series> s_series;
        public List<Series> b_series;
        public List<byte[]> w_hashes;
        public List<byte[]> u_hashes;

        // secret to blind object
        public List<BigInteger> secrets;

        public string unique_identifiers_file;
        public List<Series> alice_ids;

        private Connection bank_connection;
        private Connection vendor_connection;
        public Transmitter transmitter;

        public Tools.RSA rsa;

        public Alice()
        {
            banknotes = new List<Banknote>();
            alice_ids = new List<Series>();
            l_secret = new List<Series>();
            r_secret = new List<Series>();
            transmitter = new Transmitter();

            rsa = new Tools.RSA(false);

            GenerateAliceIdentifiers(5, 10, "alice_ids.txt");
        }

        // step 1.
        public void GenerateBanknotes(int amount, int no_banknotes)
        {
            Console.WriteLine("[info]: Drawing right secrets...");
            DrawRightSecret();
            Console.WriteLine("[info]: XORing left secrets...");
            XORLeftSecret();
            Console.WriteLine("[info]: Commiting scheme for right & left secrets...");
            CommitSchemes();

            List<int> ids = GenerateRandomIds(no_banknotes);

            Console.Write("[info]: Generating banknotes: ");
            for (int i = 0; i < no_banknotes; i++)
            {
                Console.Write('.');
                banknotes.Add(new Banknote(amount, ids[i], s_series, t_series, u_hashes, w_hashes));
            }

            Console.WriteLine("");
        }

        public void GetPublicKeyFromBank()
        {
            if (bank_connection.socket.Connected)
            {
                var public_key_in_bytes = bank_connection.Receive(1);
                var public_key_in_xml = Helper.GetString(public_key_in_bytes);

                //Console.WriteLine("\t[debug]: publick_key_in_xml: " + public_key_in_xml);

                rsa.SetPublicKey(public_key_in_xml);

                Console.WriteLine("[info]: I've got public key from Bank!");
            } else
            {
                Console.WriteLine("[fail] Establish connection with Bank first!");
            }
        }

        public void EstablishConnectionWithBank()
        {
            bank_connection = new Connection();

            try
            {
                bank_connection.socket.Connect(bank_connection.ipEndPoint);
                Console.WriteLine("[info]: Connection accepted.");
            }
            catch (SocketException e)
            {
                Console.WriteLine("[fail]: Something went wrong!\n" + e.Message);
            }
        }

        private void CommitSchemes()
        {
            t_series = new List<Series>();
            c_series = new List<Series>();
            s_series = new List<Series>();
            b_series = new List<Series>();

            var length = r_secret[0].length;

            Console.Write("[info]: Drawing series of length {0}, required to hash secrets: ", length);
            for (int i = 0; i < alice_ids.Count(); i++)
            {
                Console.Write('.');
                t_series.Add(new Series(length));
                Console.WriteLine("printint t_series[{0}]: {1}", i, t_series[i]);
                c_series.Add(new Series(length));
                s_series.Add(new Series(length));
                b_series.Add(new Series(length));
            }

            w_hashes = new List<byte[]>();
            u_hashes = new List<byte[]>();

            var sha1 = new SHA1CryptoServiceProvider();

            Console.Write("\n[info]: Hashing secrets: ");
            for (int i = 0; i < alice_ids.Count(); i++)
            {
                // --- hashing r_secrets with t_series and c_series
                Console.Write('.');

                byte[] data = Helper.CombineByteArrays(t_series[i].values, c_series[i].values);
                data = Helper.CombineByteArrays(data, r_secret[i].values);

                var hash = sha1.ComputeHash(data);
                w_hashes.Add(hash);

                // --- debug, see the results
                //Console.WriteLine("Before hashing:");
                //Helper.PrintBytes(data);
                //Console.WriteLine("After hashing:");
                //Helper.PrintBytes(hash);

                // --- hashin l_secrets with s_series and b_secrets
                Console.Write('.');

                data = Helper.CombineByteArrays(s_series[i].values, b_series[i].values);
                data = Helper.CombineByteArrays(data, l_secret[i].values);

                hash = sha1.ComputeHash(data);
                u_hashes.Add(hash);
            }

            Console.WriteLine("");
        }

        public void DrawRightSecret()
        {
            var length = alice_ids[0].length;

            for (int i = 0; i < alice_ids.Count(); i++)
            {
                r_secret.Add(new Series(length));
            }
        }

        public void XORLeftSecret()
        {
            var length = alice_ids[0].length;

            for (int i = 0; i < alice_ids.Count(); i++)
            {
                byte[] values = new byte[length];

                for (int j = 0; j < length; j++)
                    values[j] = (byte)(alice_ids[i].values[j] ^ r_secret[i].values[j]);

                l_secret.Add(new Series(length, values));
            }
        }

        public void DrawSecrets()
        {
            var length = banknotes.Count();

            for (int i = 0; i< length; i++)
            {
                secrets.Add(rsa.DrawR());
            }
        }

        public void GenerateAliceIdentifiers(int no_ids, int length_of_id, string file)
        {
            for (int i = 0; i < no_ids; i++)
            {
                Series id = new Series(length_of_id);

                alice_ids.Add(id);
                transmitter.WriteSeriesToFile(alice_ids, file);
            }

            unique_identifiers_file = file;
        }

        private List<int> GenerateRandomIds(int no_ids)
        {
            Random random = new Random();
            List<int> ids = new List<int>();

            do
            {
                Console.Write("\n[info]: Generating random ids for banknotes: ");
                for (int i = 0; i < no_ids; i++)
                {
                    Console.Write('.');
                    ids.Add(random.Next(100000, 999999));
                }

            } while (ids.Distinct().Count() != ids.Count());

            Console.WriteLine("");
            return ids;
        }

        public void HideBanknotes()
        {
            Console.WriteLine("[info]: Drawing secrets...");
            secrets = new List<BigInteger>();
            DrawSecrets();

            hidden_banknotes = new List<HiddenBanknote>();

            for (int i = 0; i < banknotes.Count(); i++)
            {
                hidden_banknotes.Add(new HiddenBanknote());
                byte[] am = Helper.GetBytesInteger(banknotes[i].amount);
                hidden_banknotes[i].amount = rsa.BlindObject(am, secrets[i]);

                byte[] id = BitConverter.GetBytes(banknotes[i].id);
                hidden_banknotes[i].id = rsa.BlindObject(id, secrets[i]);

                for (int j = 0; j < alice_ids.Count(); j++)
                {
                    hidden_banknotes[i].s_series.Add(rsa.BlindObject(banknotes[i].s_series[j].values, secrets[i]));
                    hidden_banknotes[i].t_series.Add(rsa.BlindObject(banknotes[i].t_series[j].values, secrets[i]));
                    hidden_banknotes[i].u_hashes.Add(rsa.BlindObject(banknotes[i].u_hashes[j], secrets[i]));
                    hidden_banknotes[i].w_hashes.Add(rsa.BlindObject(banknotes[i].w_hashes[j], secrets[i]));
                }

                Console.WriteLine("[info]: {0}. hidden.", i);
            }
        }

        public void SendHiddenBanknotes()
        {
            if (hidden_banknotes.Count() > 0)
            {
                bank_connection.Send(1, BitConverter.GetBytes(banknotes.Count()));
                //hidden_banknotes[0].Send(bank_connection);

                for (int i = 0; i < banknotes.Count(); i++)
                {
                    hidden_banknotes[i].Send(bank_connection);
                }
            }
        }

        public void ReceiveSelectedBanknoteIndex()
        {
            selected_banknote_index = BitConverter.ToInt32(bank_connection.Receive(1), 0);
        }

        public void RevealBanknotes()
        {
            Console.WriteLine("\t[debug]: Sending {0} secrets.", secrets.Count() - 1);

            bank_connection.Send(1, BitConverter.GetBytes(secrets.Count()));
            bank_connection.Receive(1);

            for (int i = 0; i < secrets.Count(); i++)
            {
                if (i != selected_banknote_index)
                {
                    bank_connection.Send(1, secrets[i].ToByteArray());
                    bank_connection.Receive(1);
                    Console.WriteLine("\t[debug]: Secret {0} sent: {1}", i, secrets[i]);
                } else
                {
                    Console.WriteLine("\t[debug]: Secret {0} ommited.", i);
                }
            }

            Console.WriteLine("[info] {0} secrets sent.", secrets.Count());

            Console.WriteLine("[info] {0} series to be sent.", s_series.Count() * 6);
            bank_connection.Send(1, BitConverter.GetBytes(s_series.Count()));
            bank_connection.Receive(1);

            for (int j = 0; j < s_series.Count(); j++)
            {
                s_series[j].Send(bank_connection);
                b_series[j].Send(bank_connection);
                l_secret[j].Send(bank_connection);

                t_series[j].Send(bank_connection);
                c_series[j].Send(bank_connection);
                r_secret[j].Send(bank_connection);

                Console.WriteLine("\t[debug]: {0} sent.", (j + 1) * 6);
            }

            Console.WriteLine("[info] {0} series (S, B, L, T, C, R) sent.", s_series.Count() * 6);

            //Console.WriteLine("[info] Elements required to reveal banknote sent.");
        }

        private void SendShownBankotes(List<Banknote> banknotes, int index)
        {
            //Przygotowanie listy banknotow do wyslania
            List<Banknote> banknotes_to_send = new List<Banknote>();

            for (int i=0; i<99; i++)
            {
                if (i != index)
                {
                    banknotes_to_send[i] = banknotes[i]; 
                }
                if (i == index)
                {
                    banknotes_to_send[i] = null;
                }
            }
            //przerabianie poszczegolnych pól na bity i wysylanie
            for (int i=0; i<99; i++)
            {
                byte[] amount_byte = BitConverter.GetBytes(banknotes_to_send[i].amount);
                //wysylamy kwote
                byte[] id_byte = BitConverter.GetBytes(banknotes_to_send[i].id);

                for (int k = 0; k < 99; k++)
                {
                    byte[] s_byte = banknotes_to_send[i].s_series[k].values;
                    //wysylamy
                    byte[] t_byte = banknotes_to_send[i].t_series[k].values;
                    //wysylamy
                    byte[] u_byte = banknotes_to_send[i].u_hashes[k];
                    //wysylamy
                    byte[] w_byte = banknotes_to_send[k].u_hashes[k];
                    //wysylamy
                }

            }
            //wysylamy
        }
    }
}
    