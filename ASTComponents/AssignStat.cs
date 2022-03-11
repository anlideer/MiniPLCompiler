using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class AssignStat : BaseNode
    {
        public Token iden;
        public Expr expression;

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.IDENTIFIER)
            {
                iden = currentToken;
                // :=
                Token nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.ASSIGN)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of := here"));
                    // directly skip to ;
                    SkipHelper.SkipToSemi(ref scanner, nextToken);
                    return null;
                }

                // expr
                expression = (Expr)new Expr().TryBuild(ref scanner);
                if (expression == null)
                {
                    // directly skip to ;
                    SkipHelper.SkipToSemi(ref scanner, nextToken);
                    return null;
                }

                // ;
                nextToken = scanner.PullOneToken();
                if (nextToken.type == TokenType.SEMICOLON)
                    return this;
                else
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ; here"));
                    scanner.PushOneToken(nextToken);
                    return this;
                }

            }
            else
            {
                return null;
            }
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitAssign(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeAssign(this);
        }

    }
}
