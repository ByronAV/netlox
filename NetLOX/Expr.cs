
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
            _name = name;
            _value = value;
        }

        public Token Name {
            get => _name;
        }

        public Expr<R> Value {
            get => _value;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitAssignExpr(this);
        }

        private readonly Token _name;
        private readonly Expr<R> _value;
    }

    public class Binary : Expr<R> {
        public Binary(Expr<R> left, Token _operator, Expr<R> right) {
            _left = left;
            this._operator = _operator;
            _right = right;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitBinaryExpr(this);
        }

        public Expr<R> Left {
            get => _left;
        }

        public Token Operator {
            get => _operator;
        }

        public Expr<R> Right {
            get => _right;
        }

        private readonly Expr<R> _left;
        private readonly Token _operator;
        private readonly Expr<R> _right;
    }

    public class Call : Expr<R> {
        public Call(Expr<R> callee, Token paren, List<Expr<R>> arguments) {
            _callee = callee;
            _paren = paren;
            _arguments = arguments;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitCallExpr(this);
        }

        public Expr<R> Callee {
            get => _callee;
        }

        public Token Paren {
            get => _paren;
        }

        public List<Expr<R>> Arguments {
            get => _arguments;
        }
        

        private readonly Expr<R> _callee;
        private readonly Token _paren;
        private readonly List<Expr<R>> _arguments;
    }

    public class Get : Expr<R> {
        public Get(Expr<R> _object, Token name) {
            this._object = _object;
            _name = name;
        }

        public override R Accept(IVisitor visitor) {
            return visitor.VisitGetExpr(this);
        }

        public Expr<R> Object {
            get => _object;
        }

        public Token Name {
            get => _name;
        }

        private readonly Expr<R> _object;
        private readonly Token _name;
    }

    public class Grouping : Expr<R> {
        public Grouping(Expr<R> expression) {
            _expression = expression;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }

        public Expr<R> Expression {
            get => _expression;
        }

        private readonly Expr<R> _expression;
    }


    public class Literal : Expr<R> {
        public Literal(object value) {
            _value = value;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }

        public object? Value {
            get => _value;
        }

        private readonly object? _value;
    }

    public class Logical : Expr<R> {
        public Logical(Expr<R> left, Token _operator, Expr<R> right) {
            _left = left;
            this._operator = _operator;
            _right = right;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }

        public Expr<R> Left {
            get => _left;
        }

        public Token Operator {
            get => _operator;
        }

        public Expr<R> Right {
            get => _right;
        }

        private readonly Expr<R> _left;
        private readonly Token _operator;
        private readonly Expr<R> _right;
    }


    public class Set : Expr<R> {
        public Set(Expr<R> _object, Token name, Expr<R> value) {
            this._object = _object;
            _name = name;
            _value = value;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitSetExpr(this);
        }

        public Expr<R> Object {
            get => _object;
        }

        public Token Name {
            get => _name;
        }

        public Expr<R> Value {
            get => _value;
        }

        private readonly Expr<R> _object;
        private readonly Token _name;
        private readonly Expr<R> _value;
    }


    public class Super : Expr<R> {
        public Super(Token keyword, Token method) {
            _keyword = keyword;
            _method = method;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitSuperExpr(this);
        }

        public Token Keyword {
            get => _keyword;
        }

        public Token Method {
            get => _method;
        }

        private readonly Token _keyword;
        private readonly Token _method;
    }


    public class This : Expr<R> {
        public This(Token keyword) {
            _keyword = keyword;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitThisExpr(this);
        }

        public Token Keyword {
            get => _keyword;
        }

        private readonly Token _keyword;
    }


    public class Unary : Expr<R> {
        public Unary(Token _operator, Expr<R> right) {
            this._operator = _operator;
            _right = right;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }

        public Token Operator {
            get => _operator;
        }

        public Expr<R> Right {
            get => _right;
        }

        private readonly Token _operator;
        private readonly Expr<R> _right;
    }


    public class Variable : Expr<R> {
        Variable(Token name) {
            _name = name;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitVariableExpr(this);
        }

        public Token Name {
            get => _name;
        }

        private readonly Token _name;
    }
}