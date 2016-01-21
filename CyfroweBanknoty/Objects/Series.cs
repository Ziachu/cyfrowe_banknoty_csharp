using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyfroweBanknoty.Objects
{
    class Series
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
            values = GenerateRandomValues(length);
        }

        private byte[] GenerateRandomValues(int length)
        {
            // implement!
            throw new NotImplementedException();
        }
    }
}
