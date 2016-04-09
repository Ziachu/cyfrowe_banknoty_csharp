using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Digests;

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

        public RsaKeyParameters GetPubKey()
        {
            //RSAParameters kke = rsa.ExportParameters(false);
            //BigInteger mod = new BigInteger(kke.Modulus);
            //BigInteger exp = new BigInteger(kke.Exponent);
            RsaKeyParameters key = new RsaKeyParameters(false, n, e);
            return key;
        }

        public RsaKeyParameters GetPrivKey()
        {
            RsaKeyParameters key = new RsaKeyParameters(true, n, d);
            return key;
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
        public BigInteger BlindOb(RsaKeyParameters key, byte[] message, BigInteger r)
        {
            BigInteger m = new BigInteger(message);
            //BigInteger b = (r.ModPow(e, n).Multiply(m)).Mod(n);
            BigInteger z = ((r.ModPow(e, n).Mod(n)).Multiply(m));
            BigInteger b = z.Mod(n);
            //Console.WriteLine("m: {0}\nr: {1}\ne: {2}\nn: {3}\nb: {4}", m, r, e, n, b);
            return b;
        }

        public BigInteger BlindObject(RsaKeyParameters key, byte[] msg,  BigInteger factor)
        {
            RsaBlindingEngine eng = new RsaBlindingEngine();



            RsaBlindingParameters param = new RsaBlindingParameters(key, factor);
            PssSigner blindSigner = new PssSigner(eng, new Sha1Digest(), 15);
            blindSigner.Init(true, param);

            blindSigner.BlockUpdate(msg, 0, msg.Length);

            byte[] blinded = null;
            try
            {
                blinded = blindSigner.GenerateSignature();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ");
            }
            BigInteger blinded_int = new BigInteger(blinded);
            return blinded_int;
        }

        // --- b => m
        public byte[] UnblindOb(BigInteger y, BigInteger r)
        {
            //BigInteger m = (r.ModPow(e.ModInverse(n), n).Multiply(y)).Mod(n);
            //BigInteger m = (y.Multiply(r.ModPow(e, n))).Mod(n);

            BigInteger ee = e.Negate();

            BigInteger m = (y.Multiply(r.ModPow(ee, n).Mod(n))).Mod(n);
            //BigInteger m = ((r.ModPow(e.ModInverse(n), n)).Multiply(y)).Mod(n);

            //Console.WriteLine("m: {0}\nr: {1}\ne: {2}\nn: {3}\ny: {4}", m, r, e, n, y);
            return m.ToByteArray();
        }

        public byte[] UnblindObject(RsaKeyParameters key, byte[] msg, BigInteger factor)
        {
            RsaBlindingEngine eng = new RsaBlindingEngine();

            RsaBlindingParameters param = new RsaBlindingParameters(key, factor);
            eng.Init(false, param);

            return eng.ProcessBlock(msg, 0, msg.Length);
        }

        //public void CheckEquality(BigInteger m, BigInteger r, BigInteger y)
        //{
        //    //r = new BigInteger("2");
        //    //n = new BigInteger("3");

        //    //Console.WriteLine("\n\nr: " + r);
        //    //var one = new BigInteger("1");
        //    //Console.WriteLine("\n\none: " + one);
        //    //Console.WriteLine("\n\nr^{-1}: " + one.Divide(r));
        //    //Console.WriteLine("\n\nr * r^{-1}: " + r.Multiply(one.Divide(r)));
        //    //Console.WriteLine("\n\n2 * 2 % 3: " + r.Multiply(r).Mod(n));
        //    //Console.WriteLine("\n\n2 ^ 3 % 3: " + r.ModPow(n, n));
        //    //Console.WriteLine("\n\nr / r: " + r.Divide(r));
        //    //Console.WriteLine("\n\nr / r: " + r.Divide(r));
        //    byte[] blind = BlindObject(m.ToByteArray(), r).ToByteArray();
        //    byte[] unblind = UnblindObject(BlindObject(m.ToByteArray(), r), r);
        //    byte[] m_byte = m.ToByteArray();

        //    for(int i =0; i< m_byte.Length; i++)
        //    {
        //        if (m_byte[i] == unblind[i])
        //        {
        //            Console.WriteLine("jupiii");
        //        }
        //        else
        //        {
        //            Console.WriteLine("not equal");
        //        }
        //    }
        //}

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
