using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class PrintStat : BaseNode
    {
        public Arguments args;

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            // writeln
            if (currentToken.type != TokenType.WRITELN)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword writeln"));
                scanner.PushOneToken(currentToken);
                return null;
            }
            // (
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.LEFT_BRACKET)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Exepct ("));
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
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Exepct )"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            return this;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitPrint(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExePrint(this);
        }
    }
}
