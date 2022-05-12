using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class WhileStat : BaseNode
    {
        public Expr expr;   // boolean expr
        public Statement stat;
        public Token whileToken;
        

        public override void Accept(Visitor visitor)
        {
            visitor.VisitWhile(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // while
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.WHILE)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword while"));
                scanner.PushOneToken(currentToken);
                return null;
            }
            whileToken = currentToken;

            // expr
            expr = (Expr)new Expr().TryBuild(ref scanner);
            if (expr == null)
                return null;

            // do
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.DO)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword do"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // statement
            stat = (Statement)new Statement().TryBuild(ref scanner);
            if (stat == null)
                return null;

            return this;

        }
    }
}
