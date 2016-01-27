using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using CyfroweBanknoty.Tools;
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

        public void Send(Connection con)
        {
            if (con.socket.Connected)
            {
                con.Send(1, amount.ToByteArray());
                con.Receive(1);

                con.Send(1, id.ToByteArray());
                con.Receive(1);

                con.Send(1, BitConverter.GetBytes(s_series.Count()));
                con.Receive(1);

                for (int i = 0; i < s_series.Count(); i++)
                {
                    con.Send(1, s_series[i].ToByteArray());
                    con.Receive(1);

                    con.Send(1, t_series[i].ToByteArray());
                    con.Receive(1);

                    con.Send(1, u_hashes[i].ToByteArray());
                    con.Receive(1);

                    con.Send(1, w_hashes[i].ToByteArray());
                    con.Receive(1);
                }

                Console.WriteLine("\t[debug]: Banknote sent.");
            }
        }

        public void Receive(Connection con)
        {
            if (con.handler.Connected)
            {
                var result = con.Receive(0);
                amount = new BigInteger(result);
                con.Send(0, new byte[1]);

                result = con.Receive(0);
                id = new BigInteger(result);
                con.Send(0, new byte[1]);

                var no_series = BitConverter.ToInt32(con.Receive(0), 0);
                con.Send(0, new byte[1]);

                for (int i = 0; i < no_series; i++)
                {
                    result = con.Receive(0);
                    con.Send(0, new byte[1]);
                    s_series.Add(new BigInteger(result));

                    result = con.Receive(0);
                    con.Send(0, new byte[1]);
                    t_series.Add(new BigInteger(result));

                    result = con.Receive(0);
                    con.Send(0, new byte[1]);
                    u_hashes.Add(new BigInteger(result));

                    result = con.Receive(0);
                    con.Send(0, new byte[1]);
                    w_hashes.Add(new BigInteger(result));
                }

                Console.WriteLine("\t[debug]: Banknote received.");
            }
        }
    }
}
