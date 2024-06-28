public class RunTimeError : Exception {
    public RunTimeError(Token token, string message) : base(message) {
        _token = token;
    }

    public Token Token {
        get => _token;
    }

    private readonly Token _token;
}