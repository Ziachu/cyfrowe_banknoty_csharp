using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyfroweBanknoty.Users;
using CyfroweBanknoty.Objects;

namespace CyfroweBanknoty.AliceProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            // --- step 0.
            // --- --- generating unique Alice identifiers

            Console.WriteLine("[info] Creating Alice user instance.");
            Alice alice = new Alice();

            // --- step 1. (generating 100 banknotes)
            // --- --- hiding unique identifiers behind "l_secret" and "r_secret" series
            // --- --- 
            alice.GenerateBanknotes(123.45, 10);
            alice.banknotes[0].VisualizeBanknote();
            // --- printing Alice ids

            //Console.WriteLine("[info] Alice ids:");
            //foreach(Series s in alice.alice_ids)
            //{
            //    Console.WriteLine(s);
            //}

            // --- reading Alice ids from file (saved to file during creation)

            //Console.WriteLine("\n[info] Alice ids (read from file):");
            //foreach(Series s in alice.transmitter.ReadSeriesFromFile("alice_ids.txt"))
            //{
            //    Console.WriteLine(s);
            //}

            alice.EstablishConnectionWithBank();

            Console.ReadLine();
        }
    }
}
