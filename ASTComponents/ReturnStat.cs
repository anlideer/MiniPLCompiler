using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class ReturnStat : BaseNode
    {
        public Token token; // only to provide location to error report
        public Expr expr;

        public VarType ty = VarType.NONE;

        public override void Accept(Visitor visitor)
        {
            visitor.VisitReturn(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.RETURN)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect \"return\" here"));
                scanner.PushOneToken(currentToken);
                return null;
            }
            token = currentToken;

            // expr
            // starting with +/-/not/id/literal/(
            HashSet<TokenType> startSet = new HashSet<TokenType>
            {
                    TokenType.ADD_OPERATOR,
                    TokenType.NOT,
                    TokenType.IDENTIFIER,
                    TokenType.REAL_VAL,
                    TokenType.INT_VAL,
                    TokenType.STRING_VAL,
                    TokenType.LEFT_BRACKET,
            };

            currentToken = scanner.PullOneToken();
            if (startSet.Contains(currentToken.type))
            {
                scanner.PushOneToken(currentToken);
                expr = (Expr)new Expr().TryBuild(ref scanner);
                if (expr == null)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "***Shouldn't happen. Cehck ReturnStat"));
                    return null;
                }
            }
            else
                scanner.PushOneToken(currentToken);

            return this;

        }
    }
}
