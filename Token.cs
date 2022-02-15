using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    enum TokenType {
        INT_VAL, STRING_VAL, INT_TYPE, STRING_TYPE, BOOL_TYPE, IDENTIFIER,
        OPERATOR, UNARY_OPERATOR, VAR, FOOR, END, IN, DO, READ, PRINT, ASSERT,
        SEMICOLON, // ;
        COLON, // ,
        EQUAL, // :=
        TO, // ..
        LEFT_BRACKET, RIGHT_BRACKET, // ( )
    };
    class Token
    {
        public TokenType type;
        public string lexeme;
        public int lineNum;

        public Token(TokenType _type, string _lexeme, int _lineNum)
        {
            type = _type;
            lexeme = _lexeme;
            lineNum = _lineNum;
        }
    }
}
