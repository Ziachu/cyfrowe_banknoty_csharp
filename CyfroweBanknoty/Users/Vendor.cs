using CyfroweBanknoty.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using CyfroweBanknoty.Tools;

namespace CyfroweBanknoty.Users
{
    public class Vendor
    {
        // -------
        // PART II
        // -------
        // step 10. Vendor verifies signature
        // step 11. Vendor generated random series of bites and sends it over to Alice
        // step 13. Vendor verifies banknote

        // --------
        // PART III
        // --------
        // step 14. If banknote is valid, Vendor sends it over to Bank among the partially revealed Alice ids.
        // [jump to Bank.cs][steps 15.-16.]

        // --------
        // PART IV
        // --------
        //step 17. BUYING - Vendor Generates 100 random bits
        //Step 18. Vendors sends those bits to Alice 
        //Step 19. Alice sends to Vendor 100 bit commitments - which side - it depends of 100 random bits from vendor(1 - left, 0 - right)

        public Tools.RSA rsa;
        private Connection bank_connection;
        private Connection alice_connection;





        //Sprzedawca pobiera z Banku klucz publiczny K=[E,N] do weryfikacji podpisu
        public void getSignatureFromBank()
        {
            if (bank_connection.socket.Connected)
            {
                var public_key_in_bytes = bank_connection.Receive(1);
                var public_key_in_xml = Helper.GetStringFromBytes(public_key_in_bytes);

                //Console.WriteLine("\t[debug]: publick_key_in_xml: " + public_key_in_xml);

                rsa.SetPublicKey(public_key_in_xml);

                Console.WriteLine("[info]: I've got public key from Bank!");
            }
            else
            {
                Console.WriteLine("[fail] Establish connection with Bank first!");
            }
        }
        //sprzedawca otrzymuje od Alice banknot w raz z podpisem[Mj, Sj] i oblicza S' = M^e(mod N) i sprawdza czy podpis zgadza sie z S od Alice
        public void getBanknote()
        {
            //Banknote banknote = new Banknote;
            byte[] amount_byte = null;// tu musimy odebrac od Alice kwote
            long amount = BitConverter.ToInt64(amount_byte, 0);

            byte[] id_byte = null;//odbieramy
            long id = BitConverter.ToInt64(id_byte, 0);

            for (int i = 0; i < 99; i++)
            {
                byte[] s_byte = null;//odbieramy
                byte[] t_byte = null;//odbieramy
                byte[] u_byte = null;//odbieramy
                byte[] w_byte = null;//odbieramy
                //wysylamy

            }
        }
        public bool checkSignature()
        {
            return false;
        }
        //Sprzedawca generuje losowo 100 bitow
        public void generateRandomSeriesAndSendToAlice()
        {
            Random rand = new Random();
            Byte[] series = new Byte[100];
            
            for (int i=0; i<99; i++)
            {
                int random = rand.Next(0, 1);
                series[i] = (byte)random;
            }

        }
        //Sprzedawca dostaje od Alice S, B i L - cząstki lewego zobowiazania bitowego, oblicza H(S, B, L)= U', pobiera z banknotu U, S i weryfikuje
        //Sprzedawca dostaje również od Alice T, C, R - czastki prawego zobowiazania bitowego, oblica H(T, C, R) = W', pobiera z banknotu W oraz T i weryfikuje
        public void verifyBanknote()
        {

        }
        //step 14
        //Sprzedawca wysyla banknot do Banku, poniewaz chce uzyskac uznanie konta owymi pieniedzmi.
        public void sendBanknoteAndGetMoney(Banknote banknote)
        {
            byte[] amount_byte = BitConverter.GetBytes(banknote.amount);
            //wysylamy kwote
            byte[] id_byte = BitConverter.GetBytes(banknote.id);

            for (int i = 0; i < 99; i++)
            {
                byte[] s_byte = banknote.s_series[i].values;
                //wysylamy
                byte[] t_byte = banknote.t_series[i].values;
                //wysylamy
                byte[] u_byte = banknote.u_hashes[i];
                //wysylamy
                byte[] w_byte = banknote.u_hashes[i];
                //wysylamy
            }
        }

        //step 17
        public byte[] generate100RandomBits()
        {

            byte[] bits = new byte[100];
            Random rand = new Random();

            for(int i=0; i<100; i++)
            {
                bits[i] = Helper.GetBytesInteger(rand.Next())[i];
            }

            return bits;

        }

        //step 18
        public void sendRandomBits(byte[] bits)
        {
            alice_connection.Send(1, bits);

        }

        //step 19
        public void getCommitments()
        {

        }

    }
}
