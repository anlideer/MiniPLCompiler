using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class AssertStat : BaseNode
    {
        public Expr expression;


        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.ASSERT)
            {
                Token nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.LEFT_BRACKET)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ( here"));
                    // skip until ;
                    while (nextToken.type != TokenType.SEMICOLON && nextToken.type != TokenType.END_OF_PROGRAM)
                    {
                        nextToken = scanner.PullOneToken();
                    }
                    return null;
                }
                expression = (Expr)new Expr().TryBuild(ref scanner);
                if (expression == null)
                {
                    // skip until ;
                    Token nextt = scanner.PullOneToken();
                    // skip until ;
                    while (nextt.type != TokenType.SEMICOLON && nextt.type != TokenType.END_OF_PROGRAM)
                    {
                        nextt = scanner.PullOneToken();
                    }
                    return null;
                }
                nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.RIGHT_BRACKET)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ) here"));
                    // skip until ;
                    while (nextToken.type != TokenType.SEMICOLON && nextToken.type != TokenType.END_OF_PROGRAM)
                    {
                        nextToken = scanner.PullOneToken();
                    }
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
    }
}
