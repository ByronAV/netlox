
using System.Linq.Expressions;
using Microsoft.VisualBasic;

public abstract class Stmt<R> {
    public interface IVisitor {
        R? VisitBlockStmt(Block stmt);
        R? VisitClassStmt(Class stmt);
        R? VisitExpressionStmt(Expression stmt);
        R? VisitFunctionStmt(Function stmt);
        R? VisitIfStmt(If stmt);
        R? VisitPrintStmt(Print stmt);
        R? VisitReturnStmt(Return stmt);
        R? VisitVarStmt(Var stmt);
        R? VisitWhileStmt(While stmt);
    }

    public abstract R Accept(IVisitor visitor);

    public class Block : Stmt<R> {
        public Block(List<Stmt<R>> statements) {
            _statements = statements;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitBlockStmt(this);
        }

        public List<Stmt<R>> Statements {
            get => _statements;
        }

        private readonly List<Stmt<R>> _statements;
    }


    public class Class : Stmt<R> {
        public Class(Token name, Expr<R>.Variable superclass, List<Function> methods) {
            _name = name;
            _superclass = superclass;
            _methods = methods;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitClassStmt(this);
        }

        public Token Name {
            get => _name;
        }

        public Expr<R>.Variable Superclass {
            get => _superclass;
        }

        public List<Function> Methods {
            get => _methods;
        }

        private readonly Token _name;
        private readonly Expr<R>.Variable _superclass;
        private readonly List<Function> _methods;
    }


    public class Expression : Stmt<R> {
        public Expression(Expr<R> expression) {
            _expression = expression;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }

        public Expr<R> Expresion {
            get => _expression;
        }

        private readonly Expr<R> _expression;
    }


    public class Function : Stmt<R> {
        public Function(Token name, List<Token> _params, List<Stmt<R>> body) {
            _name = name;
            this._params = _params;
            _body = body;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }

        public Token Name {
            get => _name;
        }

        public List<Token> Params {
            get => _params;
        }

        public List<Stmt<R>> Body {
            get => _body;
        }

        private readonly Token _name;
        private readonly List<Token> _params;
        private readonly List<Stmt<R>> _body;
    }


    public class If : Stmt<R> {
        public If(Expr<R> condition, Stmt<R> thenBranch, Stmt<R>? elseBranch) {
            _condition = condition;
            _thenBranch = thenBranch;
            _elseBranch = elseBranch;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitIfStmt(this);
        }

        public Expr<R> Condition {
            get => _condition;
        }

        public Stmt<R> ThenBranch {
            get => _thenBranch;
        }

        public Stmt<R>? ElseBranch {
            get => _elseBranch;
        }

        private readonly Expr<R> _condition;
        private readonly Stmt<R> _thenBranch;
        private readonly Stmt<R>? _elseBranch;
    }


    public class Print : Stmt<R> {
        public Print(Expr<R> expression) {
            _expression = expression;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitPrintStmt(this);
        }

        public Expr<R> Expresion {
            get => _expression;
        }

        private readonly Expr<R> _expression;
    }


    public class Return : Stmt<R> {
        public Return(Token keyword, Expr<R> value) {
            _keyword = keyword;
            _value = value;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitReturnStmt(this);
        }

        public Token Keyword {
            get => _keyword;
        }

        public Expr<R> Value {
            get => _value;
        }

        private readonly Token _keyword;
        private readonly Expr<R> _value;
    }

    public class Var : Stmt<R> {
        public Var(Token name, Expr<R>? initializer) {
            _name = name;
            _initializer = initializer;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitVarStmt(this);
        }

        public Token Name {
            get => _name;
        }

        public Expr<R> Initializer {
            get => _initializer;
        }

        private readonly Token _name;
        private readonly Expr<R>? _initializer;
    }


    public class While : Stmt<R> {
        public While(Expr<R> condition, Stmt<R> body) {
            _condition = condition;
            _body = body;
        }

        public override R Accept(IVisitor visitor)
        {
            return visitor.VisitWhileStmt(this);
        }

        public Expr<R> Condition {
            get => _condition;
        }

        public Stmt<R> Body {
            get => _body;
        }

        private readonly Expr<R> _condition;
        private readonly Stmt<R> _body;
    }
}