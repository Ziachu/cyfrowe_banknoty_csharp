using System;
using System.Security.Cryptography;

namespace CyfroweBanknoty.Tools
{
    public class RSA
    {
        private RSACryptoServiceProvider rsa;

        public RSA()
        {
            rsa = new RSACryptoServiceProvider();
        }

        //metoda szyfrująca
        public byte[] Encrypt(string plain_text)
        {
            byte[] encryption;

            encryption = rsa.Encrypt(Helper.GetBytes(plain_text), false);
            return encryption;
        }

        //metoda deszyfrująca
        public byte[] Decrypt(byte[] cryptogram)
        {
            byte[] plain_text;

            plain_text = rsa.Decrypt(cryptogram, false);
            return plain_text;
        }

        //metoda zwracająca klucz publiczny
        public string GetPublicKey()
        {
            return rsa.ToXmlString(false);
        }

        //metoda zwracająca klucz prywatny
        public string GetPrivateKey()
        {
            return rsa.ToXmlString(true);
        }

        public void SetPublicKey(string rsa_params)
        {
            rsa.FromXmlString(rsa_params);
        }
    }
}
