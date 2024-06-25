
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public class Parser<R> {

    private sealed class ParseError : Exception;
    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens) {
        this.tokens = tokens;
    }

    public Expr<R>? Parse() {
        try {
            return Expression();
        } catch(ParseError) {
            return null;
        }
    }

    private Expr<R> Expression() {
        return Equality();
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
            return new Expr<R>.Literal(Previous().literal);
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
        return Peek().type == type;
    }

    private Token Advance() {
        if (!IsAtEnd()) current++;
        return Previous();
    }

    private bool IsAtEnd() {
        return Peek().type == TokenType.EOF;
    }

    private Token Peek() {
        return tokens[current];
    }

    private Token Previous() {
        return tokens[current - 1];
    }

    private ParseError Error(Token token, string message) {
        Lox.Error(token, message);
        return new ParseError();
    }

    private void Synchronize() {
        Advance();

        while (!IsAtEnd()) {
            if (Previous().type == TokenType.SEMICOLON) return;
        }

        switch(Peek().type) {
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
}
