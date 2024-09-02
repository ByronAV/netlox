using System.Runtime.CompilerServices;

public class Resolver : Expr<object>.IVisitor, Stmt<object>.IVisitor {

    public Resolver(Interpreter interpreter) {
        _interpreter = interpreter;
    }

    public void Resolve(List<Stmt<object>> statements) {
        foreach(Stmt<object> statement in statements) {
            Resolve(statement);
        }
    }

    private void Resolve(Stmt<object> stmt) {
        stmt.Accept(this);
    }

    private void Resolve(Expr<object> expr) {
        expr.Accept(this);
    }

    private void ResolveLocal(Expr<object> expr, Token name) {
        for(int i = scopes.Count - 1; i >= 0; --i) {
            if(scopes.ElementAt(i).ContainsKey(name.Lexeme)) {
                _interpreter.Resolve(expr, scopes.Count - 1 - i);
                return;
            }
        }
    }

    private void ResolveFunction(Stmt<object>.Function function, FunctionType type) {
        FunctionType enclosingFunction = _currentFunction;
        _currentFunction = type;

        BeginScope();
        foreach(Token param in function.Function_.Parameters) {
           Declare(param);
           Define(param); 
        }
        Resolve(function.Function_.Body);
        EndScope();

        _currentFunction = enclosingFunction;
    }

    private void BeginScope() {
        scopes.Push([]);
    }

    private void EndScope() {
        scopes.Pop();
    }

    private void Declare(Token name) {
        if (scopes.Count == 0) return;

        Dictionary<string, bool> scope = scopes.Peek();
        if (scope.ContainsKey(name.Lexeme)) {
            Lox.Error(name, "ERROR: Already a variable with this name in scope.");
        }
        scope.Add(name.Lexeme, false);
    }

    private void Define(Token name) {
        if (scopes.Count == 0) return;
        scopes.Peek().Add(name.Lexeme, true);
    }

    public object? VisitBlockStmt(Stmt<object>.Block stmt) {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }

    public object? VisitClassStmt(Stmt<object>.Class stmt) {
        throw new NotImplementedException();
    }

    public object? VisitExpressionStmt(Stmt<object>.Expression stmt) {
        Resolve(stmt.Expresion);
        return null;
    }

    public object? VisitFunctionStmt(Stmt<object>.Function stmt) {
        Declare(stmt.Name);
        Define(stmt.Name);

        ResolveFunction(stmt, FunctionType.FUNCTION);
        return null;
    }

    public object? VisitIfStmt(Stmt<object>.If stmt) {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);
        if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
        return null; 
    }

    public object? VisitPrintStmt(Stmt<object>.Print stmt) {
        Resolve(stmt.Expresion);
        return null;
    }

    public object? VisitReturnStmt(Stmt<object>.Return stmt) {
        if (_currentFunction == FunctionType.NONE) {
            Lox.Error(stmt.Keyword, "ERROR: Can't return from top-level code.");
        }

        if (stmt.Value != null) {
            Resolve(stmt.Value);
        }
        return null;
    }

    public object? VisitVarStmt(Stmt<object>.Var stmt) {
        Declare(stmt.Name);
        if (stmt.Initializer != null) {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);
        return null;
    }

    public object? VisitWhileStmt(Stmt<object>.While stmt) {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return null;
    }

    public object? VisitBreakStmt(Stmt<object>.Break stmt) {
        throw new NotImplementedException();
    }

    public object? VisitContinueStmt(Stmt<object>.Continue stmt) {
        throw new NotImplementedException();
    }

    public object VisitAssignExpr(Expr<object>.Assign expr) {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object VisitBinaryExpr(Expr<object>.Binary expr) {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object VisitCallExpr(Expr<object>.Call expr) {
        Resolve(expr.Callee);

        foreach(Expr<object> argument in expr.Arguments) {
            Resolve(argument);
        }
        return null;
    }

    public object VisitFunctionExpr(Expr<object>.Function expr) {
        throw new NotImplementedException();
    }

    public object VisitGetExpr(Expr<object>.Get expr) {
        throw new NotImplementedException();
    }

    public object VisitGroupingExpr(Expr<object>.Grouping expr) {
        Resolve(expr.Expression);
        return null;
    }

    public object VisitLiteralExpr(Expr<object>.Literal expr) {
        return null;
    }

    public object VisitLogicalExpr(Expr<object>.Logical expr) {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object VisitSetExpr(Expr<object>.Set expr) {
        throw new NotImplementedException();
    }

    public object VisitSuperExpr(Expr<object>.Super expr) {
        throw new NotImplementedException();
    }

    public object VisitThisExpr(Expr<object>.This expr) {
        throw new NotImplementedException();
    }

    public object VisitUnaryExpr(Expr<object>.Unary expr) {
        Resolve(expr.Right);
        return null;
    }

    public object VisitVariableExpr(Expr<object>.Variable expr) {
        if ((scopes.Count != 0) &&
            scopes.Peek()[expr.Name.Lexeme] == false) {
                Lox.Error(expr.Name, "ERROR: Can't read local variable in its own initializer.");
            }

        ResolveLocal(expr, expr.Name);
        return null;
    }

    private enum FunctionType {
        NONE,
        FUNCTION
    }

    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> scopes = new();

    private FunctionType _currentFunction = FunctionType.NONE;
}