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
            { '+', TokenType.ADD_OPERATOR}, { '-', TokenType.ADD_OPERATOR}, 
            { '*', TokenType.MULTIPLY_OPERATOR}, { ';', TokenType.SEMICOLON}, { '%', TokenType.MULTIPLY_OPERATOR},
            { '=', TokenType.RELATIONAL_OPERATOR},
            { '(', TokenType.LEFT_BRACKET}, { ')', TokenType.RIGHT_BRACKET},
            { '[', TokenType.LEFT_SBRACKET }, { ']', TokenType.RIGHT_SBRACKET},
            { ',', TokenType.COMMA}, { '.', TokenType.DOT},
        };
        // keywords
        private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "integer", TokenType.INT_TYPE},
            { "string", TokenType.STRING_TYPE},
            { "Boolean", TokenType.BOOL_TYPE},
            { "real", TokenType.REAL_TYPE},
            { "var", TokenType.VAR},
            { "end", TokenType.END},
            { "do", TokenType.DO},
            { "read", TokenType.READ},
            { "writeln", TokenType.WRITELN},
            { "assert", TokenType.ASSERT},
            { "or", TokenType.OR},
            { "and", TokenType.AND},
            { "not", TokenType.NOT},
            { "if", TokenType.IF},
            { "then", TokenType.THEN},
            { "else", TokenType.ELSE},
            { "of", TokenType.OF},
            { "while", TokenType.WHILE},
            { "begin", TokenType.BEGIN},
            { "array", TokenType.ARRAY},
            { "procedure", TokenType.PROCEDURE},
            { "function", TokenType.FUNCTION},
            { "program", TokenType.PROGRAM},
            { "return", TokenType.RETURN},
            { "false", TokenType.BOOL_VAL},
            { "true", TokenType.BOOL_VAL},
            { "size", TokenType.SIZE},

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
                //: & :=
                else if (current_char == ':')
                {
                    return HandleStartWithColon();
                }
                // comments (and maybe div /)
                else if (current_char == '/')
                {
                    Token t = HandleStartWithDiv();
                    if (t != null)
                        return t;
                }
                // start with <
                else if (current_char == '<')
                {
                    return HandleStartWithSmaller();
                }
                // start with >
                else if (current_char == '>')
                {
                    return HandleStartWithBigger();
                }
                // int value
                else if (current_char >= '0' && current_char <= '9')
                {
                    Token t = HandleNumber(current_char);
                    if (t != null)
                        return t;
                }
                // string value
                else if (current_char == '"')
                {
                    string s = StringHandler.ScanString(ref cStream);
                    current_char = cStream.PullOne();
                    // should be '"', if '\0' or '\n' then string incomplete
                    if (current_char == '\0' || current_char == '\n')
                    {
                        // report error
                        Token errorT = new Token(TokenType.ERROR, "\"" + s, lineCnt);
                        MyError e = new MyError("\"" + s, lineCnt, "Lack of the end symbol of string");
                        ErrorHandler.PushError(e);
                        lineCnt++;
                        return errorT;
                    }
                    else
                    {
                        return new Token(TokenType.STRING_VAL, s, lineCnt);
                    }
                }
                // identifier or keyword
                else if ((current_char >= 'a' && current_char <= 'z') ||
                (current_char >= 'A' && current_char <= 'Z'))
                {
                    Token t = HandleIdorKeyword(current_char);
                    if (t != null)
                        return t;
                }
                // {*...*} comments
                else if (current_char == '{')
                {
                    cStream.PushOne();
                    HandleComments();
                }
                else
                {
                    ErrorHandler.PushError(new MyError(current_char.ToString(), lineCnt, "Unacceptable token."));
                }

                current_char = cStream.PullOne();
            }

            // if '\0'
            return new Token(TokenType.END_OF_PROGRAM, "", lineCnt);
        }


        // handle multiple lines comments
        private void HandleComments()
        {
            char last_char = cStream.PullOne();
            char current_char = cStream.PullOne();
            //*...*}
            if (current_char != '*')
            {
                ErrorHandler.PushError(new MyError(last_char.ToString(), lineCnt, "Unacceptable token"));
                cStream.PushOne();
                return;
            }

            // skip to *}
            current_char = cStream.PullOne();
            while (true)
            {
                if (current_char == '*')
                {
                    current_char = cStream.PullOne();
                    if (current_char == '}')
                        break;
                }
                else
                {
                    current_char = cStream.PullOne();
                }
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
            // just div
            else
            {
                // return preread
                cStream.PushOne();
                return new Token(TokenType.MULTIPLY_OPERATOR, "/", lineCnt);
            }
            return null;
        }

        // integer / real
        private Token HandleNumber(char current_char)
        {
            string digits = ReadDigits(current_char);
            current_char = cStream.PullOne();
            // real
            if (current_char == '.')
            {
                bool errorFlag = false;
                string res = digits + ".";
                current_char = cStream.PullOne();
                digits = ReadDigits(current_char);
                if (digits.Length == 0)
                {
                    ErrorHandler.PushError(new MyError(current_char.ToString(), lineCnt, "Need some digits after dot ."));
                    errorFlag = true;
                }
                res += digits;
                current_char = cStream.PullOne();
                // optional [e...]
                if (current_char == 'e')
                {
                    res += "e";
                    // optional [sign]
                    current_char = cStream.PullOne();
                    if (current_char == '+' || current_char == '-')
                    {
                        res += current_char.ToString();
                        current_char = cStream.PullOne();
                    }

                    // digits (must exist)
                    digits = ReadDigits(current_char);
                    if (digits.Length == 0)
                    {
                        ErrorHandler.PushError(new MyError(current_char.ToString(), lineCnt, "Need some digits if you use 'e' "));
                        errorFlag = true;
                    }
                    res += digits;

                    if (errorFlag)
                        return new Token(TokenType.ERROR, res, lineCnt);
                    else
                        return new Token(TokenType.REAL_VAL, res, lineCnt);
                }
                else
                {
                    cStream.PushOne();
                    return new Token(TokenType.REAL_VAL, res, lineCnt);
                }
            }
            // int
            else
            {
                cStream.PushOne();
                return new Token(TokenType.INT_VAL, digits, lineCnt);
            }
            
        }

        // get <digits>
        private string ReadDigits(char current_char)
        {
            string tmp = "";
            while (current_char >= '0' && current_char <= '9')
            {
                tmp += current_char.ToString();
                current_char = cStream.PullOne();
            }
            cStream.PushOne();
            return tmp;
        }

        // identifier or keyword
        private Token HandleIdorKeyword(char current_char)
        {
            string tmp = "";
            while ((current_char >= 'a' && current_char <= 'z') ||
                (current_char >='A' && current_char <= 'Z') || 
                (current_char >= '0' && current_char <= '9') ||
                current_char == '_')
            {
                tmp += current_char.ToString();
                current_char = cStream.PullOne();
            }
            cStream.PushOne();
            if (keywords.ContainsKey(tmp))
            {
                return new Token(keywords[tmp], tmp, lineCnt);
            }
            else
            {
                return new Token(TokenType.IDENTIFIER, tmp, lineCnt);
            }
        }

        // handle start with <
        private Token HandleStartWithSmaller()
        {
            char next_char = cStream.PullOne();
            if (next_char == '>')
                return new Token(TokenType.RELATIONAL_OPERATOR, "<>", lineCnt);
            else if (next_char == '=')
                return new Token(TokenType.RELATIONAL_OPERATOR, "<=", lineCnt);
            else
            {
                // just <
                cStream.PushOne();
                return new Token(TokenType.RELATIONAL_OPERATOR, "<", lineCnt);
            }
        }

        // handle start with >
        private Token HandleStartWithBigger()
        {
            char next_char = cStream.PullOne();
            if (next_char == '=')
                return new Token(TokenType.RELATIONAL_OPERATOR, ">=", lineCnt);
            else
            {
                // just >
                cStream.PushOne();
                return new Token(TokenType.RELATIONAL_OPERATOR, ">", lineCnt);
            }
        }

    }
}
