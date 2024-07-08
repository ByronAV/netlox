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
            "Undefined variable '" + name.Lexeme + "'.");
    }

    public void Define(string name, object value) {
        _values.Add(name, value);
    }

    public object Get(Token name) {
        if (_values.ContainsKey(name.Lexeme)){
            if (_values[name.Lexeme] == null) {
                throw new RunTimeError(name,
                "Error: Accessing variable '" + name.Lexeme + "' whose value is `nil`.");
            }
            return _values[name.Lexeme];
        }

        if (_enclosing != null) return _enclosing.Get(name);

        throw new RunTimeError(name,
                "Undefined variable '" + name.Lexeme + "'.");
    }

    private readonly Environment? _enclosing;
    private readonly Dictionary<string,object> _values = new Dictionary<string, object>();
}