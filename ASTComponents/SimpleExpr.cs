using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPLCompiler.ASTComponents
{
    class SimpleExpr : BaseNode
    {
        public Token sign;  // optional
        public Term term;
        public List<Token> ops = new List<Token>();
        public List<Term> terms = new List<Term> ();

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
            Token currentToken = scanner.PullOneToken();
            
            // [sign]
            if (currentToken.type == TokenType.ADD_OPERATOR)
            {
                sign = currentToken;
            }
            else
            {
                scanner.PushOneToken(currentToken);
            }

            // term
            term = (Term)new Term().TryBuild(ref scanner);
            if (term == null)
                return term;

            // {op term}
            currentToken = scanner.PullOneToken();
            while (currentToken.type == TokenType.ADD_OPERATOR || currentToken.type == TokenType.OR)
            {
                ops.Add(currentToken);
                Term tmp = (Term)new Term().TryBuild(ref scanner);
                if (tmp == null)
                    return tmp;
                terms.Add(tmp);
            }
            scanner.PushOneToken(currentToken);

            return this;
        }
    }
}
