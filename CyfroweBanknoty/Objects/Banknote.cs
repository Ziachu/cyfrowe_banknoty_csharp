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
        public double amount;
        public int id;

        public Series[] s_series;
        public Series[] u_series;
        public Series[] t_series;
        public Series[] w_series;

        public bool hidden = false;

        public Banknote(double amount, int id, Series[] s_series, Series[] u_series, Series[] t_series, Series[] w_series)
        {
            this.amount = amount;
            this.id = id;
            this.s_series = s_series;
            this.u_series = u_series;
            this.t_series = t_series;
            this.w_series = w_series;
        }

        public Banknote(double amount, int id, int no_id_series)
        {
            this.amount = amount;
            this.id = id;

            s_series = new Series[no_id_series];
            u_series = new Series[no_id_series];
            t_series = new Series[no_id_series];
            w_series = new Series[no_id_series];

            for (int i = 0; i < no_id_series; i++)
            {
                s_series[i] = new Series(100);
                u_series[i] = new Series(100);
                t_series[i] = new Series(100);
                w_series[i] = new Series(100);
            }
        }
    }
}
