using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public class Resolver(Interpreter interpreter) : Expr<object>.IVisitor, Stmt<object>.IVisitor {

    private class Variable(Token name, Variable.VariableState state)
    {
        public enum VariableState {
            DECLARED,
            DEFINED,
            READ
        }

        public VariableState State {
            get => _state;
            set => _state = value;
        }

        public Token Name {
            get => _name;
        }

        private readonly Token _name = name;
        private VariableState _state = state;
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

    private void ResolveLocal(Expr<object> expr, Token name, bool isRead) {
        for(int i = _scopes.Count - 1; i >= 0; --i) {
            if(_scopes.ElementAt(i).ContainsKey(name.Lexeme)) {
                _interpreter.Resolve(expr, _scopes.Count - 1 - i);

                // Mark it used.
                if (isRead) {
                    _scopes.ElementAt(i)[name.Lexeme].State = Variable.VariableState.READ;
                }
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
        _scopes.Push([]);
    }

    private void EndScope() {
        Dictionary<string, Variable> scope = _scopes.Pop();

        foreach(var (_, value) in scope) {
            if (value.State == Variable.VariableState.DEFINED) {
                Lox.Error(value.Name, "Local variable defined but not used.");
            }
        }
    }

    private void Declare(Token name) {
        if (_scopes.Count == 0) return;

        Dictionary<string, Variable> scope = _scopes.Peek();
        if (scope.ContainsKey(name.Lexeme)) {
            Lox.Error(name, "ERROR: Already a variable with this name in scope.");
        }
        scope.Add(name.Lexeme, new Variable(name, Variable.VariableState.DECLARED));
    }

    private void Define(Token name) {
        if (_scopes.Count == 0) return;
        _scopes.Peek()[name.Lexeme].State = Variable.VariableState.DEFINED;
    }

    public object? VisitBlockStmt(Stmt<object>.Block stmt) {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }

    public object? VisitClassStmt(Stmt<object>.Class stmt) {
        ClassType enclosingClass = _currentClass;
        _currentClass = ClassType.CLASS;

        Declare(stmt.Name);
        Define(stmt.Name);

        if (stmt.Superclass != null &&
            stmt.Name.Lexeme.Equals(stmt.Superclass.Name.Lexeme)) {
            Lox.Error(stmt.Superclass.Name,
                "ERROR: A class can't inherit from itself.");
        } else if (stmt.Superclass != null) {
            _currentClass = ClassType.SUBCLASS;
            Resolve(stmt.Superclass);
        }

        if (stmt.Superclass != null) {
            BeginScope();
            _scopes.Peek().Add("super", new Variable(new Token(TokenType.SUPER, "super", null, -1),
                                    Variable.VariableState.READ));
        }

        BeginScope();
        // Q: Is the below correct ?? 
        // A: Should be since we don't care about using `this`,
        // in order to throw a warning.
        _scopes.Peek().Add("this", new Variable(new Token(TokenType.THIS, "this", null, -1),
                                    Variable.VariableState.READ));

        foreach (Stmt<object>.Function method in stmt.Methods) {
            FunctionType declaration = FunctionType.METHOD;
            if (method.Name.Lexeme.Equals("init")) {
                declaration = FunctionType.INITIALIZER;
            }
            ResolveFunction(method, declaration);
        }

        EndScope();

        if (stmt.Superclass != null) EndScope();

        _currentClass = enclosingClass;
        return null;
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
            if (_currentFunction == FunctionType.INITIALIZER) {
                Lox.Error(stmt.Keyword,
                        "Can't return a value from an initializer.");
            }
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
        ResolveLocal(expr, expr.Name, false);
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
        Resolve(expr.Object);
        return null;
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
        Resolve(expr.Value);
        Resolve(expr.Object);

        return null;
    }

    public object VisitSuperExpr(Expr<object>.Super expr) {
        if (_currentClass == ClassType.NONE) {
            Lox.Error(expr.Keyword,
                    "ERROR: Can't use 'super' outside of class");
        } else if (_currentClass != ClassType.SUBCLASS) {
            Lox.Error(expr.Keyword,
                    "ERROR: Can't use 'super' in a class with no superclass.");
        }
        ResolveLocal(expr, expr.Keyword, true);
        return null;
    }

    public object VisitThisExpr(Expr<object>.This expr) {
        if (_currentClass == ClassType.NONE) {
            Lox.Error(expr.Keyword,
                "ERROR: Can't use 'this' outside of a class.");
                return null;
        }
        ResolveLocal(expr, expr.Keyword, true);
        return null;
    }

    public object VisitUnaryExpr(Expr<object>.Unary expr) {
        Resolve(expr.Right);
        return null;
    }

    public object VisitVariableExpr(Expr<object>.Variable expr) {
        if ((_scopes.Count != 0) &&
            _scopes.Peek().ContainsKey(expr.Name.Lexeme) &&
            _scopes.Peek()[expr.Name.Lexeme].State == Variable.VariableState.DECLARED) {
                Lox.Error(expr.Name, "ERROR: Can't read local variable in its own initializer.");
            }

        ResolveLocal(expr, expr.Name, true);
        return null;
    }

    private enum FunctionType {
        NONE,
        FUNCTION,
        INITIALIZER,
        METHOD
    }

    private enum ClassType {
        NONE,
        CLASS,
        SUBCLASS
    }

    private readonly Interpreter _interpreter = interpreter;
    private readonly Stack<Dictionary<string, Variable>> _scopes = new();

    private FunctionType _currentFunction = FunctionType.NONE;
    private ClassType _currentClass = ClassType.NONE;
}