using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class Term : BaseNode
    {
        public Factor factor;
        public List<Token> ops = new List<Token> ();
        public List<Factor> facs = new List<Factor>();

        public override void Accept(Visitor visitor)
        {
            throw new NotImplementedException();
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            throw new NotImplementedException();
        }

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // factor
            factor = (Factor) new Factor().TryBuild(ref scanner);
            if (factor == null)
                return null;

            // {op fac}
            Token currentToken = scanner.PullOneToken();
            while (currentToken.type == TokenType.MULTIPLY_OPERATOR || currentToken.type == TokenType.AND)
            {
                ops.Add(currentToken);
                // factor
                Factor tmp = (Factor)new Factor().TryBuild(ref scanner);
                if (tmp == null)
                    return null;
                facs.Add(tmp);

                currentToken = scanner.PullOneToken();
            }
            scanner.PushOneToken(currentToken);

            return this;

        }
    }
}
