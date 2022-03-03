using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    enum ComponentType
    {
        PROG, DEF_STAT, ASSIGN_STAT, LOOP_STAT, READ_STAT, PRINT_STAT, ASSERT_STAT, EXPR, 
        TYPE, IDENT, OP, UNARY_OP, OPND, 
        EMPTY,
    }

    class AST
    {
        ComponentType rootType;
        List<AST> children;
        Token token;

        public AST (ComponentType type, Token t)
        {
            rootType = type;
            token = t;
        }

        // add a child to this
        public void AddChild(AST ast)
        {
            children.Add(ast);
        }
    }
}
