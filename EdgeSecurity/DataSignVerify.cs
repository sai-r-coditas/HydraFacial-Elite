using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;

namespace Edge.EdgeSecurity
{
    public class DataSignVerify
    {
        private readonly string _publicKeyCertificatePath;

        public DataSignVerify([NotNull] string publicKeyCertPath)
        {
            _publicKeyCertificatePath = publicKeyCertPath;
        }

        public bool CheckSignature(byte[] data, byte[] signature)
        {
            var publicCert = new X509Certificate2(_publicKeyCertificatePath);
            var verifyingProvider = (RSACryptoServiceProvider) publicCert.PublicKey.Key;
            return verifyingProvider.VerifyData(data, new SHA1CryptoServiceProvider(), signature);
        }
    }
}