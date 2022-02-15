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
        }

        // read one character forward
        public char PullOne()
        {
            currentInd++;
            if (currentInd < sourceStr.Length)
                return sourceStr[currentInd];
            else
                return '\0';    // safe to call for many extra times
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
