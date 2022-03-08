﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class PrintStat : BaseNode
    {
        // valid: print expression
        public Expr expression;


        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            if (currentToken.type == TokenType.PRINT)
            {
                // expression
                expression = (Expr)new Expr().TryBuild(ref scanner);
                if (expression == null)
                {
                    // skip
                    SkipHelper.SkipToSemi(ref scanner);
                    return null;
                }
                // ;
                Token nextToken = scanner.PullOneToken();
                if (nextToken.type != TokenType.SEMICOLON)
                {
                    ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of ; here"));
                    // push back this token, continue the program (hopefully ignoring the error and continue
                    scanner.PushOneToken(nextToken);
                    return this;
                }
                else
                {
                    return this;
                }
            }
            else
            {
                return null;
            }
        }
    }
}