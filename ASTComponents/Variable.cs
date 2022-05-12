using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class Variable : BaseNode
    {
        public Token iden;
        public Expr expr;   // optional integer expr

        public VarType ty;

        public override void Accept(Visitor visitor)
        {
            throw new NotImplementedException();
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // identifier
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.IDENTIFIER)
            {
                iden = currentToken;
            }
            else
            {
                scanner.PushOneToken(currentToken);
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect identifier here"));
                return null;
            }

            // [
            currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.LEFT_SBRACKET)
            {
                // integer expr
                expr = (Expr)new Expr().TryBuild(ref scanner);
                if (expr == null)
                    return null;
                // ]
                currentToken = scanner.PullOneToken();
                if (currentToken.type != TokenType.RIGHT_SBRACKET)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ] here"));
                    scanner.PushOneToken(currentToken);
                    return null;
                }

            }
            else
            {
                scanner.PushOneToken(currentToken);
            }

            return this;
        }
    }
}
