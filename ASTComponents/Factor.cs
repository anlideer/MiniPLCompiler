using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class Factor : BaseNode
    {
        // whether node or literal
        public BaseNode node;   
        public Token literal;
        // optional
        public Token ifNot;
        public Token ifSize;


        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            // [not]
            if (currentToken.type == TokenType.NOT)
            {
                ifNot = currentToken;
                // factor
                node = new Factor().TryBuild(ref scanner);
                if (node == null)
                    return null;
            }

            // id
            if (currentToken.type == TokenType.IDENTIFIER)
            {
                // preread once again
                Token nextToken = scanner.PullOneToken();
                // call
                if (nextToken.type == TokenType.LEFT_BRACKET)
                {
                    scanner.PushOneToken(nextToken);
                    scanner.PushOneToken(currentToken);
                    node = new CallStat().TryBuild(ref scanner);
                    if (node == null)
                        return null;
                }
                // variable
                else
                {
                    scanner.PushOneToken(nextToken);
                    scanner.PushOneToken(currentToken);
                    node = new Variable().TryBuild(ref scanner);
                    if (node == null)
                        return null;
                }    
            }
            // literal
            else if (currentToken.type == TokenType.REAL_VAL || currentToken.type == TokenType.INT_VAL 
                || currentToken.type == TokenType.STRING_VAL || currentToken.type == TokenType.BOOL_VAL)
            {
                literal = currentToken;
            }
            // (expr)
            else if (currentToken.type == TokenType.LEFT_BRACKET)
            {
                scanner.PushOneToken(currentToken);
                node = new Expr().TryBuild(ref scanner);
                if (node == null)
                    return null;
            }
            // error
            else
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Unacceptable token for building <factor>"));
                return null;
            }

            // optional [.size]
            currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.DOT)
            {
                Token nextToken = scanner.PullOneToken();
                if (nextToken.type == TokenType.SIZE)
                {
                    ifSize = nextToken;
                }
                else
                {
                    ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect size after dot . "));
                    scanner.PushOneToken(nextToken);
                    return null;
                }
            }
            else
            {
                scanner.PushOneToken(currentToken);
            }


            return this;
        }

        public override void Accept(Visitor visitor)
        {
            throw new NotImplementedException();
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }


    }
}
