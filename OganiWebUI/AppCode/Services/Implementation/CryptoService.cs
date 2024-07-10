using Microsoft.Extensions.Options;
using OganiWebUI.Models.Configurations;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OganiWebUI.AppCode.Services.Implementation
{
    public class CryptoService :  ICryptoService
    {
        private CryptoServiceConfiguration options;

        public CryptoService(IOptions<CryptoServiceConfiguration> options) { 
        this.options=options.Value;
                }
        public string Decrypt(string chiperText)
        {
            using (var symProvider = new TripleDESCryptoServiceProvider())
            using (var md5 = MD5.Create())
            {
                byte[] valueBuffer = Convert.FromBase64String(chiperText);
                byte[] keyBuffer = md5.ComputeHash(Encoding.ASCII.GetBytes($"{options.Key}202$"));
                byte[] ivBuffer = md5.ComputeHash(Encoding.ASCII.GetBytes($"202$_#$@{options.Key}"));
                var transformer = symProvider.CreateDecryptor(keyBuffer, ivBuffer);
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, transformer, CryptoStreamMode.Write))
                {
                    cs.Write(valueBuffer, 0, valueBuffer.Length);
                    cs.FlushFinalBlock();
                    ms.Position = 0;
                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = new byte[ms.Length];
                    ms.Read(bytes, 0, bytes.Length);
                    string pureText = Encoding.UTF8.GetString(bytes);
                    return pureText;
                }


            }
        }

        public string Encrypt(string value,bool appliedUrlEncode=false)
        {
            using(var symProvider=new TripleDESCryptoServiceProvider()) 
            using(var md5=MD5.Create())
            {
                byte[] valueBuffer=Encoding.UTF8.GetBytes(value);
                byte[] keyBuffer = md5.ComputeHash(Encoding.UTF8.GetBytes($"{options.Key}202$"));
                byte[] ivBuffer = md5.ComputeHash(Encoding.UTF8.GetBytes($"202$_#$@{options.Key}"));
                var transformer=symProvider.CreateEncryptor(keyBuffer, ivBuffer);
                using(var ms=new MemoryStream())
                using (var cs = new CryptoStream(ms, transformer,CryptoStreamMode.Write)) { 
                    cs.Write(valueBuffer,0,valueBuffer.Length);
                    cs.FlushFinalBlock();
                    ms.Position = 0;
                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = new byte[ms.Length];
                    ms.Read(bytes,0, bytes.Length);
                    string chiperText=Convert.ToBase64String(bytes);
                    if (appliedUrlEncode)
                    {
                        chiperText = HttpUtility.UrlEncode(chiperText);
                    }
                    return chiperText;
                }
                 

            }
            //using(var provider=new TripleDESCryptoServiceProvider())
            //using(var md5 = MD5.Create())
            //{
            //    try
            //    {
            //        var keyBuffer=md5.ComputeHash(Encoding.UTF8.GetBytes($"#{options.Key}!2024"));
            //        var ivBuffer = md5.ComputeHash(Encoding.UTF8.GetBytes($"2024${options.Key}$"));
            //        var transform =provider.CreateEncryptor(keyBuffer, ivBuffer);
            //        using (var ms=new MemoryStream())
            //        using (var cs=new CryptoStream(ms,transform,CryptoStreamMode.Write))
            //        {
            //            var textBuffer = Encoding.UTF8.GetBytes(value);
            //            cs.Write(textBuffer,0, textBuffer.Length);
            //            cs.FlushFinalBlock();
            //            ms.Position = 0;
            //            var result = new byte[ms.Length];
            //            ms.Read(result,0,result.Length);
            //            var chiperText=Convert.ToBase64String(result);
            //            if (appliedUrlEncode)
            //                return HttpUtility.UrlEncode(chiperText);

            //            return chiperText;
            //        }
            //    }
            //    catch
            //    {
            //        return "";
            //    }
            //}
        }
    }
}
