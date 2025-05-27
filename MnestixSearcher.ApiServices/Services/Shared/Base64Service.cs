using MnestixSearcher.ApiServices.Contracts;
using System.Text;

namespace MnestixSearcher.ApiServices.Services.Shared
{
    public class Base64Service : IBase64Service
    {
        public string Base64Encode(string input)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytesToEncode);
        }

        public string Base64Decode(string input)
        {
            byte[] bytesToDecode = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytesToDecode);
        }
    }
}
