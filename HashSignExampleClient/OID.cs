using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashSignExampleClient
{
    public class OID
    {
        public const string SIGNED_DATA ="1.2.840.113549.1.7.2";

        public const string RIPEMD_160 = "1.3.36.3.2.1";
        public const string SHA1 = "1.3.14.3.2.26";
        public const string SHA256 = "2.16.840.1.101.3.4.2.1";
        public const string SHA384 = "2.16.840.1.101.3.4.2.2";
        public const string SHA512 = "2.16.840.1.101.3.4.2.3";
        public const string SHA224 = "2.16.840.1.101.3.4.2.4";

        public const string RSA_ENCRYPTION = "1.2.840.113549.1.1.1";

        public const string SHA1_WITH_RSA_ENCRYPTION = "1.2.840.113549.1.1.5";
        public const string SHA256_WITH_RSA_ENCRYPTION = "1.2.840.113549.1.1.11";


        public const string ECDSA_WITH_SHA1 = "1.2.840.10045.4.1";
        public const string ECDSA_WITH_SHA256 = "1.2.840.10045.4.3.2";
        public const string ECDSA_WITH_SHA384 = "1.2.840.10045.4.3.3";
        public const string ECDSA_WITH_SHA512 ="1.2.840.10045.4.3.4";
        public const string ECDSA_WITH_RIPEMD160 = "0.4.0.127.0.7.1.1.4.1.6"; // eigentlich ecdsa-plain-RIPEMD160

    }
}
