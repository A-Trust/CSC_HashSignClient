using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using HashSignExampleClient.classes;


namespace HashSignExampleClient
{
    class CmsSigBuilder
    {
        private ContentInfo encapContentInfo = null;
        private HashSet<SignerIdentifier> signerIdSet = new HashSet<SignerIdentifier>();
        private HashSet<AlgorithmIdentifier> digestAlgSet = new HashSet<AlgorithmIdentifier>();
        private List<SignerInfo> signerInfoSet = new List<SignerInfo>();
        private HashSet<Asn1Object> certSet = new HashSet<Asn1Object>();
        private HashSet<Asn1Object> crlSet = new HashSet<Asn1Object>();


        public enum KEY_TYPE
        {
            unknown,
            RSA,
            ECDSA,
        }

        public bool buildOK
        {
            get;
            protected set;
        }

        private string sid = "";

        public CmsSigBuilder(string sid, string dataB64, bool isDetached, DerObjectIdentifier contentType = null)
        {
            this.sid = sid;
            try
            {
                byte[] data = Convert.FromBase64String(dataB64);
                buildOK = buildEncapContentInfo(data, isDetached, contentType);
            }
            catch (Exception)
            { }
        }

        public CmsSigBuilder(string sid, byte[] data, bool isDetached, DerObjectIdentifier contentType = null)
        {
            this.sid = sid;
            buildOK = buildEncapContentInfo(data, isDetached, contentType);
        }


        private bool buildEncapContentInfo(byte[] data, bool isDetached, DerObjectIdentifier contentType = null)
        {
            var ct = contentType;
            try
            {
                if (null == ct)
                {
                    ct = new DerObjectIdentifier("1.2.840.113549.1.7.1");
                }

                DerOctetString dataDer = null;
                if (!isDetached)
                {
                    if (data != null && data.Length > 0)
                    {
                        dataDer = new DerOctetString(data);
                    }
                    else
                    {
                        return false;
                    }
                }

                encapContentInfo = new ContentInfo(ct, dataDer);
                return true;

            }
            catch (Exception)
            {
                return false;
            }


        }

        public byte[] GetCms()
        {
            if (!buildOK)
                return null;

            var digestAlgorithms = new DerSet(new Asn1EncodableVector(digestAlgSet.ToArray()));

            DerSet certs = null;
            if (certSet.Count > 0)
            {
                certs = new DerSet(new Asn1EncodableVector(certSet.ToArray()));
            }

            DerSet crls = null;
            if (crlSet.Count > 0)
            {
                crls = new DerSet(new Asn1EncodableVector(crlSet.ToArray()));
            }


            var signers = new DerSet(new Asn1EncodableVector(signerInfoSet.ToArray()));


            var signedData = new SignedData(digestAlgorithms, encapContentInfo, certs, crls, signers);
            var signedDataOID = new DerObjectIdentifier(OID.SIGNED_DATA);

            var cmsDer = new ContentInfo(signedDataOID, signedData);

            return cmsDer.GetDerEncoded();
        }


        public bool AddSigner(byte[] sigCertBytes, Hash.HASH_ALG digestAlg, byte[] signedAttributes, byte[] signature, byte[] unsignedAttributes)
        {
            bool bOK = false;
            try
            {
                DerSet signedAttributesDer = null;
                DerSet unsignedAttributesDer = null;
                // Signed Attributes are optional
                if (signedAttributes != null)
                {
                    // Convert signed attributes to set
                    signedAttributes[0] = 0x31;
                    var inStream = new Asn1InputStream(signedAttributes);
                    signedAttributesDer = inStream.ReadObject() as DerSet;
                }

                // Unsigned Attributes are optional
                if (unsignedAttributes != null)
                {
                    // Convert unsigned attributes to set
                    unsignedAttributes[0] = 0x31;
                    var inStream = new Asn1InputStream(unsignedAttributes);
                    unsignedAttributesDer = inStream.ReadObject() as DerSet;
                }

                var stream = new Asn1InputStream(sigCertBytes);
                var certObj = stream.ReadObject();

                X509Certificate sigCert;
                bOK = getCertFromBytes(sigCertBytes, out sigCert);


                SignerIdentifier sigID = null;
                if (bOK)
                {
                    // Build signer ID
                    var issuerDn = sigCert.IssuerDN;
                    var serNum = sigCert.SerialNumber;
                    var issuerAndSerialNumber = new IssuerAndSerialNumber(issuerDn, serNum);
                    sigID = new SignerIdentifier(issuerAndSerialNumber);
                }

                AlgorithmIdentifier digestAlgOID;
                bOK = getDigestAlg(digestAlg, out digestAlgOID);


                // Determine key type from public key
                KEY_TYPE keyType = KEY_TYPE.unknown;
                if (sigCert != null)
                {
                    var sigAlgOid = sigCert.SigAlgOid;
                    var sigAlg = sigCert.SigAlgName;

                    var pubKey = sigCert.GetPublicKey();
                    SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey);

                    AlgorithmIdentifier algID = publicKeyInfo.AlgorithmID;
                    DerObjectIdentifier algOid = algID.Algorithm;

                    if (algOid.Equals(X9ObjectIdentifiers.IdECPublicKey))
                    {
                        keyType = KEY_TYPE.ECDSA;
                    }
                    else
                    {
                        keyType = KEY_TYPE.RSA;
                    }

                }


                AlgorithmIdentifier sigAlgOID = null;
                if (bOK)
                {
                    sigAlgOID = new AlgorithmIdentifier(getSigAlgorithmOID(digestAlg, keyType), DerNull.Instance);
                    bOK = sigAlgOID != null;
                }

                DerOctetString signatureDer = null;
                if (bOK)
                {
                    bOK = getDerSignature(keyType, signature, out signatureDer);
                }

                if (bOK)
                {
                    var signerInfo = new SignerInfo(sigID, digestAlgOID, signedAttributesDer, sigAlgOID, signatureDer, unsignedAttributesDer);

                    certSet.Add(certObj);
                    signerIdSet.Add(sigID);
                    digestAlgSet.Add(digestAlgOID);
                    signerInfoSet.Add(signerInfo);
                }
            }
            catch (Exception)
            {
                bOK = false;
            }

