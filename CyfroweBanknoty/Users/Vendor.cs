using CyfroweBanknoty.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

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

        public Tools.RSA rsa;
        //Sprzedawca pobiera z Banku klucz publiczny K=[E,N] do weryfikacji podpisu
        public void getSignatureFromBank()
        {

        }
        //sprzedawca otrzymuje od Alice banknot w raz z podpisem[Mj, Sj] i oblicza S' = M^e(mod N) i sprawdza czy podpis zgadza sie z S od Alice
        public void getBanknote()
        {
            //Banknote banknote = new Banknote;
            byte[] amount_byte = null;// tu musimy odebrac od Alice kwote
            int amount = BitConverter.ToInt32(amount_byte, 0);

            byte[] id_byte = null;//odbieramy
            int id = BitConverter.ToInt32(id_byte, 0);

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
    }
}
