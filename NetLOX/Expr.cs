
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

public abstract class Expr<R> {
    public interface IVisitor {
        R VisitAssignExpr(Assign expr);
        R VisitBinaryExpr(Binary expr);
        R VisitCallExpr(Call expr);
        R VisitGetExpr(Get expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitLogicalExpr(Logical expr);
        R VisitSetExpr(Set expr);
        R VisitSuperExpr(Super expr);
        R VisitThisExpr(This expr);
        R VisitUnaryExpr(Unary expr);
        R VisitVariableExpr(Variable expr);
    }

    public abstract R Accept(IVisitor visitor);

    public class Assign : Expr<R> {
        public Assign(Token name, Expr<R> value) {
            this.name = name;
            this.value = value;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitAssignExpr(this);
        }

        public readonly Token name;
        public readonly Expr<R> value;
    }

    public class Binary : Expr<R> {
        public Binary(Expr<R> left, Token _operator, Expr<R> right) {
            this.left = left;
            this._operator = _operator;
            this.right = right;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitBinaryExpr(this);
        }

        public readonly Expr<R> left;
        public readonly Token _operator;
        public readonly Expr<R> right;
    }

    public class Call : Expr<R> {
        public Call(Expr<R> callee, Token paren, List<Expr<R>> arguments) {
            this.callee = callee;
            this.paren = paren;
            this.arguments = arguments;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitCallExpr(this);
        }

        public readonly Expr<R> callee;
        public readonly Token paren;
        public readonly List<Expr<R>> arguments;
    }

    public class Get : Expr<R> {
        public Get(Expr<R> _object, Token name) {
            this._object = _object;
            this.name = name;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitGetExpr(this);
        }

        public readonly Expr<R> _object;
        public readonly Token name;
    }

    public class Grouping : Expr<R> {
        public Grouping(Expr<R> expression) {
            this.expression = expression;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }

        public readonly Expr<R> expression;
    }


    public class Literal : Expr<R> {
        public Literal(object value) {
            this.value = value;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }

        public readonly object? value;
    }

    public class Logical : Expr<R> {
        public Logical(Expr<R> left, Token _operator, Expr<R> right) {
            this.left = left;
            this._operator = _operator;
            this.right = right;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }

        public readonly Expr<R> left;
        public readonly Token _operator;
        public readonly Expr<R> right;
    }


    public class Set : Expr<R> {
        public Set(Expr<R> _object, Token name, Expr<R> value) {
            this._object = _object;
            this.name = name;
            this.value = value;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitSetExpr(this);
        }

        public readonly Expr<R> _object;
        public readonly Token name;
        public readonly Expr<R> value;
    }


    public class Super : Expr<R> {
        public Super(Token keyword, Token method) {
            this.keyword = keyword;
            this.method = method;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitSuperExpr(this);
        }

        public readonly Token keyword;
        public readonly Token method;
    }


    public class This : Expr<R> {
        public This(Token keyword) {
            this.keyword = keyword;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitThisExpr(this);
        }

        public readonly Token keyword;
    }


    public class Unary : Expr<R> {
        public Unary(Token _operator, Expr<R> right) {
            this._operator = _operator;
            this.right = right;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }

        public readonly Token _operator;
        public readonly Expr<R> right;
    }


    public class Variable : Expr<R> {
        Variable(Token name) {
            this.name = name;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitVariableExpr(this);
        }

        public readonly Token name;
    }
}