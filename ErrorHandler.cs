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

        public static bool IsEmpty()
        {
            if (errorList.Count == 0)
                return true;
            else
                return false;
        }

        // print errors
        public static void PrintErrors()
        {
            foreach(MyError e in errorList)
            {
                e.PrintError();
            }
        }

        // empty
        public static void ClearAll()
        {
            errorList.Clear();
        }
    }
}
