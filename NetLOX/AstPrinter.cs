
using System.Text;

class AstPrinter : Expr<string>.IVisitor {
    public string Print(Expr<string> expr) {
        return expr.Accept(this);
    }

    public string VisitAssignExpr(Expr<string>.Assign expr) {
        throw new NotImplementedException();
    }

    public string VisitBinaryExpr(Expr<string>.Binary expr) {
        return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitCallExpr(Expr<string>.Call expr) {
        throw new NotImplementedException();
    }

    public string VisitGetExpr(Expr<string>.Get expr) {
        throw new NotImplementedException();
    }

    public string VisitGroupingExpr(Expr<string>.Grouping expr) {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Expr<string>.Literal expr) {
        if (expr.Value == null) return "nil";
        return expr.Value.ToString();
    }

    public string VisitLogicalExpr(Expr<string>.Logical expr) {
        throw new NotImplementedException();
    }

    public string VisitSetExpr(Expr<string>.Set expr) {
        throw new NotImplementedException();
    }

    public string VisitSuperExpr(Expr<string>.Super expr) {
        throw new NotImplementedException();
    }

    public string VisitThisExpr(Expr<string>.This expr) {
        throw new NotImplementedException();
    }

    public string VisitUnaryExpr(Expr<string>.Unary expr) {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    public string VisitVariableExpr(Expr<string>.Variable expr) {
        throw new NotImplementedException();
    }

    private string Parenthesize(string name, params Expr<string>[] exprs) {
        StringBuilder builder = new StringBuilder();

        builder.Append("(").Append(name);
        foreach(Expr<string> expr in exprs) {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");

        return builder.ToString();
    }
}