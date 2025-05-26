using MnestixSearcher.AasSearcher;

namespace MnestixSearcher.ApiServices.Visitors
{
    public interface IVisitorFactory
    {
        Visitor Create(AasSearchEntry record);
    }
}
