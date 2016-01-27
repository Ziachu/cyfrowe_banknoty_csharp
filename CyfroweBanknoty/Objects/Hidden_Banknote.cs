using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyfroweBanknoty.Objects
{
    class Hidden_Banknote
    {
        public BigInteger amount;
        public BigInteger id;

        public List<BigInteger> s_series;
        public List<BigInteger> t_series;
        public List<BigInteger> u_hashes;
        public List<BigInteger> w_hashes;


        public Hidden_Banknote(BigInteger amount, BigInteger id, List<BigInteger> s_series, List<BigInteger> t_series, List<BigInteger> u_hashes, List<BigInteger> w_hashes)
        {
            this.amount = amount;
            this.id = id;
            this.s_series = s_series;
            this.t_series = t_series;
            this.u_hashes = u_hashes;
            this.w_hashes = w_hashes;
        }
    }
}
