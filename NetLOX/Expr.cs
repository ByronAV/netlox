
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

public abstract class Expr<R> {
    public interface IVisitor {
        abstract R visitAssignExpr(Assign expr);
        R visitBinaryExpr(Binary expr);
        R visitCallExpr(Call expr);
        R visitGetExpr(Get expr);
        R visitGroupingExpr(Grouping expr);
        R visitLiteralExpr(Literal expr);
        R visitLogicalExpr(Logical expr);
        R visitSetExpr(Set expr);
        R visitSuperExpr(Super expr);
        R visitThisExpr(This expr);
        R visitUnaryExpr(Unary expr);
        R visitVariableExpr(Variable expr);
    }

    public abstract R accept(IVisitor visitor);

    public class Assign : Expr<R> {
        public Assign(Token name, Expr<R> value) {
            this.name = name;
            this.value = value;
        }

        public override R accept(IVisitor visitor) {
            return visitor.visitAssignExpr(this);
        }

        public readonly Token name;
        public readonly Expr<R> value;
    }

    public class Binary : Expr<R> {
        Binary(Expr<R> left, Token _operator, Expr<R> right) {
            this.left = left;
            this._operator = _operator;
            this.right = right;
        }

        public override R accept(IVisitor visitor) {
            return visitor.visitBinaryExpr(this);
        }

        public readonly Expr<R> left;
        public readonly Token _operator;
        public readonly Expr<R> right;
    }

    public class Call : Expr<R> {
        Call(Expr<R> callee, Token paren, List<Expr<R>> arguments) {
            this.callee = callee;
            this.paren = paren;
            this.arguments = arguments;
        }

        public override R accept(IVisitor visitor) {
            return visitor.visitCallExpr(this);
        }

        public readonly Expr<R> callee;
        public readonly Token paren;
        public readonly List<Expr<R>> arguments;
    }

    public class Get : Expr<R> {
        Get(Expr<R> _object, Token name) {
            this._object = _object;
            this.name = name;
        }

        public override R accept(IVisitor visitor) {
            return visitor.visitGetExpr(this);
        }

        public readonly Expr<R> _object;
        public readonly Token name;
    }

    public class Grouping : Expr<R> {
        Grouping(Expr<R> expression) {
            this.expression = expression;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitGroupingExpr(this);
        }

        public readonly Expr<R> expression;
    }


    public class Literal : Expr<R> {
        Literal(object value) {
            this.value = value;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitLiteralExpr(this);
        }

        public readonly object value;
    }

    public class Logical : Expr<R> {
        Logical(Expr<R> left, Token _operator, Expr<R> right) {
            this.left = left;
            this._operator = _operator;
            this.right = right;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitLogicalExpr(this);
        }

        public readonly Expr<R> left;
        public readonly Token _operator;
        public readonly Expr<R> right;
    }


    public class Set : Expr<R> {
        Set(Expr<R> _object, Token name, Expr<R> value) {
            this._object = _object;
            this.name = name;
            this.value = value;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitSetExpr(this);
        }

        public readonly Expr<R> _object;
        public readonly Token name;
        public readonly Expr<R> value;
    }


    public class Super : Expr<R> {
        Super(Token keyword, Token method) {
            this.keyword = keyword;
            this.method = method;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitSuperExpr(this);
        }

        public readonly Token keyword;
        public readonly Token method;
    }


    public class This : Expr<R> {
        This(Token keyword) {
            this.keyword = keyword;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitThisExpr(this);
        }

        public readonly Token keyword;
    }


    public class Unary : Expr<R> {
        Unary(Token _operator, Expr<R> right) {
            this._operator = _operator;
            this.right = right;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitUnaryExpr(this);
        }

        public readonly Token _operator;
        public readonly Expr<R> right;
    }


    public class Variable : Expr<R> {
        Variable(Token name) {
            this.name = name;
        }

        public override R accept(IVisitor visitor)
        {
            return visitor.visitVariableExpr(this);
        }

        public readonly Token name;
    }
}