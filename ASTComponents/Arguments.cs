using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class Arguments : BaseNode
    {
        public List<Expr> exprs = new List<Expr>(); // can be empty

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

            Token currentToken = scanner.PullOneToken();
            if (startSet.Contains(currentToken.type))
            {
                scanner.PushOneToken(currentToken);
                Expr tmp = (Expr)new Expr().TryBuild(ref scanner);
                if (tmp == null)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "***Shouldn't happen, check Arguments."));
                    return null;
                }
                exprs.Add(tmp);

                // {, expr}
                currentToken = scanner.PullOneToken();
                while (currentToken.type == TokenType.COMMA)
                {
                    // expr
                    Expr tmp2 = (Expr)new Expr().TryBuild(ref scanner);
                    if (tmp2 == null)
                    {
                        ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect an expression after ,"));
                        return null;
                    }

                    currentToken = scanner.PullOneToken();
                }
                scanner.PushOneToken(currentToken);
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
