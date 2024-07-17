
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
        if (Match(TokenType.FOR)) return ForStatement();
        if (Match(TokenType.IF)) return IfStatement();
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Match(TokenType.WHILE)) return WhileStatement();
        if (Match(TokenType.BREAK)) return BreakStatement();
        if (Match(TokenType.CONTINUE)) return ContinueStatement();
        if (Match(TokenType.LEFT_BRACE)) return new Stmt<R>.Block(Block());

        return ExpressionStatement();
    }

    private Stmt<R> ForStatement() {
        Consume(TokenType.LEFT_PAREN, "ERROR: Expect '(' after 'for'.");

        Stmt<R>? initializer;
        if (Match(TokenType.SEMICOLON)) {
            initializer = null;
        } else if (Match(TokenType.VAR)) {
            initializer = VarDeclaration();
        } else {
            initializer = ExpressionStatement();
        }

        Expr<R>? condition = null;
        if (!Check(TokenType.SEMICOLON)) {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "ERROR: Expect ';' after loop condition.");

        Expr<R>? increment = null;
        if (!Check(TokenType.RIGHT_PAREN)) {
            increment = Expression();
        }
        Consume(TokenType.RIGHT_PAREN, "ERROR: Expect ')' after for clauses");

        Stmt<R> body = Statement();

        if (increment != null) {
            body = new Stmt<R>.Block(
                        new List<Stmt<R>>{body, new Stmt<R>.Expression(increment)}
            );
        }

        if (condition == null) condition = new Expr<R>.Literal(true);
        body = new Stmt<R>.While(condition, body);

        if (initializer != null) {
            body = new Stmt<R>.Block(new List<Stmt<R>>{initializer, body});
        }

        return body;
    }

    private Stmt<R> IfStatement() {
        Consume(TokenType.LEFT_PAREN, "ERROR: Expect '(' after 'if'.");
        Expr<R> condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "ERROR: Expect ')' after if condition");

        Stmt<R> thenBranch = Statement();
        Stmt<R>? elseBranch = null;
        if (Match(TokenType.ELSE)) {
            elseBranch = Statement();
        }

        return new Stmt<R>.If(condition, thenBranch, elseBranch);
    }

    private Stmt<R> PrintStatement() {
        Expr<R> value = Expression();
        Consume(TokenType.SEMICOLON, "ERROR: Expect ';' after value.");
        return new Stmt<R>.Print(value);
    }

    private Stmt<R> ExpressionStatement() {
        Expr<R> expr = Expression();
        Consume(TokenType.SEMICOLON, "ERROR: Expect ';' after expression.");
        return new Stmt<R>.Expression(expr);
    }

    private List<Stmt<R>> Block() {
        List<Stmt<R>> statements = new List<Stmt<R>>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "ERROR: Expect '}' after block.");
        return statements;
    }

    private Expr<R> Assignment() {
        Expr<R> expr = Or();

        if (Match(TokenType.EQUAL)) {
            Token equals = Previous();
            Expr<R> value = Assignment();

            if (expr is Expr<R>.Variable) {
                Token name = ((Expr<R>.Variable)expr).Name;
                return new Expr<R>.Assign(name, value);
            }

            Error(equals, "ERROR: Invalid assignment target.");
        }

        return expr;
    }

    private Expr<R> Or() {
        Expr<R> expr = And();

        while(Match(TokenType.OR)) {
            Token oper = Previous();
            Expr<R> right = And();
            expr = new Expr<R>.Logical(expr, oper, right);
        }

        return expr;
    }

    private Expr<R> And() {
        Expr<R> expr = Equality();

        while(Match(TokenType.AND)) {
            Token oper = Previous();
            Expr<R> right = Equality();
            expr = new Expr<R>.Logical(expr, oper, right);
        }

        return expr;
    }

    private Stmt<R> VarDeclaration() {
        Token name = Consume(TokenType.IDENTIFIER, "ERROR: Expect variable name.");

        Expr<R>? initializer = null;
        if (Match(TokenType.EQUAL)) {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "ERROR: Expect ';' after variable declaration");
        return new Stmt<R>.Var(name, initializer);
    }

    private Stmt<R> WhileStatement() {
        Consume(TokenType.LEFT_PAREN, "ERROR: Expect '(' after 'while'.");
        Expr<R> condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "ERROR: Expect ')' after condition.");
        Stmt<R> body = Statement();

        return new Stmt<R>.While(condition, body);
    }

    private Stmt<R> BreakStatement() {
        Token name = Consume(TokenType.SEMICOLON, "ERROR: Expect ';' after expression.");
        return new Stmt<R>.Break(name);
    }

    private Stmt<R> ContinueStatement() {
        Token name = Consume(TokenType.SEMICOLON, "ERROR: Expect ';' after expression.");
        return new Stmt<R>.Continue(name);
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
