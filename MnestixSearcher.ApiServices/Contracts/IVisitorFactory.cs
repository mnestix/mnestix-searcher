using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Visitors;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface IVisitorFactory
    {
        Visitor Create(AasSearchEntry record);
    }
}
