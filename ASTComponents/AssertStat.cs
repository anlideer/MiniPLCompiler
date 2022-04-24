using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class AssertStat : BaseNode
    {
        public Expr expression; // boolean expr
        public Token assertToken;   // just for reporting the position, not really useful

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            // assert
            if (currentToken.type == TokenType.ASSERT)
            {
                assertToken = currentToken;
                Token nextToken = scanner.PullOneToken();
                // (
                if (nextToken.type != TokenType.LEFT_BRACKET)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ( here"));
                    return null;
                }
                // expr
                expression = (Expr)new Expr().TryBuild(ref scanner);
                if (expression == null)
                {
                    return null;
                }
                // )
                nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.RIGHT_BRACKET)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ) here"));
                    return this;
                }

                return this;
            }
            else
                return null;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitAssert(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeAssert(this);   
        }
    }
}
