using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class Statement : BaseNode
    {
        public BaseNode stat;

        public override void Accept(Visitor visitor)
        {
            visitor.VisitStatement(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            // structured
            if (currentToken.type == TokenType.BEGIN || currentToken.type == TokenType.IF || currentToken.type == TokenType.WHILE)
            {
                scanner.PushOneToken(currentToken);
                stat = new StructuredStat().TryBuild(ref scanner);
                if (stat == null)
                    return null;
                else
                    return this;
            }
            // var
            else if (currentToken.type == TokenType.VAR)
            {
                scanner.PushOneToken(currentToken);
                stat = new DefStat().TryBuild(ref scanner);
                if (stat == null)
                {
                    SkipHelper.SkipToSemi(ref scanner);
                    return null;
                }

                // ;
                currentToken = scanner.PullOneToken();
                if (currentToken.type != TokenType.SEMICOLON)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ; after var-declaration"));
                    scanner.PushOneToken(currentToken); // tolerant
                }
                return this;
            }
            // simple stat
            else
            {
                scanner.PushOneToken(currentToken);
                stat = new SimpleStat().TryBuild(ref scanner);

                if (stat == null)
                {
                    SkipHelper.SkipToSemi(ref scanner);
                    return null;
                }

                // ;
                currentToken = scanner.PullOneToken();
                if (currentToken.type != TokenType.SEMICOLON)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ; after simple statement"));
                    scanner.PushOneToken(currentToken); // tolerant
                }
                return this;
            }
        }
    }
}
