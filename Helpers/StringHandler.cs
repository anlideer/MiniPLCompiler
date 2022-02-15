using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    class StringHandler
    {
        // scan the string by character
        public static string ScanString(ref CharacterHandler cStream)
        {
            Dictionary<char, string> ESCAPE = new Dictionary<char, string>{
                {'a', "\a" },
                {'b', "\b" },
                {'f', "\f" },
                {'n', "\n" },
                {'r', "\r" },
                {'t', "\t" },
                {'v', "\v" },
                {'\'', "\'" },
                {'\"', "\"" },
                {'\\', "\\" },
            };
            string res = "";
            char current_char = cStream.PullOne();
            while(current_char != '\0')
            {
                if (current_char != '"' && current_char != '\\')
                {
                    res += current_char.ToString();
                }
                else if (current_char == '\\')
                {
                    // preread one more
                    current_char = cStream.PullOne();
                    if (current_char == '\0')
                        break;
                    if (ESCAPE.ContainsKey(current_char))
                    {
                        res += ESCAPE[current_char];
                    }
                    else
                    {
                        res += "\\" + current_char.ToString();
                    }
                }
                // "
                else
                {
                    cStream.PushOne();
                    break;
                }
                current_char = cStream.PullOne();
            }
            return res;
        }
    }
}
