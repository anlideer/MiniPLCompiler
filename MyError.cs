using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    class MyError
    {
        public string errorSource;
        public int errorLine;
        public string errorMsg;

        public MyError(string eSource, int eLine, string eMsg)
        {
            errorSource = eSource;
            errorLine = eLine;
            errorMsg = eMsg;
        }
    }
}
