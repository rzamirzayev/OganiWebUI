namespace OganiWebUI.AppCode.Services
{
    public interface ICryptoService
    {
        string Encrypt(string value,bool appliedUrlEncode=false);
        string Decrypt(string chiperText);   
    }
}
