using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class PLType : BaseNode
    {
        public Token pltype;
        public Expr expr;   // optional integer expr
        public bool isArray;

        public VarType ty;

        public override void Accept(Visitor visitor)
        {
            visitor.VisitPLType(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExePLType(this);
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            HashSet<TokenType> simpleTypes = new HashSet<TokenType> {
                TokenType.INT_TYPE,
                TokenType.REAL_TYPE,
                TokenType.STRING_TYPE,
                TokenType.BOOL_TYPE,
            };

            Token currentToken = scanner.PullOneToken();
            // simple type
            if (simpleTypes.Contains(currentToken.type))
            {
                isArray = false;
                pltype = currentToken;
                return this;
            }
            // array type
            else if (currentToken.type == TokenType.ARRAY)
            {
                isArray = true;
                // [
                currentToken = scanner.PullOneToken();
                if (currentToken.type != TokenType.LEFT_SBRACKET)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect [ here to define an array"));
                    scanner.PushOneToken(currentToken);
                    return null;
                }
                // optional expr
                // starting with +/-/not/id/literal/(
                HashSet<TokenType> startSet = new HashSet<TokenType> { 
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
                        ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "***Shouldn't happen, check your startSet in PLType"));
                        return null;
                    }
                    currentToken = scanner.PullOneToken();
                }
                
                // ]
                if (currentToken.type != TokenType.RIGHT_SBRACKET)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ] here to define an array"));
                    scanner.PushOneToken(currentToken);
                    return null;
                }

                // of
                currentToken = scanner.PullOneToken();
                if (currentToken.type != TokenType.OF)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect \"of\" here to define an array"));
                    scanner.PushOneToken(currentToken);
                    return null;
                }

                // simple type
                currentToken = scanner.PullOneToken();
                if (simpleTypes.Contains(currentToken.type))
                {
                    pltype = currentToken;
                    return this;
                }
                else
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Exepct a type here to define array's type"));
                    scanner.PushOneToken(currentToken);
                    return null;
                }

            }
            // error
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect type here"));
                scanner.PushOneToken(currentToken);
                return null;
            }
        }
    }
}
