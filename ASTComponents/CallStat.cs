using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class CallStat : BaseNode
    {
        public Token iden;
        public Arguments args;

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
            // id
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.IDENTIFIER)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect an identifier here"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // (
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.LEFT_BRACKET)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect a ( here to complete a call"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // args
            args = (Arguments)new Arguments().TryBuild(ref scanner);
            if (args == null)
                return null;

            // )
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.RIGHT_BRACKET)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect a ) here to complete a call"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            return this;
        }
    }
}
