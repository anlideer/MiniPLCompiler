using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class Expr : BaseNode
    {
        // valid expr: left+op+right, unary_op+right, left
        public Opnd left;
        public Token op;
        public Opnd right;


        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();    // preread
            if (currentToken.type == TokenType.UNARY_OPERATOR)
            {
                op = currentToken;
                right = (Opnd)new Opnd().TryBuild(ref scanner);
                // try to get right
                if (right != null)
                    return this;
                else
                    return null;    // opnd error
            }
            // left+op+right / left
            else
            {
                scanner.PushOneToken(currentToken);
                left = (Opnd)new Opnd().TryBuild(ref scanner);
                if (left == null)
                    return null;

                // look ahead
                Token nextToken = scanner.PullOneToken();
                // just left
                if (nextToken.type != TokenType.OPERATOR)
                {
                    scanner.PushOneToken(nextToken);
                    return this;
                }
                // left+op+right
                else
                {
                    op = nextToken;
                    right = (Opnd)new Opnd().TryBuild(ref scanner);
                    if (right == null)
                        return null;
                    return this;
                }
            }
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitExpr(this);
        }
    }
}
