
class Function : ICallable {

    public Function(string name, Expr<object>.Function declaration, Environment closure) {
        _name = name;
        _declaration = declaration;
        _closure = closure;
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
            return return_value.Value;
        }
        return null;
    }

    public override string ToString()
    {
        return "<fn " + _name + ">";
    }

    private readonly string _name;
    private readonly Expr<object>.Function _declaration;
    private readonly Environment _closure;
}