using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class SkipHelper
    { 
        // general skip
        // but leave the token here
        public static void SkipTo(ref Scanner scanner, TokenType t)
        {
            Token nextToken = scanner.PullOneToken();
            while (nextToken.type != t && nextToken.type != TokenType.END_OF_PROGRAM)
            {
                nextToken = scanner.PullOneToken();
            }
            scanner.PushOneToken(nextToken);
        }
        public static void SkipTo(ref Scanner scanner, HashSet<TokenType> ts)
        {
            Token nextToken = scanner.PullOneToken();
            while ( !ts.Contains(nextToken.type) && nextToken.type != TokenType.END_OF_PROGRAM)
            {
                nextToken = scanner.PullOneToken();
            }
            scanner.PushOneToken(nextToken);
        }

        // skip to ; or .
        // but still leave the last token
        public static void SkipToSemiOrDot(ref Scanner scanner)
        {
            Token nextToken = scanner.PullOneToken();
            // directly skip to ;
            while (nextToken.type != TokenType.SEMICOLON && nextToken.type != TokenType.DOT && nextToken.type != TokenType.END_OF_PROGRAM )
            {
                nextToken = scanner.PullOneToken();
            }
            scanner.PushOneToken(nextToken);
        }

        // skip to semi colon or end
        // but leave the last token in the scanner
        public static void SkipToSemiOrEnd(ref Scanner scanner)
        {
            Token nextToken = scanner.PullOneToken();
            // directly skip to ;
            while (nextToken.type != TokenType.SEMICOLON && nextToken.type != TokenType.END_OF_PROGRAM && nextToken.type != TokenType.END)
            {
                nextToken = scanner.PullOneToken();
            }
            scanner.PushOneToken(nextToken);
        }


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
