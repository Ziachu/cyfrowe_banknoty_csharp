using CyfroweBanknoty.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyfroweBanknoty.Users
{
    public class Bank
    {
        // ------
        // PART I
        // ------
        // step 4. Bank picks one banknote and demands disclosure of the rest
        // step 6. Bank verifies 99 banknotes
        // step 7. Bank signs picked banknote
        // step 8. Bank sends signed banknote over to Alice

        // --------
        // PART III
        // --------
        // step 15. Bank verifies signature under banknote
        // step 16. Bank verifies if banknote was not used before

        private RSA rsa;

        public Bank()
        {
            rsa = new RSA();
        }
    }
}
