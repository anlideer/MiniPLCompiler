using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class DefStat : BaseNode
    {
        public List<Token> idens = new List<Token>();
        public PLType idenType;


        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // var
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.VAR) // won't happen but...
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect var"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // identifier
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.IDENTIFIER)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of identifier here"));
                scanner.PushOneToken(currentToken);
                return null;
            }
            idens.Add(currentToken);

            // , iden
            currentToken = scanner.PullOneToken();
            while(currentToken.type == TokenType.COMMA)
            {
                currentToken = scanner.PullOneToken();
                // iden
                if (currentToken.type != TokenType.IDENTIFIER)
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of identifier here"));
                    scanner.PushOneToken(currentToken);
                    return null;
                }
                idens.Add(currentToken);

                currentToken = scanner.PullOneToken();
            }

            // :
            if (currentToken.type != TokenType.COLON)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of : here"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // type
            idenType = (PLType)new PLType().TryBuild(ref scanner);
            if (idenType == null)
                return null;

            return this;
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
