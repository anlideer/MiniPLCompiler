using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    class SkipHelper
    {
        // skip to semi colon (or end of program)
        public static void SkipToSemi(ref Scanner scanner, Token nextToken)
        {
            // directly skip to ;
            while (nextToken.type != TokenType.SEMICOLON && nextToken.type != TokenType.END_OF_PROGRAM)
            {
                nextToken = scanner.PullOneToken();
            }
        }
        public static void SkipToSemi(ref Scanner scanner)
        {
            Token nextToken = scanner.PullOneToken();
            // directly skip to ;
            while (nextToken.type != TokenType.SEMICOLON && nextToken.type != TokenType.END_OF_PROGRAM)
            {
                nextToken = scanner.PullOneToken();
            }
        }


    }
}
