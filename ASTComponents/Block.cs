using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class Block : BaseNode
    {
        public List<Statement> stats = new List<Statement>();

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
            // begin
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.BEGIN)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword begin"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // {statement}
            currentToken = scanner.PullOneToken();
            while (currentToken.type != TokenType.END && currentToken.type != TokenType.END_OF_PROGRAM)
            {
                scanner.PushOneToken(currentToken);
                Statement tmps = (Statement)new Statement().TryBuild(ref scanner);
                if (tmps == null)
                    SkipHelper.SkipToSemi(ref scanner);
                else
                    stats.Add(tmps);

                currentToken = scanner.PullOneToken();
            }

            // end
            if (currentToken.type != TokenType.END)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword end"));
                SkipHelper.SkipToSemiOrDot(ref scanner);
                return this;
            }

            return this;

        }
    }
}
