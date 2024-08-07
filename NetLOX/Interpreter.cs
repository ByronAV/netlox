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
        if (IsTruthy(Evaluate(stmt.Condition))) {
            Execute(stmt.ThenBranch);
        } else if (stmt.ElseBranch != null) {
            Execute(stmt.ElseBranch);
        }
        return null;
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
        while(IsTruthy(Evaluate(stmt.Condition))) {
            _in_loop = true;
            Execute(stmt.Body);
            if (_should_break) break;
            _should_continue = false;
        }
        _in_loop = false;
        _should_break = false;
        return null;
    }

    public object? VisitBreakStmt(Stmt<object>.Break stmt) {
        // We should never reach here because this would come from Execute
        // and we never execute Break Statements
        throw new RunTimeError(stmt.Keyword, "ERROR: Break statement outside of loop");
    }

    public object? VisitContinueStmt(Stmt<object>.Continue stmt) {
        // We should never reach here because this would come from Execute
        // and we never execute Continue Statements
        throw new RunTimeError(stmt.Keyword, "ERROR: Continue statement outside of loop");
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
                                    "ERROR: Operands must be valid addition types (strings or numbers)");
            }
            case TokenType.SLASH: {
                CheckNumberOperands(expr.Operator, left, right);
                // Check if we divide by zero and don't throw exception
                // but return error
                if (Convert.ToInt32(right)== 0) {
                    throw new RunTimeError(expr.Operator, "ERROR: Trying to divide by zero. Abort");
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
        object left = Evaluate(expr.Left);

        if (expr.Operator.Type == TokenType.OR) {
            if (IsTruthy(left)) return left;
        } else {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.Right);
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
            case TokenType.BANG: return !IsTruthy(right);
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

    private bool IsTruthy(object obj) {
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
        throw new RunTimeError(_operator, "ERROR: Operands must be a number.");
    }

    private string Stringify(object obj) {
        // This should never happen because we're
        // throwing error for accessing null values
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
        Environment previous = _environment;
        try {
            _environment = environment;

            foreach (Stmt<object> statement in statements) {
                // We might have a nested break statement
                // so we need to check here before continuing
                if (_should_break) break;
                /* 
                    This is a bit more complicated. When we see the
                    _should_continue inside a nested block we should
                    break the loop of statements BUT if we have reached
                    the outermost statements (the body of the lox loop)
                    we still need to increment the counter of the loop.
                    When we're at the outermost loop the statements will contain
                    two elements, the body of the lox loop and the increment
                    expression 
                    TODO: This might create problems in the case that we have
                    a nested block with 2 statements and the flag is set.
                */
                if (_should_continue && (statements.Count != 2)) break;

                // We check here if we have a break statement
                if ((statement is Stmt<object>.Break) && _in_loop) {
                    _should_break = true;
                    break;
                }
                else if ((statement is Stmt<object>.Continue) && _in_loop) {
                    _should_continue = true;
                    break;
                }
                // These should never happen outside of loops
                else if (statement is Stmt<object>.Break) {
                    throw new RunTimeError(((Stmt<object>.Break)statement).Keyword, "ERROR: Break statement outside of loop");
                } else if (statement is Stmt<object>.Continue) {
                    throw new RunTimeError(((Stmt<object>.Continue)statement).Keyword, "ERROR: Continue statement outside of loop");
                }
                Execute(statement);
            }
        } finally {
            _environment = previous;
        }
    }

    private Environment _environment = new Environment();
    static private bool _in_loop = false;
    static private bool _should_break = false;
    static private bool _should_continue = false;
}