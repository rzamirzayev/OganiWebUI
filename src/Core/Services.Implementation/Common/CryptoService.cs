using Domain.Configurations;
using Microsoft.Extensions.Options;
using Services.Common;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Services.Implementation.Common
{
    public class CryptoService : ICryptoService, IDisposable
    {
        private readonly CryptoServiceConfiguration options;
        private readonly HashAlgorithm ha;
        private readonly SymmetricAlgorithm csp;
        private bool disposed = false;


        public CryptoService(IOptions<CryptoServiceConfiguration> options)
        {
            this.options = options.Value;
            ha = MD5.Create();
            csp = TripleDES.Create();

            var keyBytes = ha.ComputeHash(Encoding.UTF8.GetBytes($"{this.options.Key}202$"));
            var newKeyBytes = new byte[24];
            Array.Copy(keyBytes, newKeyBytes, Math.Min(keyBytes.Length, newKeyBytes.Length));
            csp.Key = newKeyBytes;
            csp.Mode = CipherMode.ECB;
            csp.Padding = PaddingMode.PKCS7;
        }
        public string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = csp.CreateDecryptor().TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public string Encrypt(string value, bool appliedUrlEncode = false)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            var cipherBytes = csp.CreateEncryptor().TransformFinalBlock(valueBytes, 0, valueBytes.Length);
            var cipherText = Convert.ToBase64String(cipherBytes);
            if (appliedUrlEncode)
            {
                cipherText = HttpUtility.UrlEncode(cipherText);
            }
            return cipherText;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                ha.Dispose();
                csp.Dispose();
            }
            disposed = true;
        }
    }
}