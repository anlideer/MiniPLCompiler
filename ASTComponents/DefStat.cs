using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class DefStat : BaseNode
    {
        public Token iden;
        public Token idenType;
        public Expr expression; // can be null

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // var
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.VAR) // won't happen but...
                return null;

            // identifier
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.IDENTIFIER)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of identifier here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }
            iden = currentToken;

            // :
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.COLON)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of : here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }
            

            // type
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.INT_TYPE && currentToken.type != TokenType.STRING_TYPE && currentToken.type != TokenType.BOOL_TYPE)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of type (int, string, bool) here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }
            idenType = currentToken;

            // ; or :=
            currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.SEMICOLON)
            {
                return this;
            }
            else if (currentToken.type == TokenType.ASSIGN)
            {
                // expr
                expression = (Expr)new Expr().TryBuild(ref scanner);
                if (expression == null)
                {
                    SkipHelper.SkipToSemi(ref scanner, currentToken);
                    return null;
                }

                // ;
                currentToken = scanner.PullOneToken();
                if (currentToken.type == TokenType.SEMICOLON)
                {
                    return this;
                }
                else
                {
                    // now we have ; tolerance
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of ; here"));
                    scanner.PushOneToken(currentToken);
                    return this;
                }
            }
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of ; or := here"));
                // here I'm not doing the ; tolerance anymore, just skip until the next ;
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitDef(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeDef(this);
        }

    }
}
