using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class Param
    {
        public Token iden;
        public PLType pltype;
        public bool byRef;

        public VarType ty;
    }

    class Parameters : BaseNode
    {
        public List<Param> parameters = new List<Param>();

        public override void Accept(Visitor visitor)
        {
            visitor.VisitParameter(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeParameters(this);
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // var
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.VAR || currentToken.type == TokenType.IDENTIFIER)
            {
                // param
                scanner.PushOneToken(currentToken);
                var p = ParseParam(ref scanner);
                if (p == null)
                {
                    SkipHelper.SkipTo(ref scanner, TokenType.RIGHT_BRACKET);
                    return null;
                }
                else
                    parameters.Add(p);

                // {, param}
                currentToken = scanner.PullOneToken();
                while(currentToken.type == TokenType.COMMA)
                {
                    p = ParseParam(ref scanner);
                    if (p == null)
                    {
                        SkipHelper.SkipTo(ref scanner, TokenType.RIGHT_BRACKET);
                        return null;
                    }
                    else
                        parameters.Add(p);
                    currentToken = scanner.PullOneToken();
                }

                scanner.PushOneToken(currentToken);
                return this;

            }
            // empty
            else
            {
                scanner.PushOneToken(currentToken);
                return this;
            }

        }

        private Param ParseParam(ref Scanner scanner)
        {
            Param p = new Param();
            Token currentToken = scanner.PullOneToken();
            // [var]
            if (currentToken.type == TokenType.VAR)
            {
                p.byRef = true;
                currentToken = scanner.PullOneToken();
            }
            else
                p.byRef = false;

            // id
            if (currentToken.type != TokenType.IDENTIFIER)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect identifier"));
                scanner.PushOneToken(currentToken);
                return null;
            }
            else
                p.iden = currentToken;

            // :
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.COLON)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect :"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // type
            p.pltype = (PLType)new PLType().TryBuild(ref scanner);
            if (p.pltype == null)
                return null;

            return p;
        }
    }
}
