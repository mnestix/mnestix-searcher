using MnestixSearcher.AasSearcher;

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
