
class Function : ICallable {

    public Function(Stmt<object>.Function declaration) {
        _declaration = declaration;
    }

    public override int Arity()
    {
        return _declaration.Params.Count;
    }

    public override object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(interpreter.Globals);
        for (int i = 0; i < _declaration.Params.Count; ++i) {
            environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
        }

        interpreter.ExecuteBlock(_declaration.Body, environment);
        return null;
    }

    public override string ToString()
    {
        return "<fn " + _declaration.Name.Lexeme + ">";
    }

    private readonly Stmt<object>.Function _declaration;
}