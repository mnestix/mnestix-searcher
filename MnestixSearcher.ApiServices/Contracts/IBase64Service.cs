using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface IBase64Service
    {
        public string Base64Encode(string input);
        public string Base64Decode(string input);
    }
}