            if (!bOK)
            {
                buildOK = false;
            }

            return bOK;
        }

        public static byte[] GetITUinteger(byte[] bytesToFormat)
        {
            int firstNonZeroByte = bytesToFormat.Length;
            bool needExtraZero = false;
            for (int i = 0; i < bytesToFormat.Length; i++)
            {
                if (bytesToFormat[i] != 0)
                {
                    firstNonZeroByte = i;
                    // If the first byte is bigger than 7F then we need to add an extra zero so the integer is not seen as negative
                    if (bytesToFormat[i] > 0x7F)
                    {
                        needExtraZero = true;
                    }

                    break;
                }
            }

            List<byte> byteList = new List<byte>();
            // If all bytes are zero we will need to add back a zero so we don't get a null array 
            if (needExtraZero || firstNonZeroByte == bytesToFormat.Length)
            {
                byteList.Add(0x0);
            }

            for (int i = firstNonZeroByte; i < bytesToFormat.Length; i++)
            {
                byteList.Add(bytesToFormat[i]);
            }

            return byteList.ToArray();

        }

        public static bool getDerSignature(KEY_TYPE keyType, byte[] signatureBytes, out DerOctetString signatureDer)
        {
            bool bOK = true;
            signatureDer = null;

            if (keyType == KEY_TYPE.RSA)
            {
                signatureDer = new DerOctetString(signatureBytes);
            }
            else if (keyType == KEY_TYPE.ECDSA)
            {

                // Split the array
                int len = signatureBytes.Length / 2;
                byte[] leftBytes = (new ArraySegment<byte>(signatureBytes, 0, len)).ToArray();
                byte[] rightBytes = (new ArraySegment<byte>(signatureBytes, len, len)).ToArray();

                // Format each byte array as an integer according to ITU-T X.690
                byte[] leftIntBytes = GetITUinteger(leftBytes);
                byte[] rightIntBytes = GetITUinteger(rightBytes);

                DerInteger r = new DerInteger(leftIntBytes);
                DerInteger s = new DerInteger(rightIntBytes);

                var ecdsaSigVec = new Asn1EncodableVector();
                ecdsaSigVec.Add(r);
                ecdsaSigVec.Add(s);

                var signatureSeq = new DerSequence(ecdsaSigVec);

                signatureDer = new DerOctetString(signatureSeq.GetDerEncoded());

            }

            return bOK;
        }

        public bool AddCertificate(byte[] certBytes)
        {
            bool bOK = true;

            try
            {
                X509Certificate cert;
                if (!getCertFromBytes(certBytes, out cert))
                {
                    bOK = false;
                }
                else
                {
                    var stream = new Asn1InputStream(certBytes);
                    var certObj = stream.ReadObject();
                    certSet.Add(certObj);
                }
            }
            catch (Exception)
            {
                bOK = buildOK = false;

            }

            return bOK;

        }
        public static bool getCertFromBytes(byte[] certBytes, out X509Certificate cert)
        {
            bool bOK = false;
            cert = null;
            try
            {
                var parser = new X509CertificateParser();
                cert = parser.ReadCertificate(certBytes);

                bOK = cert != null;
            }
            catch (Exception)
            {
            }

            return bOK;
        }

        public bool getDigestAlg(Hash.HASH_ALG digestAlg, out AlgorithmIdentifier digestAlgOID)
        {
            bool bOK = false;
            digestAlgOID = null;
            try
            {

                digestAlgOID = new AlgorithmIdentifier(Hash.getHashOID(digestAlg), DerNull.Instance);
                bOK = digestAlgOID != null;
            }
            catch (Exception)
            {
            }

            return bOK;
        }

        private DerObjectIdentifier getSigAlgorithmOID(Hash.HASH_ALG digestAlg, KEY_TYPE keyType)
        {
            switch (keyType)
            {
                case KEY_TYPE.RSA:
                    return new DerObjectIdentifier(OID.RSA_ENCRYPTION);

                case KEY_TYPE.ECDSA:
                    switch (digestAlg)
                    {
                        case Hash.HASH_ALG.SHA1:
                            return new DerObjectIdentifier(OID.ECDSA_WITH_SHA1);
                        case Hash.HASH_ALG.SHA256:
                            return new DerObjectIdentifier(OID.ECDSA_WITH_SHA256);
                        case Hash.HASH_ALG.SHA384:
                            return new DerObjectIdentifier(OID.ECDSA_WITH_SHA384);
                        case Hash.HASH_ALG.SHA512:
                            return new DerObjectIdentifier(OID.ECDSA_WITH_SHA512);
                        case Hash.HASH_ALG.RIPEMD160:
                            return new DerObjectIdentifier(OID.ECDSA_WITH_RIPEMD160);
                    }
                    break;
            }

            return null;

        }
    }


}
