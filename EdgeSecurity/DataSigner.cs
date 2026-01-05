using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Edge.EdgeSecurity
{
    public class DataSigner
    {
        private readonly string _privateKeyCertificatePath;
        //current implementation has no password on the private key cert,
        //this might change depending on how Edge uses the generation application
        private const string mPrivateKeyCertPassword = "";

        public DataSigner(string privateKeyCertPath)
        {
            _privateKeyCertificatePath = privateKeyCertPath;
        }

        public byte[] GetSignature(byte[] dataToSignAsBytes)
        {
            var privateCert = new X509Certificate2(_privateKeyCertificatePath, mPrivateKeyCertPassword);
            var signingProvider = (RSACryptoServiceProvider) privateCert.PrivateKey;
            return signingProvider.SignData(dataToSignAsBytes, new SHA1CryptoServiceProvider());
        }

        public void SignObject(ISignableObject obj)
        {
            obj.DataSignature = GetSignature(obj.DataForSigning);
        }
    }
}