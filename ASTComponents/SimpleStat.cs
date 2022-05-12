using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class SimpleStat : BaseNode
    {
        public BaseNode stat;

        public override void Accept(Visitor visitor)
        {
            visitor.VisitSimpleStat(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // analyze starting set
            Token currentToken = scanner.PullOneToken();
            // id (assgin / call)
            if (currentToken.type == TokenType.IDENTIFIER)
            {
                Token nextToken = scanner.PullOneToken();
                // ( call
                if (nextToken.type == TokenType.LEFT_BRACKET)
                {
                    scanner.PushOneToken(nextToken);
                    scanner.PushOneToken(currentToken);
                    stat = new CallStat().TryBuild(ref scanner);
                }
                // assignment
                else
                {
                    scanner.PushOneToken(nextToken);
                    scanner.PushOneToken(currentToken);
                    stat = new AssignStat().TryBuild(ref scanner);
                }
            }
            // return
            else if (currentToken.type == TokenType.RETURN)
            {
                scanner.PushOneToken(currentToken);
                stat = new ReturnStat().TryBuild(ref scanner);
            }
            // read
            else if (currentToken.type == TokenType.READ)
            {
                scanner.PushOneToken(currentToken);
                stat = new ReadStat().TryBuild(ref scanner);
            }
            // write
            else if (currentToken.type == TokenType.WRITELN)
            {
                scanner.PushOneToken(currentToken);
                stat = new PrintStat().TryBuild(ref scanner);
            }
            // assert
            else if (currentToken.type == TokenType.ASSERT)
            {
                scanner.PushOneToken(currentToken);
                stat = new AssertStat().TryBuild(ref scanner);
            }
            // error
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Unacceptable token for starting a simple statement"));
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
