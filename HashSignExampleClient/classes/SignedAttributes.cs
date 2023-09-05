using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HashSignExampleClient.classes.Hash;

namespace HashSignExampleClient.classes
{
    internal class CmsSignedAttributes
    {
        public Hash.HASH_ALG hashAlgo = Hash.HASH_ALG.SHA256;

        public CmsSignedAttributes()
        {
        }

        public byte[] GetPAdES(byte[] tbsContent, string certB64, bool isContentHashed)
        {
            return GetPAdES(tbsContent, certB64, isContentHashed, DateTime.UtcNow);
        }
        public byte[] GetPAdES(byte[] tbsContent, string certB64, bool isContentHashed, DateTime signingTime)
        {
            try
            {
                Asn1EncodableVector signedAttributesVec = getCommonAttributes(tbsContent, certB64, isContentHashed, signingTime);
                var set = new DerSet(signedAttributesVec);
                return set.GetDerEncoded();
            }
            catch (Exception ex)
            {
                //error
            }

            return null;
        }
        private Asn1EncodableVector getCommonAttributes(byte[] tbsContent, string certB64, bool isContentHashed)
        {
            return getCommonAttributes(tbsContent, certB64, isContentHashed, DateTime.UtcNow);
        }


        private Asn1EncodableVector getCommonAttributes(byte[] tbsContent, string certB64, bool isContentHashed, DateTime signingTime)
        {
            try
            {
                Asn1EncodableVector signedAttributesVec = GetDefaultSignedAttributes(tbsContent, isContentHashed);

                var signCertAttr = GetSigningCertAttribute(certB64, hashAlgo, true);
                signedAttributesVec.Add(signCertAttr);

                var signingTimeAttribute = new Org.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.SigningTime,
                    new DerSet(new Org.BouncyCastle.Asn1.Cms.Time(signingTime)));

                return signedAttributesVec;
            }
            catch (Exception ex)
            {
                //error
            }

            return null;
        }
        private Asn1EncodableVector GetDefaultSignedAttributes(byte[] tbsContent, bool isHashed)
        {

            DerOctetString tbsHash;
            // If content not hashed, then hash it...
            if (isHashed)
            {
                tbsHash = new DerOctetString(tbsContent);
            }
            else
            {
                tbsHash = GetHashedContent(tbsContent);
            }

            // Add mandatory attributes:
            Asn1EncodableVector signedAttributes = new Asn1EncodableVector();

            signedAttributes.Add(new Org.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.ContentType, new DerSet(CmsObjectIdentifiers.Data)));
            signedAttributes.Add(new Org.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.MessageDigest, new DerSet(tbsHash)));

            return signedAttributes;

            // signingTime and mimeType attributes are to be added if and only if not PAdES
            //signedAttributes.add(new Attribute(CMSAttributes.signingTime, new DERSet(new DERUTCTime(signingDate))));


            //Here's how to convert back to a byte array...
            //AttributeTable signedAttributesTable = new AttributeTable(signedAttributes);
            //return signedAttributesTable.ToAttributes().GetDerEncoded();

        }
        private DerOctetString GetHashedContent(byte[] tbsContent)
        {
            byte[] hash = Hash.HashData(tbsContent, hashAlgo);
            DerOctetString hashDer = new DerOctetString(hash);

            return hashDer;
        }
        private Org.BouncyCastle.Asn1.Cms.Attribute GetSigningCertAttribute(string certB64, Hash.HASH_ALG hashAlg, bool bIncludeIssuerSerial)
        {
            // Hash the signing certificate
            byte[] certBytes = Convert.FromBase64String(certB64);
            byte[] certHash = Hash.HashData(certBytes, hashAlg);
            var hashDer = new DerOctetString(certHash);

            Org.BouncyCastle.Asn1.Cms.Attribute signingCertAttribute = null;

            Asn1EncodableVector signingCertVec = new Asn1EncodableVector();

            if (hashAlg != Hash.HASH_ALG.SHA1 && hashAlg != Hash.HASH_ALG.SHA256)
            {
                AlgorithmIdentifier hashAlgId = Hash.getAlgID(hashAlg);
                signingCertVec.Add(hashAlgId);
            }

            signingCertVec.Add(hashDer);

            if (bIncludeIssuerSerial)
            {
                Asn1Sequence issuerSerial;
                if (getIssuerSerial(certBytes, out issuerSerial))
                {
                    signingCertVec.Add(issuerSerial);
                }
                else
                {
                    //error
                }
            }


            DerObjectIdentifier signingCertificateOID = null;

            // There are two different ways of encoding the signing certificate depending on whether the hash algorithm is SHA-1 or not
            if (hashAlg == Hash.HASH_ALG.SHA1)
            {
                // id-aa-signingCertificate OBJECT IDENTIFIER ::= { iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs9(9) smime(16) id-aa(2) 12 }
                signingCertificateOID = new DerObjectIdentifier("1.2.840.113549.1.9.16.2.12");
                signingCertAttribute = new Org.BouncyCastle.Asn1.Cms.Attribute(signingCertificateOID, new DerSet(hashDer));
            }
            else
            {
                // id-aa-signingCertificateV2 OBJECT IDENTIFIER ::= { iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs9(9) smime(16) id-aa(2) 47 }
                signingCertificateOID = new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47");

                // Add the cert hash and issuer serial
                var issuerSerialSeq = new DerSequence(signingCertVec);
                var certsSeq = new DerSequence(issuerSerialSeq);
                var sigCertsSeq = new DerSequence(certsSeq);
                signingCertAttribute = new Org.BouncyCastle.Asn1.Cms.Attribute(signingCertificateOID, new DerSet(sigCertsSeq));
            }

            return signingCertAttribute;
        }
        private bool getIssuerSerial(byte[] certBytes, out Asn1Sequence issuerSerial)
        {
            issuerSerial = null;
            X509Certificate cert;
            if (!CmsSigBuilder.getCertFromBytes(certBytes, out cert))
            {
                return false;
            }

            var issuer = cert.CertificateStructure.Issuer;
            var serial = cert.SerialNumber;

            DerInteger serialASN1 = new DerInteger(serial);

            DerTaggedObject issuerDer = new DerTaggedObject(4, issuer);

            var issuerSeq = new DerSequence(issuerDer);

            Asn1EncodableVector issuerSerialVec = new Asn1EncodableVector();
            issuerSerialVec.Add(issuerSeq);
            issuerSerialVec.Add(serialASN1);

            issuerSerial = new DerSequence(issuerSerialVec);

            return true;
        }
    }
}
