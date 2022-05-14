using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class Func : BaseNode   // function / procedure
    {
        public Token iden;
        public Parameters parameters;
        public Block block;
        public bool isFunc;
        public PLType returnedType;
        public VarType ty;

        public override void Accept(Visitor visitor)
        {
            visitor.VisitFunction(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeFunc(this);
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            // function / procedure
            if (currentToken.type == TokenType.FUNCTION)
            {
                isFunc = true;
            }
            else if (currentToken.type == TokenType.PROCEDURE)
            {
                isFunc = false;
            }
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword function or procedure"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // id
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.IDENTIFIER)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect identifier"));
                SkipHelper.SkipToEndSemi(ref scanner, currentToken);
                return null;
            }
            iden = currentToken;

            // (
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.LEFT_BRACKET)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ("));
                SkipHelper.SkipToEndSemi(ref scanner, currentToken);
                return null;
            }

            // param 
            parameters = (Parameters)new Parameters().TryBuild(ref scanner);
            if (parameters == null)
            {
                SkipHelper.SkipToEndSemi(ref scanner);
                return null;
            }

            // )
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.RIGHT_BRACKET)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect )"));
                SkipHelper.SkipToEndSemi(ref scanner, currentToken);
                return null;
            }

            // function's type
            if (isFunc)
            {
                // :
                currentToken = scanner.PullOneToken();
                if (currentToken.type != TokenType.COLON)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect : because it's function"));
                    SkipHelper.SkipToEndSemi(ref scanner, currentToken);
                    return null;
                }
                // type
                returnedType = (PLType) new PLType().TryBuild(ref scanner);
                if (returnedType == null)
                {
                    SkipHelper.SkipToEndSemi(ref scanner);
                    return null;
                }
            }

            // ;
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.SEMICOLON)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ;"));
                SkipHelper.SkipToEndSemi(ref scanner, currentToken);
                return null;
            }

            // block
            block = (Block)new Block().TryBuild(ref scanner);
            if (block == null)
            {
                SkipHelper.SkipToEndSemi(ref scanner);
                return null;
            }

            // ;
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.SEMICOLON)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ;"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            return this;

        }
    }
}
