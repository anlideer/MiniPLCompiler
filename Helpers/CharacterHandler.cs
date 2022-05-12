using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MiniPLCompiler
{
    class CharacterHandler
    {
        private string sourceStr = "";
        private int currentInd = 0;

        public CharacterHandler(string filePath)
        {
            sourceStr = File.ReadAllText(filePath);
            sourceStr = sourceStr.ToLower();
        }

        // read one character forward
        public char PullOne()
        {
            char c;
            if (currentInd < sourceStr.Length)
                c = sourceStr[currentInd]; 
            else
                c =  '\0';    // safe to call for many extra times
            currentInd++;
            return c;
        }

        // return one character back
        public void PushOne()
        {
            currentInd--;
            if (currentInd < 0)
                currentInd = 0;
        }
    }
}
