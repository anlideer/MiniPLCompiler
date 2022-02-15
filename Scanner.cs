using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    class Scanner
    {
        private CharacterHandler cStream;
        private List<Token> tokens;
        private int lineCnt = 0;
        
        // init
        public Scanner(string filePath)
        {
            cStream = new CharacterHandler(filePath);
            Tokenize();
            Console.WriteLine(tokens);
        }

        // get tokens (result of the scanner)
        private void Tokenize()
        {

        }
    }
}
