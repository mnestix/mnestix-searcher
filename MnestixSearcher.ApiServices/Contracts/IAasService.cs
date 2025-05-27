using AasCore.Aas3_0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface IAasService
    {
        public Task<ICollection<AssetAdministrationShell>> GetAssetAdministrationShellsAsync(int? limit = 10_000_000);
    }
}
