using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class PLProgram : BaseNode
    {
        public Token iden;
        public List<Func> funcs = new List<Func>();
        public Block mainBlock;

        public override void Accept(Visitor visitor)
        {
            visitor.VisitProgram(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            ///// starting part
            // program
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.PROGRAM)
            {
                // id
                currentToken = scanner.PullOneToken();
                if (currentToken.type == TokenType.IDENTIFIER)
                {
                    iden = currentToken;

                    // ;
                    currentToken = scanner.PullOneToken();
                    if (currentToken.type != TokenType.SEMICOLON)
                    {
                        ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ;"));
                        // tolerant. do nothing
                    }
                }
                else
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect identifier"));
                    SkipHelper.SkipToSemi(ref scanner);
                }
            }
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "The program should start with keyword program"));
                SkipHelper.SkipToSemi(ref scanner);
                // continue analyze
            }

            ///// funcs and procedures part
            currentToken = scanner.PullOneToken();
            while (currentToken.type != TokenType.BEGIN && currentToken.type != TokenType.END_OF_PROGRAM)
            {
                if (currentToken.type == TokenType.FUNCTION || currentToken.type == TokenType.PROCEDURE)
                {
                    scanner.PushOneToken(currentToken);
                    var func = (Func)new Func().TryBuild(ref scanner);
                    if (func == null)
                    {
                        // skip to begin/func/proce
                        SkipHelper.SkipTo(ref scanner, new HashSet<TokenType> { TokenType.BEGIN, TokenType.FUNCTION, TokenType.PROCEDURE });
                    }
                    else
                    {
                        funcs.Add(func);
                    }
                }
            }
            scanner.PushOneToken(currentToken);

            ///// block part
            mainBlock = (Block)new Block().TryBuild(ref scanner);
            // dot
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.DOT)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect dot . "));
            }

            // if still have content, then error
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.END_OF_PROGRAM)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Unnecessary contents after main block."));
                while (currentToken.type != TokenType.END_OF_PROGRAM)
                    currentToken = scanner.PullOneToken();
            }

            return this;

        }
    }
}
