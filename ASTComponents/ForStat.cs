using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class ForStat : BaseNode
    {
        public Token iden;
        public Expr left;
        public Expr right;
        public Statements stats;

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // for
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.FOR) // won't happen if program goes correctly
                return null;

            // iden
            currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.IDENTIFIER)
            {
                iden = currentToken;
            }
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of identifier here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // in
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.IN)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of \"in\" here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // expr
            left = (Expr)new Expr().TryBuild(ref scanner);
            if (left == null)
            {
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // ..
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.TO)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of \"..\" here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // expr
            right = (Expr)new Expr().TryBuild(ref scanner);
            if (right == null)
            {
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // do
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.DO)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of \"do\" here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // statements
            stats = (Statements)new Statements().TryBuild();
            if (stats == null)
                return null;

            // end
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.END)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of \"end\" here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // for
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.FOR)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of \"for\" here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }

            // ;
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.SEMICOLON)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Need ;"));
                scanner.PushOneToken(currentToken);
                return this;
            }

            return this;
        }
    }
}
