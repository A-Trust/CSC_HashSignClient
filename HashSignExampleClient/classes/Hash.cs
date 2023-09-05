using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System;

namespace HashSignExampleClient.classes
{
    public class Hash
    {

        public enum HASH_ALG
        {
            SHA1,
            RIPEMD160,
            SHA256,
            SHA384,
            SHA512,
        }

        public static byte[] HashData(byte[] data, HASH_ALG alg)
        {
            IDigest h = null;


            switch (alg)
            {
                case HASH_ALG.SHA256:
                    h = new Sha256Digest();
                    break;
                case HASH_ALG.SHA1:
                    h = new Sha1Digest();
                    break;
                case HASH_ALG.SHA384:
                    h = new Sha384Digest();
                    break;
                case HASH_ALG.SHA512:
                    h = new Sha512Digest();
                    break;
                case HASH_ALG.RIPEMD160:
                    h = new RipeMD256Digest();
                    break;
                default:
                    break;
            }


            byte[] hash = null;

            if (h != null)
            {
                h.BlockUpdate(data, 0, data.Length);

                hash= new byte[h.GetDigestSize()];
                h.DoFinal(hash, 0);
            }

            return hash;
        }


        public static DerObjectIdentifier getHashOID(Hash.HASH_ALG hashAlg)
        {
            DerObjectIdentifier hashOID = null;
            switch (hashAlg)
            {
                case Hash.HASH_ALG.RIPEMD160:
                    hashOID = new DerObjectIdentifier(OID.RIPEMD_160);
                    break;
                case Hash.HASH_ALG.SHA256:
                    hashOID = new DerObjectIdentifier(OID.SHA256);
                    break;
                case Hash.HASH_ALG.SHA384:
                    hashOID = new DerObjectIdentifier(OID.SHA384);
                    break;
                case Hash.HASH_ALG.SHA512:
                    hashOID = new DerObjectIdentifier(OID.SHA512);
                    break;
                case Hash.HASH_ALG.SHA1:
                    hashOID = new DerObjectIdentifier(OID.SHA1);
                    break;
                default:
                    hashOID = null;
                    break;
            }

            return hashOID;
        }

        public static AlgorithmIdentifier getAlgID(Hash.HASH_ALG digestAlg)
        {
            AlgorithmIdentifier algID = null;
            try
            {
                algID = new AlgorithmIdentifier(Hash.getHashOID(digestAlg), DerNull.Instance);
            }
            catch (Exception)
            {
            }

            return algID;
        }
    }
}
