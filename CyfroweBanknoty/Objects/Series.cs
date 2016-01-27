using CyfroweBanknoty.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyfroweBanknoty.Objects
{
    public class Series
    {
        // implement series object (part of banknote object)
        public int length;
        public byte[] values;

        public Series(int length, byte[] values)
        {
            this.length = length;
            this.values = values;
        }

        // generate random series of given length
        public Series(int length)
        {
            this.length = length;
            values = new byte[length];
            GenerateRandomValues(length);
        }

        public Series() { }

        private void GenerateRandomValues(int length)
        {
            Random random = new Random();
            random.NextBytes(values);
        }

        public void Send(Connection con)
        {
            if (con.socket.Connected)
            {
                con.Send(1, BitConverter.GetBytes(length));
                con.Receive(1);

                con.Send(1, values);
                con.Receive(1);
            }
        }

        public void Receive(Connection con)
        {
            if (con.handler.Connected)
            {
                length = BitConverter.ToInt32(con.Receive(0), 0);
                con.Send(0, new byte[1]);

                values = con.Receive(0);
                con.Send(0, new byte[1]);
            }
        }

        public override string ToString()
        {
            return BitConverter.ToString(values);
        }
    }
}
