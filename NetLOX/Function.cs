
public class Function : ICallable {

    public Function(string name, Expr<object>.Function declaration, Environment closure, bool isInitializer) {
        _name = name;
        _declaration = declaration;
        _closure = closure;
        _isInitializer = isInitializer;
    }
    public Function Bind(Instance instance) {
        Environment environment = new(_closure);
        environment.Define("this", instance);
        return new(_name, _declaration, environment, _isInitializer);
    }

    public override int Arity()
    {
        return _declaration.Parameters.Count;
    }

    public override object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(_closure);
        for (int i = 0; i < _declaration.Parameters.Count; ++i) {
            environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
        }

        try {
            interpreter.ExecuteBlock(_declaration.Body, environment);
        } catch (Return return_value) {
            if (_isInitializer) return _closure.GetAt(0, "this");
            return return_value.Value;
        }

        if (_isInitializer) return _closure.GetAt(0, "this");
        return null;
    }

    public override string ToString()
    {
        return "<fn " + _name + ">";
    }

    private readonly string _name;
    private readonly Expr<object>.Function _declaration;
    private readonly Environment _closure;
    private readonly bool _isInitializer;
}