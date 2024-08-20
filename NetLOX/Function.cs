
class Function : ICallable {

    public Function(Stmt<object>.Function declaration, Environment closure) {
        _declaration = declaration;
        _closure = closure;
    }

    public override int Arity()
    {
        return _declaration.Params.Count;
    }

    public override object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(_closure);
        for (int i = 0; i < _declaration.Params.Count; ++i) {
            environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
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
        return "<fn " + _declaration.Name.Lexeme + ">";
    }

    private readonly Stmt<object>.Function _declaration;
    private readonly Environment _closure;
}