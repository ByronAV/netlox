
using System.Text;

class AstPrinter : Expr<string>.IVisitor {
    public string Print(Expr<string> expr) {
        return expr.Accept(this);
    }

    public string VisitAssignExpr(Expr<string>.Assign expr) {
        throw new NotImplementedException();
    }

    public string VisitBinaryExpr(Expr<string>.Binary expr) {
        return Parenthesize(expr._operator.lexeme, expr.left, expr.right);
    }

    public string VisitCallExpr(Expr<string>.Call expr) {
        throw new NotImplementedException();
    }

    public string VisitGetExpr(Expr<string>.Get expr) {
        throw new NotImplementedException();
    }

    public string VisitGroupingExpr(Expr<string>.Grouping expr) {
        return Parenthesize("group", expr.expression);
    }

    public string VisitLiteralExpr(Expr<string>.Literal expr) {
        if (expr.value == null) return "nil";
        return expr.value.ToString();
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
        return Parenthesize(expr._operator.lexeme, expr.right);
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