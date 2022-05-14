using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class IfStat : BaseNode
    {
        public Expr expr;   // boolean expr
        public Statement thenStat;
        public Statement elseStat;  // optional
        // for error report
        public Token ifToken;

        public override void Accept(Visitor visitor)
        {
            visitor.VisitIf(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeIfStat(this);
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // if
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.IF)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword if"));
                scanner.PushOneToken(currentToken);
                return null;
            }
            ifToken = currentToken;

            // expr
            expr = (Expr)new Expr().TryBuild(ref scanner);
            if (expr == null)
                return null;

            // then
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.THEN)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword then"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // statement
            thenStat = (Statement)new Statement().TryBuild(ref scanner);
            if (thenStat == null)
                return null;

            // [else statement]
            currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.ELSE)
            {
                // statement
                elseStat = (Statement)new Statement().TryBuild(ref scanner);
                if (elseStat == null)
                    return null;
                return this;
            }
            else
            {
                scanner.PushOneToken(currentToken);
                return this;
            }
        }

    }
}
