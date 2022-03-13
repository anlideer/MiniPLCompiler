using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    class AssertStat : BaseNode
    {
        public Expr expression;
        public Token assertToken;   // just for reporting the position, not really useful

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.ASSERT)
            {
                assertToken = currentToken;
                Token nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.LEFT_BRACKET)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ( here"));
                    // skip
                    SkipHelper.SkipToSemi(ref scanner, nextToken);
                    return null;
                }
                expression = (Expr)new Expr().TryBuild(ref scanner);
                if (expression == null)
                {
                    // skip
                    SkipHelper.SkipToSemi(ref scanner);
                    return null;
                }
                nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.RIGHT_BRACKET)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ) here"));
                    // skip until ;
                    SkipHelper.SkipToSemi(ref scanner, nextToken);
                    return this;
                }
                nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.SEMICOLON)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ; here"));
                    scanner.PushOneToken(nextToken);
                    return this;
                }
                // ok
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
