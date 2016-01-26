using CyfroweBanknoty.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //Sprzedawca pobiera z Banku klucz publiczny K=[E,N] do weryfikacji podpisu
        public void getSignatureFromBank()
        {

        }
        //sprzedawca otrzymuje od Alice banknot w raz z podpisem[Mj, Sj] i oblicza S' = M^e(mod N) i sprawdza czy podpis zgadza sie z S od Alice
        public void getBanknoteAndCheckSignature()
        {

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
        public void sendBanknoteAndGetMoney()
        {

        }
    }
}
