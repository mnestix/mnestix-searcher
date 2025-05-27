using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnestixSearcher.ApiServices.Dto
{
    public class SearchCriteria
    {
        public string? ProductRoot { get; set; }
        public string? ProductFamily { get; set; }
        public string? ProductDesignation { get; set; }
        public Dictionary<string, string>? Classification { get; set; }
    }
}
