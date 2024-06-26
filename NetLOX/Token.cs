
public class Token {
    public Token(TokenType? type, string lexeme, object? literal, int line) {
        _type = type;
        _lexeme = lexeme;
        _literal = literal;
        _line = line;
    }

    public override string ToString() {
        return _type + " " + _lexeme + " " + _literal;
    }

    public TokenType? Type {
        get => _type;
    }

    public string Lexeme {
        get => _lexeme;
    }

    public object? Literal {
        get => _literal;
    }

    public int Line {
        get => _line;
    }

    private readonly TokenType? _type;
    private readonly string _lexeme;
    private readonly object? _literal;
    private readonly int _line;

}