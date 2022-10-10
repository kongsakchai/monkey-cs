using System.Collections.Generic;

namespace Monkey.Core;

public class Environment
{

    private Dictionary<string, IObject> Store;

    public Environment()
    {
        Store = new Dictionary<string, IObject>();
    }

    public (IObject?, bool) Get(string name)
    {
        var ok = Store.TryGetValue(name, out var value);

        if (!ok)
            return (null, false);

        return (value, ok);
    }

    public IObject Set(string name, IObject value)
    {
        Store[name] = value;
        return value;
    }

}