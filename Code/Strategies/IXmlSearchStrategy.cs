using DormitoryXml.Code.Models;

namespace DormitoryXml.Code.Strategies;

public interface IXmlSearchStrategy
{
    string Name { get; }

    List<Student> Search(string path, SearchCriteria criteria);
}
