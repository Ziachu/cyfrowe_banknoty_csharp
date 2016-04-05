using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyfroweBanknoty.Objects
{
    public class Banknote
    {
        // implement banknote object
        public long amount;
        public long id;

        public List<Series> s_series;
        public List<Series> t_series;
        public List<byte[]> u_hashes;
        public List<byte[]> w_hashes;

        public Banknote() { }

        public Banknote(long amount, long id, List<Series> s_series, List<Series> t_series, List<byte[]> u_hashes, List<byte[]> w_hashes)
        {
            this.amount = amount;
            this.id = id;
            this.s_series = s_series;
            this.t_series = t_series;
            this.u_hashes = u_hashes;
            this.w_hashes = w_hashes;
        }

        public void VisualizeBanknote()
        {
            Console.WriteLine("\n------------------ banknote ------------------");
            Console.WriteLine("--- amount: {0}", amount);
            Console.WriteLine("--- id: {0} ---", id);
            Console.WriteLine("--- contains {0} (4 x {1}) series, each of length: {2} ---", t_series.Count() * 4, t_series.Count(), t_series[0].length);
        }
    }
}
