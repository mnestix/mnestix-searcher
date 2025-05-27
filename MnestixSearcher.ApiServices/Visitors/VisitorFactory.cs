using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Contracts;

namespace MnestixSearcher.ApiServices.Visitors
{
    public class VisitorFactory : IVisitorFactory
    {
        public Visitor Create(AasSearchEntry record)
        {
            return new Visitor(record);
        }
    }
}
