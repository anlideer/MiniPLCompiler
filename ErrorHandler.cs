using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    static class ErrorHandler
    {
        private static List<MyError> errorList = new List<MyError>();

        public static void PushError(MyError e)
        {
            errorList.Add(e);
        }
    }
}
