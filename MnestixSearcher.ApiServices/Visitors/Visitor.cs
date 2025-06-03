using AasCore.Aas3_0;
using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Types;
using AasVisitation = AasCore.Aas3_0.Visitation;

namespace MnestixSearcher.ApiServices.Visitors
{

    public class Visitor : AasVisitation.VisitorThrough
    {
        private readonly AasSearchEntry _record;
        private readonly List<string> _idShorts = [];

        public Visitor(AasSearchEntry record)
        {
            _record = record;
        }

        public override void Visit(IClass that)
        {
            if(that is ISubmodelElement)
            {
                var element = (IReferable)that;
                _idShorts.Add(element.IdShort);
            }
            base.Visit(that);
            if (that is ISubmodelElement)
            {
                _idShorts.RemoveAt(_idShorts.Count - 1);
            }
        }

        public override void VisitSubmodelElementCollection(ISubmodelElementCollection that)
        {
            var matchingKey = that.SemanticId?.Keys?.FirstOrDefault(key =>
                (key.Type == KeyTypes.GlobalReference || key.Type == KeyTypes.ConceptDescription) &&
                SemanticGroups.AllSmElCollections.Any(id => key.Value.Contains(id, StringComparison.OrdinalIgnoreCase))
            );

            bool saveSmElCollection = matchingKey != null;

            if (saveSmElCollection == true && that.Value != null)
            {
                _record.SaveData = true;

                var prodClassifications = new Dictionary<string, object>();

                var classVal = new ProductClassificationValues();

                foreach (var item in that.Value.OfType<Property>()) {

                    var semanticReferences = item.SemanticId;

                    var semanticId = semanticReferences?.Keys.FirstOrDefault(key =>
                        (key.Type == KeyTypes.GlobalReference || key.Type == KeyTypes.ConceptDescription) &&
                        SemanticGroups.AllClassificationProperties.Any(id => key.Value.Contains(id, StringComparison.OrdinalIgnoreCase))
                    );

                    if (semanticId == null)
                        continue;

                    AssignIfMatch(SemanticGroups.ProductClassificationSystem, semanticId.Value, v => classVal.System = item.Value);
                    AssignIfMatch(SemanticGroups.ProductClassId, semanticId.Value, v => classVal.ProductId = item.Value.Trim());
                    AssignIfMatch(SemanticGroups.ClassificationSystemVersion, semanticId.Value, v => classVal.Version = item.Value);
                }

                if (!string.IsNullOrWhiteSpace(classVal.System))
                    _record.ProductClassifications.Add(classVal);
            }

            base.VisitSubmodelElementCollection(that);
        }

        public override void VisitMultiLanguageProperty(IMultiLanguageProperty that)
        {

            var matchingKey = that.SemanticId?.Keys?.FirstOrDefault(key =>
                (key.Type == KeyTypes.GlobalReference || key.Type == KeyTypes.ConceptDescription) &&
                SemanticGroups.AllMlProperties.Any(id => key.Value.Contains(id, StringComparison.OrdinalIgnoreCase))
            );

            bool saveProperty = matchingKey != null;

            if (saveProperty == true && that.Value != null)
            {
                _record.SaveData = true;

                var propertyData = new PropertyData
                {
                    IdShortPath = string.Join('.', _idShorts),
                    SemanticId = matchingKey.Value,
                    MLValues = [],
                };

                foreach (var value in that.Value)
                {
                    propertyData.MLValues.Add(new MLValue
                    {
                        Language = value.Language,
                        Text = value.Text,
                    });
                }

                AssignIfMatch(SemanticGroups.ManufacturerName, matchingKey.Value, v => _record.ManufacturerName = propertyData);
                AssignIfMatch(SemanticGroups.ProductRoot, matchingKey.Value, v => _record.ProductRoot = propertyData);
                AssignIfMatch(SemanticGroups.ProductFamily, matchingKey.Value, v => _record.ProductFamily = propertyData);
                AssignIfMatch(SemanticGroups.ProductDesignation, matchingKey.Value, v => _record.ProductDesignation = propertyData);
            }

            base.VisitMultiLanguageProperty(that);
        }

        private static void AssignIfMatch(string[] group, string value, Action<string> assignAction)
        {
            if (group.Any(id => value.Contains(id, StringComparison.OrdinalIgnoreCase)))
            {
                assignAction(value);
            }
        }
    }
}
