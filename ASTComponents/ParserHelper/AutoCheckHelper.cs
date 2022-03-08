using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class AutoCheckHelper
    {
        // TODO: check the value and possibly report errors
        // Like this kind of thing:

        /*
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.FOR)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Lack of \"for\" here"));
                SkipHelper.SkipToSemi(ref scanner, currentToken);
                return null;
            }
         */
    }
}
