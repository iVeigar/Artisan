using System.Collections.Generic;

namespace ThiriaExpertSolver.Thiria;

public class ConditionWrapper(ThiriaSession sess)
{
    private readonly ThiriaSession session = sess;

    private Dictionary<string, int> Cache
    { 
        get
        {
            if (field != null)
                return field;

            var map = new Dictionary<string, int>();
            var count = GetNumElements();
            for (var i = 0; i < count; i++)
            {
                map[GetShortName(i)] = i;
            }

            field = map;
            return map;
        }
    }

    public int GetNumElements()
    {
        return session.exports.condition_getNumElements();
    }

    public string GetShortName(int index)
    {
        return session.PtrToString(session.exports.condition_getShortName(index));
    }

    public int? GetIndex(string conditionName)
    {
        return Cache.TryGetValue(conditionName, out var index) ? index : null;
    }
}
