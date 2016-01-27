using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyfroweBanknoty.Objects
{
    public class HiddenBanknote
    {
        public BigInteger amount;
        public BigInteger id;

        public List<BigInteger> s_series;
        public List<BigInteger> t_series;
        public List<BigInteger> u_hashes;
        public List<BigInteger> w_hashes;

        public HiddenBanknote()
        {
            s_series = new List<BigInteger>();
            t_series = new List<BigInteger>();
            u_hashes = new List<BigInteger>();
            w_hashes = new List<BigInteger>();
        }

        public HiddenBanknote(BigInteger amount, BigInteger id, List<BigInteger> s_series, List<BigInteger> t_series, List<BigInteger> u_hashes, List<BigInteger> w_hashes)
        {
            this.amount = amount;
            this.id = id;
            this.s_series = s_series;
            this.t_series = t_series;
            this.u_hashes = u_hashes;
            this.w_hashes = w_hashes;
        }

        public void VisualizeHiddenBanknote()
        {
            Console.WriteLine("\n------------------ hidden banknote ------------------");
            Console.WriteLine("--- amount: {0}", amount);
            Console.WriteLine("--- id: {0} ---", id);
            Console.WriteLine("--- contains {0} (4 x {1}) series, each of length: {2} ---", t_series.Count() * 4, t_series.Count(), t_series[0].BitCount);
        }
    }
}
