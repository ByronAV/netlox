
public class Class(string name, Class? superclass, Dictionary<string, Function> methods) : ICallable
{

    public Function? FindMethod(string name) {
        if (_methods.TryGetValue(name, out Function? value)) {
            return value;
        }

        if (_superclass != null) {
            return _superclass.FindMethod(name);
        }

        return null;
    }
    public override string ToString()
    {
        return _name;
    }

    public override object? Call(Interpreter interpreter, List<object> arguments)
    {
        Instance instance = new(this);
        Function? initializer = FindMethod("init");
        initializer?.Bind(instance).Call(interpreter, arguments);
        return instance;
    }

    public override int Arity()
    {
        Function? initializer = FindMethod("init");
        if (initializer == null) return 0;
        return initializer.Arity();
    }

    public string Name {
        get => _name;
    }

    private readonly string _name = name;
    private readonly Class? _superclass = superclass;
    private readonly Dictionary<string, Function> _methods = methods;
    
}