public class Instance(Class klass)
{
    public override string ToString()
    {
        return _klass.Name + " instance";
    }

    public object Get(Token name) {
        if (_fields.TryGetValue(name.Lexeme, out object? value)) {
            return value;
        }

        Function? method = _klass.FindMethod(name.Lexeme);
        if (method != null) return method.Bind(this);

        throw new RunTimeError(name,
                        "ERROR: Undefined property '" + name.Lexeme + "'.");
    }

    public void Set(Token name, object? value) {
        _fields[name.Lexeme] = value ?? "";
    }

    private Class _klass = klass;
    private readonly Dictionary<string, object> _fields = [];
}