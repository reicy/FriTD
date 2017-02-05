using System.Collections.Generic;

namespace Manager.Settings
{
    public interface IDataStructure
    {
        KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals);
    }
}
