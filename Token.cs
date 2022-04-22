using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    enum TokenType {
        INT_VAL, STRING_VAL, REAL_VAL,
        FALSE, TRUE,
        AND, OR, NOT,
        INT_TYPE, STRING_TYPE, BOOL_TYPE, REAL_TYPE,
        IDENTIFIER,
        ADD_OPERATOR,   // +, -
        MULTIPLY_OPERATOR, // *, /, %
        RELATIONAL_OPERATOR, // =, <>, <, >, <=, >=
        IF, THEN, ELSE, OF, WHILE, DO, BEGIN, END, VAR,
        ARRAY, PROCEDURE, FUNCTION, PROGRAM, ASSERT, RETURN,
        READ, WRITELN, SIZE,
        LEFT_SBRACKET, RIGHT_SBRACKET, // []
        LEFT_BRACKET, RIGHT_BRACKET, // ( )
        DOT, // .
        COMMA, // ,
        SEMICOLON, // ;
        COLON, // :
        ASSIGN, // :=

        // special
        END_OF_PROGRAM,
        ERROR, // special token indicating error
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
