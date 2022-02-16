using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    class Scanner
    {
        private CharacterHandler cStream;
        private int lineCnt = 0;    // TODO: do it later
        private Stack<Token> tokenStack = new Stack<Token>();

        // One character for one token (not ambigious)
        private Dictionary<char, TokenType> singleSymbol = new Dictionary<char, TokenType> {
            { '+', TokenType.OPERATOR}, { '-', TokenType.OPERATOR},
            { '*', TokenType.OPERATOR}, { ';', TokenType.SEMICOLON},
            { '&', TokenType.OPERATOR}, { '=', TokenType.OPERATOR},
            { '<', TokenType.OPERATOR}, { '!', TokenType.UNARY_OPERATOR},
            { '(', TokenType.LEFT_BRACKET}, { ')', TokenType.RIGHT_BRACKET},
        };
        
        // init
        public Scanner(string filePath)
        {
            cStream = new CharacterHandler(filePath);
        }

        // get one token
        public Token PullOneToken()
        {
            if (tokenStack.Count > 0)
                return tokenStack.Pop();
            else
                return FindNextToken();
        }

        // return one token
        public void PushOneToken(Token token)
        {
            tokenStack.Push(token);
        }

        // get the next token
        private Token FindNextToken()
        {
            char current_char = cStream.PullOne();
            while(current_char != '\0')
            {
                // one character -> one token (ops, some symbols) (not ambigious ones)
                if (singleSymbol.ContainsKey(current_char))
                {
                    return new Token(singleSymbol[current_char], current_char.ToString(), lineCnt);
                }
                //: & :=
                else if (current_char == ':')
                {
                    return HandleStartWithColon();
                }
                // ..
                else if (current_char == '.')
                {
                    Token t =  HandleStartWithDot();
                    if (t != null)
                        return t;
                    // if null report the error and continue to find the next token
                }
                // comments (and maybe div /)
                else if (current_char == '/')
                {
                    Token t =  HandleStartWithDiv();
                    if (t != null)
                        return t;
                }
                // space (ignore
                else if (current_char == ' ' || current_char == '\t' || current_char == '\r' || current_char == '\f' || current_char == '\v')
                {
                    // pass
                }
                // \n (ignore but lineCnt++
                else if (current_char == '\n')
                {
                    lineCnt++;
                }
                // TODO: to be continue...


                current_char = cStream.PullOne();
            }

            // if '\0'
            return new Token(TokenType.END_OF_PROGRAM, "", lineCnt);
        }


        // deal with start with "."
        private Token HandleStartWithDot()
        {
            // preread
            char current_char = cStream.PullOne();
            if (current_char == '.')
            {
                return new Token(TokenType.TO, "..", lineCnt);
            }
            else
            {
                cStream.PushOne();
                Console.WriteLine("ERROR"); // TODO: report error
                return null;
            }
        }

        // deal with start with ":"
        private Token HandleStartWithColon()
        {
            // preread
            char current_char = cStream.PullOne();
            if (current_char == '=')
            {
                return new Token(TokenType.ASSIGN, ":=", lineCnt);
            }
            else
            {
                // push the preread character back
                cStream.PushOne();
                return new Token(TokenType.COLON, ":", lineCnt);
            }
        }


        // deal with start with "/"
        private Token HandleStartWithDiv()
        {
            // preread
            char current_char = cStream.PullOne();
            if (current_char == '/')
            {
                // ignore whole line
                while (current_char != '\n' && current_char != '\0')
                {
                    current_char = cStream.PullOne();
                }
                lineCnt++;
            }
            else if (current_char == '*')
            {
                // ignore until */
                while (current_char != '\0')
                {
                    current_char = cStream.PullOne();
                    if (current_char == '*')
                    {
                        // preread again
                        current_char = cStream.PullOne();
                        if (current_char == '/')
                            break;  // end of comment
                        else
                            cStream.PushOne();
                    }
                    else if (current_char == '\n')
                        lineCnt++;
                    // else pass
                }
            }
            // just div
            else
            {
                // return preread
                cStream.PushOne();
                return new Token(TokenType.OPERATOR, "/", lineCnt);
            }
            return null;
        }
    }
}
