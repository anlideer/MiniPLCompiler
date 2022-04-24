using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class StructuredStat : BaseNode
    {
        public BaseNode stat;

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
            Token currentToken = scanner.PullOneToken();
            // block
            if (currentToken.type == TokenType.BEGIN)
            {
                scanner.PushOneToken(currentToken);
                stat = new Block().TryBuild(ref scanner);
            }
            // if
            else if (currentToken.type == TokenType.IF)
            {
                scanner.PushOneToken(currentToken);
                stat = new IfStat().TryBuild(ref scanner);
            }
            // while
            else if (currentToken.type == TokenType.WHILE)
            {
                scanner.PushOneToken(currentToken);
                stat = new WhileStat().TryBuild(ref scanner);
            }
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Unaccpetable token for starting a structured statement"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            if (stat == null)
                return null;
            else
                return this;

        }
    }
}
