﻿using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using System.Collections.Generic;

namespace CyfroweBanknoty.Tools
{
    public class RSA
    {
        private RSACryptoServiceProvider rsa;
        // random value to blind objects

        private BigInteger n;
        private BigInteger e;
        private BigInteger d;

        public RSA(bool it_is_bank)
        {
            rsa = new RSACryptoServiceProvider();

            if (it_is_bank)
            {
                var privateKey = rsa.ExportParameters(true);

                e = new BigInteger(privateKey.Exponent);
                n = new BigInteger(privateKey.Modulus).Abs();
                d = new BigInteger(privateKey.D);
            }
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
            n = new BigInteger(publicKey.Modulus).Abs();
            d = null;

            DrawR();
        }

        public BigInteger DrawR()
        {
            if (n != null)
            {
                BigInteger gcd = null;
                BigInteger one = new BigInteger("1");
                BigInteger r = null;

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

                return r;
            } else
            {
                Console.WriteLine("[fail] Get public key from Bank first!");
                return null;
            }
        }

        // --- m => b
        public BigInteger BlindObject(byte[] message, BigInteger r)
        {
            BigInteger m = new BigInteger(message);
            BigInteger b = ((r.ModPow(e, n)).Multiply(m)).Mod(n);

            return b;
        }

        // --- b => m
        public byte[] UnblindObject(BigInteger y, BigInteger r)
        {
            BigInteger m = ((r.ModInverse(n)).Multiply(y)).Mod(n);
            return m.ToByteArray();
        }

        // --- b => bs
        public BigInteger SignObject(BigInteger m)
        {
            BigInteger bs = m.ModPow(d, n);
            return bs;
        }

        // --- bs => s
        public BigInteger UnblindSignature(BigInteger bs, BigInteger r)
        {
            BigInteger s = ((r.ModInverse(n)).Multiply(bs)).Mod(n);
            return s;
        }
    }
}
