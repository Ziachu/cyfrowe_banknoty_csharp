using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;

namespace CyfroweBanknoty.Tools
{
    public class RSA
    {
        private RSACryptoServiceProvider rsa;
        // random value to blind objects
        private BigInteger r;

        private BigInteger n;
        private BigInteger e;
        private BigInteger d;

        public RSA()
        {
            rsa = new RSACryptoServiceProvider();

            // w przypadku Alice można zignorować, metoda SetPublicKey()
            // i tak nadpisze te wartości
            var privateKey = rsa.ExportParameters(true);

            e = new BigInteger(privateKey.Exponent);
            n = new BigInteger(privateKey.Modulus);
            d = new BigInteger(privateKey.D);
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

            var publicKey = rsa.ExportParameters(false);

            e = new BigInteger(publicKey.Exponent);
            n = new BigInteger(publicKey.Modulus);
            d = null;

            DrawR();
        }

        private void DrawR()
        {
            BigInteger gcd = null;
            BigInteger one = new BigInteger("1");

            SecureRandom random = new SecureRandom();
            byte[] randomBytes = new byte[10];

            // --- verify that gcd(r,n) = 1 && r < n && r > 1
            do
            {
                random.NextBytes(randomBytes);
                r = new BigInteger(1, randomBytes);
                gcd = r.Gcd(n);
            }
            while (!gcd.Equals(one) || r.CompareTo(n) >= 0 || r.CompareTo(one) <= 0);
        }

        // --- m => b
        public BigInteger BlindObject(byte[] message)
        {
            BigInteger m = new BigInteger(message);
            BigInteger b = ((r.ModPow(e, n)).Multiply(m)).Mod(n);

            return b;
        }

        // --- b => bs
        public BigInteger SignObject(BigInteger m)
        {
            BigInteger bs = m.ModPow(d, n);
            return bs;
        }

        // --- bs => s
        public BigInteger UnblindSignature(BigInteger bs)
        {
            BigInteger s = ((r.ModInverse(n)).Multiply(bs)).Mod(n);
            return s;
        }
    }
}
