using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyfroweBanknoty.Objects
{
    class Banknote
    {
        // implement banknote object
        public float amount;
        public int id;

        public Series[] s_series;
        public Series[] u_series;
        public Series[] t_series;
        public Series[] w_series;

        public bool hidden = false;

        public Banknote(float amount, int id, Series[] s_series, Series[] u_series, Series[] t_series, Series[] w_series)
        {
            this.amount = amount;
            this.id = id;
            this.s_series = s_series;
            this.u_series = u_series;
            this.t_series = t_series;
            this.w_series = w_series;
        }

        public Banknote(float amount, int id, int length)
        {
            this.amount = amount;
            this.id = id;

            s_series = new Series[length];
            u_series = new Series[length];
            t_series = new Series[length];
            w_series = new Series[length];

            for (int i = 0; i < length; i++)
            {
                s_series[i] = new Series(100);
                u_series[i] = new Series(100);
                t_series[i] = new Series(100);
                w_series[i] = new Series(100);
            }
        }
    }
}
