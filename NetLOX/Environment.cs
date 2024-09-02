public class Environment {

    public Environment() {
        _enclosing = null;
    }

    public Environment(Environment enclosing) {
        _enclosing = enclosing;
    }

    public void Assign(Token name, object value) {
        if (_values.ContainsKey(name.Lexeme)) {
            _values[name.Lexeme] = value;
            return;
        }

        if (_enclosing != null) {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RunTimeError(name,
            "ERROR: Undefined variable '" + name.Lexeme + "'.");
    }

    public void AssignAt(int? distance, Token name, object value) {
        Ancestor(distance)?._values.Add(name.Lexeme, value);
    }

    public void Define(string name, object value) {
        _values.Add(name, value);
    }

    public object Get(Token name) {
        if (_values.ContainsKey(name.Lexeme)){
            if (_values[name.Lexeme] == null) {
                throw new RunTimeError(name,
                "ERROR: Accessing variable '" + name.Lexeme + "' whose value is `nil`.");
            }
            return _values[name.Lexeme];
        }

        if (_enclosing != null) return _enclosing.Get(name);

        throw new RunTimeError(name,
                "ERROR: Undefined variable '" + name.Lexeme + "'.");
    }

    public object GetAt(int? distance, string name) {
        return Ancestor(distance)?._values[name];
    }

    public Environment? Ancestor(int? distance) {
        Environment? environment = this;
        for (int i = 0; i < distance; ++i) {
            environment = environment?._enclosing;
        }

        return environment;
    }

    private readonly Environment? _enclosing;
    private readonly Dictionary<string,object> _values = new Dictionary<string, object>();
}