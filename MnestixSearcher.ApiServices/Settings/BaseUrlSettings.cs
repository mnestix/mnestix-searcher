using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnestixSearcher.ApiServices.Settings
{
    public class BaseUrlSettings
    {
        public required string AasRepositoryBaseUrl { get; set; }
        public required string SubmodelRepositoryBaseUrl { get; set; }
        public required string ConceptDescriptionRepositoryBaseUrl { get; set; }
    }
}
