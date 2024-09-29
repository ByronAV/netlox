public abstract class ICallable {
    abstract public int Arity();
    abstract public object? Call(Interpreter interpreter, List<object> arguments);
}

public class Clock : ICallable {
    override public int Arity() { return 0; }

    override public object? Call(Interpreter interpreter, List<object> arguments) {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
    }

    override public string ToString()
    {
        return "<native fn>";
    }
}
