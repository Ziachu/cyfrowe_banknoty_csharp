﻿using System;
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

            //// --- step 0.
            //// --- --- generating unique Alice identifiers

            //Console.WriteLine("[info]: Creating Alice user instance.");
            //Alice alice = new Alice();

            //// --- step 1. (generating 100 banknotes)
            //// --- --- hiding unique identifiers behind "l_secret" and "r_secret" series
            //// --- --- 
            //alice.GenerateBanknotes(123.45, 10);
            //alice.banknotes[0].VisualizeBanknote();
            //// --- printing Alice ids

            ////Console.WriteLine("[info]: Alice ids:");
            ////foreach(Series s in alice.alice_ids)
            ////{
            ////    Console.WriteLine(s);
            ////}

            //// --- reading Alice ids from file (saved to file during creation)

            ////Console.WriteLine("\n[info]: Alice ids (read from file):");
            ////foreach(Series s in alice.transmitter.ReadSeriesFromFile("alice_ids.txt"))
            ////{
            ////    Console.WriteLine(s);
            ////}

            //alice.EstablishConnectionWithBank();

            //Console.WriteLine("[info]: Getting public key from Bank...");
            //alice.GetPublicKeyFromBank();

            //Console.WriteLine("[info]: Hiding banknotes...");
            //alice.HideBanknotes();

            //// --- visualizing hidden banknotes

            //alice.hidden_banknotes[0].VisualizeHiddenBanknote();

            //Console.WriteLine("[info]: Sending hidden banknotes to Bank...");
            //alice.SendHiddenBanknotes();
            //Console.WriteLine("[info]: {0} banknotes sent.", alice.hidden_banknotes.Count());

            //Console.WriteLine("[info]: Waiting for Bank to choose single banknote....");
            //alice.ReceiveSelectedBanknoteIndex();
            //Console.WriteLine("[info]: Received decision from Bank: " + alice.selected_banknote_index);

            //Console.WriteLine("[info]: Sending to Bank elements required to reveal banknotes...");
            //alice.RevealBanknotes();

            //Console.ReadLine();

            RSA rsa = new RSA(true);
            BigInteger r = rsa.DrawR();
            //Console.WriteLine("r: " + r);

            string msg1 = "Hello World!";
            byte[] bytes = Helper.GetBytes(msg1);
            BigInteger m = new BigInteger(bytes);

            BigInteger y = rsa.BlindObject(bytes, r);
            //Console.WriteLine("y: " + y);

            rsa.CheckEquality(m, r, y);

            //byte[] res = rsa.UnblindObject(y, r);

            //Console.WriteLine("equal? : " + res.Equals(bytes));
            //string msg2 = Encoding.UTF8.GetString(res);
            //Console.WriteLine("msg: " + msg2);

            //Console.WriteLine("equal? : " + msg1.Equals(msg2));

        }
    }
}
