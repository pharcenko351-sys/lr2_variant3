using DormitoryXml.Code.Models;

namespace DormitoryXml.Code.Strategies;

public class XmlSearchContext
{
    private IXmlSearchStrategy? _strategy;

    public void SetStrategy(IXmlSearchStrategy strategy) => _strategy = strategy;

    public List<Student> ExecuteSearch(string path, SearchCriteria criteria)
    {
        if (_strategy is null)
            throw new InvalidOperationException("Не обрано спосіб аналізу XML.");
        return _strategy.Search(path, criteria);
    }
}
