
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Collections;

public class Parser<R> {

    public Parser(List<Token> tokens) {
        _tokens = tokens;
    }

    public List<Stmt<R>> Parse() {
        List<Stmt<R>> statements = new List<Stmt<R>>();
        while (!IsAtEnd()) {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Expr<R> Expression() {
        return Assignment();
    }

    private Stmt<R>? Declaration() {
        try {
            if (Match(TokenType.VAR)) return VarDeclaration();
            return Statement();
        } catch (ParseError error) {
            Synchronize();
            return null;
        }
    }

    private Stmt<R> Statement() {
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Match(TokenType.LEFT_BRACE)) return new Stmt<R>.Block(Block());

        return ExpressionStatement();
    }

    private Stmt<R> PrintStatement() {
        Expr<R> value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt<R>.Print(value);
    }

    private Stmt<R> ExpressionStatement() {
        Expr<R> expr = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new Stmt<R>.Expression(expr);
    }

    private List<Stmt<R>> Block() {
        List<Stmt<R>> statements = new List<Stmt<R>>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private Expr<R> Assignment() {
        Expr<R> expr = Equality();

        if (Match(TokenType.EQUAL)) {
            Token equals = Previous();
            Expr<R> value = Assignment();

            if (expr is Expr<R>.Variable) {
                Token name = ((Expr<R>.Variable)expr).Name;
                return new Expr<R>.Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Stmt<R> VarDeclaration() {
        Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr<R>? initializer = null;
        if (Match(TokenType.EQUAL)) {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");
        return new Stmt<R>.Var(name, initializer);
    }

    private Expr<R> Equality() {
        Expr<R> expr = Comparison();

        while(Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
            Token _operator = Previous();
            Expr<R> right = Comparison();
            expr = new Expr<R>.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr<R> Comparison() {
        Expr<R> expr = Term();

        while(Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
            Token _operator = Previous();
            Expr<R> right = Term();
            expr = new Expr<R>.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr<R> Term() {
        Expr<R> expr = Factor();

        while(Match(TokenType.MINUS, TokenType.PLUS)) {
            Token _operator = Previous();
            Expr<R> right = Factor();
            expr = new Expr<R>.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr<R> Factor() {
        Expr<R> expr = Unary();

        while(Match(TokenType.SLASH, TokenType.STAR)) {
            Token _operator = Previous();
            Expr<R> right = Unary();
            expr = new Expr<R>.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr<R> Unary() {
        if (Match(TokenType.BANG, TokenType.MINUS)) {
            Token _operator = Previous();
            Expr<R> right = Unary();
            return new Expr<R>.Unary(_operator, right);
        }

        return Primary();
    }

    private Expr<R> Primary() {
        if (Match(TokenType.FALSE)) return new Expr<R>.Literal(false);
        if (Match(TokenType.TRUE)) return new Expr<R>.Literal(true);
        if(Match(TokenType.NIL)) return new Expr<R>.Literal(null);

        if (Match(TokenType.NUMBER, TokenType.STRING)) {
            return new Expr<R>.Literal(Previous().Literal);
        }

        if (Match(TokenType.IDENTIFIER)) {
            return new Expr<R>.Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN)) {
            Expr<R> expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
            return new Expr<R>.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression");
    }

    private bool Match(params TokenType[] types) {
        foreach(TokenType type in types) {
            if (Check(type)) {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Consume(TokenType type, string message) {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private bool Check(TokenType type) {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance() {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd() {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek() {
        return _tokens[_current];
    }

    private Token Previous() {
        return _tokens[_current - 1];
    }

    private ParseError Error(Token token, string message) {
        Lox.Error(token, message);
        return new ParseError();
    }

    private void Synchronize() {
        Advance();

        while (!IsAtEnd()) {
            if (Previous().Type == TokenType.SEMICOLON) return;
        }

        switch(Peek().Type) {
            case TokenType.CLASS:
            case TokenType.FUN:
            case TokenType.VAR:
            case TokenType.FOR:
            case TokenType.IF:
            case TokenType.WHILE:
            case TokenType.PRINT:
            case TokenType.RETURN:
                return;
        }

        Advance();
    }

    private sealed class ParseError : Exception;
    private readonly List<Token> _tokens;
    private int _current = 0;
}
