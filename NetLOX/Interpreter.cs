using System.Data;

// Stmt should have type Void here but I'm not sure how
// to convert Void to object. So we're sticking
// with object for now.
public class Interpreter : Expr<object>.IVisitor, Stmt<object>.IVisitor {

    public void Interpet(List<Stmt<object>> statements) {
        try {
            foreach (Stmt<object> statement in statements) {
                Execute(statement);
            }
        } catch (RunTimeError error) {
            Lox.RuntimeError(error);
        }
    }

    public object? VisitBlockStmt(Stmt<object>.Block stmt) {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
        return null;
    }

    public object? VisitClassStmt(Stmt<object>.Class stmt) {
        throw new NotImplementedException();
    }

    public object? VisitExpressionStmt(Stmt<object>.Expression stmt) {
        // In order for the REPL to work we need to be able to evaluate
        // expression and then print the result in the env
        if (System.Environment.GetCommandLineArgs().Length == 1) {
            Console.WriteLine(Stringify(Evaluate(stmt.Expresion)));
            return null;
        }
        Evaluate(stmt.Expresion);
        return null;
    }

    public object? VisitFunctionStmt(Stmt<object>.Function stmt) {
        throw new NotImplementedException();
    }

    public object? VisitIfStmt(Stmt<object>.If stmt) {
        throw new NotImplementedException();
    }

    public object? VisitPrintStmt(Stmt<object>.Print stmt) {
        object value = Evaluate(stmt.Expresion);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object? VisitReturnStmt(Stmt<object>.Return stmt) {
        throw new NotImplementedException();
    }

    public object? VisitVarStmt(Stmt<object>.Var stmt) {
        object? value = null;
        if (stmt.Initializer != null) {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme, value);
        return null;
    }

    public object? VisitWhileStmt(Stmt<object>.While stmt) {
        throw new NotImplementedException();
    }

    public object VisitAssignExpr(Expr<object>.Assign expr) {
        object value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);

        return value;
    }

    public object VisitBinaryExpr(Expr<object>.Binary expr) {
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);

        switch(expr.Operator.Type) {
            case TokenType.GREATER: {
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            }
            case TokenType.GREATER_EQUAL: {
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            }
            case TokenType.LESS: {
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            }
            case TokenType.LESS_EQUAL: {
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            }
            case TokenType.MINUS: {
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            }
            case TokenType.PLUS: {
                if (left is double && right is double) {
                    return (double)left + (double)right;
                }

                if (left is string && right is string) {
                    return (string)left + (string)right;
                }
                // If either operand is string, convert the other
                // operand to string and concatenate
                else if (left is string) {
                    return (string)left + right.ToString();
                } else {
                    return left.ToString() + (string)right;
                }

                throw new RunTimeError(expr.Operator, 
                                    "Operands must be valid addition types (strings or numbers)");
            }
            case TokenType.SLASH: {
                CheckNumberOperands(expr.Operator, left, right);
                // Check if we divide by zero and don't throw exception
                // but return error
                if (Convert.ToInt32(right)== 0) {
                    throw new RunTimeError(expr.Operator, "Trying to divide by zero. Abort");
                }
                return (double)left / (double)right;
            }
            case TokenType.STAR: {
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            }
            case TokenType.BANG_EQUAL: return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
        }

        // Unreachable
        return null;
    }

    public object VisitCallExpr(Expr<object>.Call expr) {
        throw new NotImplementedException();
    }

    public object VisitGetExpr(Expr<object>.Get expr) {
        throw new NotImplementedException();
    }

    public object VisitGroupingExpr(Expr<object>.Grouping expr) {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Expr<object>.Literal expr) {
        return expr.Value;
    }

    public object VisitLogicalExpr(Expr<object>.Logical expr) {
        throw new NotImplementedException();
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
        object right = Evaluate(expr.Right);

        switch(expr.Operator.Type) {
            case TokenType.BANG: return !IsThruthy(right);
            case TokenType.MINUS: return -(double)right;
        }

        // Unreachable
        return null;
    }

    public object VisitVariableExpr(Expr<object>.Variable expr) {
        return _environment.Get(expr.Name);
    }

    private object Evaluate(Expr<object> expr) {
        return expr.Accept(this);
    }

    private bool IsThruthy(object obj) {
        if (obj == null) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }

    private bool IsEqual(object a, object b) {
        if (a == null && b == null) return true;
        if (a == null) return false;
        return a.Equals(b);
    }

    private void CheckNumberOperands(Token _operator, object left, object right) {
        if (left is double && right is double) return;
        throw new RunTimeError(_operator, "Operands must be a number.");
    }

    private string Stringify(object obj) {
        if (obj == null) return "nil";

        if (obj is double) {
            string text = obj.ToString();
            if (text.EndsWith(".0")) {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return obj.ToString();
    }

    private void Execute(Stmt<object> stmt) {
        stmt.Accept(this);
    }

    private void ExecuteBlock(List<Stmt<object>> statements, Environment environment) {
        Environment previous = this._environment;
        try {
            this._environment = environment;

            foreach (Stmt<object> statement in statements) {
                Execute(statement);
            }
        } finally {
            this._environment = previous;
        }
    }

    private Environment _environment = new Environment();
}