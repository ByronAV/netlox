using System.Data;

public class Interpreter : Expr<object>.IVisitor {

    public void Interpet(Expr<object> expression) {
        try {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        } catch (RunTimeError error) {
            Lox.RuntimeError(error);
        }
    }

    public object VisitAssignExpr(Expr<object>.Assign expr) {
        throw new NotImplementedException();
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
                                    "Operands must be two numbers or two strings");
            }
            case TokenType.SLASH: {
                CheckNumberOperands(expr.Operator, left, right);
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
        throw new NotImplementedException();
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
}