using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    class Scanner
    {
        private CharacterHandler cStream;
        private List<Token> tokens = new List<Token>(); // TODO: list->stack and return the stack to caller
        private int lineCnt = 0;    // TODO: do it later

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
            Tokenize();
            Console.WriteLine(String.Format("Tokenization completed. {0} tokens.", tokens.ToArray().Length));
        }

        // get tokens (result of the scanner)
        private void Tokenize()
        {
            char current_char = cStream.PullOne();
            while(current_char != '\0')
            {
                // one character -> one token (ops, some symbols) (not ambigious ones)
                if (singleSymbol.ContainsKey(current_char))
                {
                    tokens.Add(new Token(singleSymbol[current_char], current_char.ToString(), lineCnt));
                }
                //: & :=
                else if (current_char == ':')
                {
                    HandleStartWithColon();
                }
                // ..
                else if (current_char == '.')
                {
                    HandleStartWithDot();
                }
                // comments (and maybe div /)
                else if (current_char == '/')
                {
                    if (!HandleStartWithDiv())
                        break;
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
        }


        // deal with start with "."
        private void HandleStartWithDot()
        {
            // preread
            char current_char = cStream.PullOne();
            if (current_char == '.')
            {
                tokens.Add(new Token(TokenType.TO, "..", lineCnt));
            }
            else
            {
                cStream.PushOne();
                Console.WriteLine("ERROR"); // TODO: report error
            }
        }

        // deal with start with ":"
        private void HandleStartWithColon()
        {
            // preread
            char current_char = cStream.PullOne();
            if (current_char == '=')
            {
                tokens.Add(new Token(TokenType.ASSIGN, ":=", lineCnt));
            }
            else
            {
                tokens.Add(new Token(TokenType.COLON, ":", lineCnt));
                // push the preread character back
                cStream.PushOne();
            }
        }


        // deal with start with "/"
        // return true means ok
        // return false means encounter '\0'
        private bool HandleStartWithDiv()
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
                if (current_char == '\0')
                    return false;
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
                if (current_char == '\0')
                    return false;  // end scanner
            }
            // just div
            else
            {
                // return preread
                cStream.PushOne();
                tokens.Add(new Token(TokenType.OPERATOR, "/", lineCnt));
            }
            return true;
        }
    }
}
