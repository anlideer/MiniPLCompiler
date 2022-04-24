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

            // statement
            Statement tmp = (Statement)new Statement().TryBuild(ref scanner);
            if (tmp == null)
                SkipHelper.SkipToSemiOrEnd(ref scanner);
            else
                stats.Add(tmp);

            // {; statement}
            currentToken = scanner.PullOneToken();
            bool flag = false;
            while (currentToken.type == TokenType.SEMICOLON)
            {
                // statement or end
                Token nextToken = scanner.PullOneToken();
                if (nextToken.type == TokenType.END)
                {
                    scanner.PushOneToken(nextToken);
                    flag = true;
                    break;
                }
                else
                    scanner.PushOneToken(nextToken);

                Statement tmps = (Statement)new Statement().TryBuild(ref scanner);
                if (tmps == null)
                    SkipHelper.SkipToSemiOrEnd(ref scanner);
                else
                    stats.Add(tmps);

                currentToken = scanner.PullOneToken();
            }
            if (flag)
                currentToken = scanner.PullOneToken();


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
