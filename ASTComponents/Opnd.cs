using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class Opnd : BaseNode
    {
        // valid: selfToken, (expression)
        public Token selfToken;
        public Expr expression;


        private HashSet<TokenType> followSet = new HashSet<TokenType>
        {
            TokenType.RIGHT_BRACKET,
            TokenType.OPERATOR,
            TokenType.SEMICOLON,
            TokenType.TO,
            TokenType.DO,
            TokenType.END_OF_PROGRAM
        };

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.INT_VAL || currentToken.type == TokenType.STRING_VAL || currentToken.type == TokenType.IDENTIFIER)
            {
                selfToken = currentToken;
                return this;
            }
            else if (currentToken.type == TokenType.LEFT_BRACKET)
            {
                expression = (Expr)new Expr().TryBuild(ref scanner);
                // expression ok
                if (expression != null)
                {
                    Token nextToken = scanner.PullOneToken();
                    if (nextToken.type == TokenType.RIGHT_BRACKET)
                        return this;
                    else
                    {
                        ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Miss )"));
                        scanner.PushOneToken(nextToken);
                        return null;    // no need to skip
                    }
                }
                // expression error
                else
                {
                    Token nextToken = scanner.PullOneToken();
                    if (nextToken.type == TokenType.RIGHT_BRACKET)
                        return null;
                    else
                    {
                        ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Miss )"));
                        scanner.PushOneToken(nextToken);
                        return null;
                    }
                }
            }
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of Opnd here"));
                // skip until meeting the followSet
                while (!followSet.Contains(currentToken.type))
                {
                    currentToken = scanner.PullOneToken();
                }

                scanner.PushOneToken(currentToken);
                return null;
            }
        }


    }
}
