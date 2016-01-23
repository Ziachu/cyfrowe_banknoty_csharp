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

        private void GenerateRandomValues(int length)
        {
            Random random = new Random();
            random.NextBytes(values);
        }

        public override string ToString()
        {
            //string representation;

            //foreach (byte b in values)
            //{
            //    representation += 
            //}

            return BitConverter.ToString(values);
        }
    }
}
