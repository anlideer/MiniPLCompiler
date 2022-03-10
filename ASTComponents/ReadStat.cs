using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class ReadStat : BaseNode
    {
        public Token iden;

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.READ)
            {
                // identifier
                Token nextToken = scanner.PullOneToken();
                if (nextToken.type == TokenType.IDENTIFIER)
                {
                    iden = nextToken;
                }
                else
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of identifier here"));
                    // skip to ;
                    SkipHelper.SkipToSemi(ref scanner, nextToken);
                    return null;
                }

                // ;
                nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.SEMICOLON)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ; here"));
                    // push back this token, continue the program (hopefully ignoring the error and continue
                    scanner.PushOneToken(nextToken);
                    return this;
                }
                else
                {
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
            visitor.VisitRead(this);
        }
    }
}
