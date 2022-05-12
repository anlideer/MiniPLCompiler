using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class Expr : BaseNode
    {
        public SimpleExpr left;
        // optional
        public Token op;
        public SimpleExpr right;

        public VarType ty;

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            left = (SimpleExpr)new SimpleExpr().TryBuild(ref scanner);
            if (left == null)
                return null;

            // optional op+right
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.RELATIONAL_OPERATOR)
            {
                op = currentToken;
                right = (SimpleExpr)new SimpleExpr().TryBuild(ref scanner);
                if (right == null)
                    return null;
            }
            else
            {
                scanner.PushOneToken(currentToken);
            }

            return this;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitExpr(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeExpr(this);
        }
    }
}
