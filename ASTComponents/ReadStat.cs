using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class ReadStat : BaseNode
    {
        public List<Variable> vars = new List<Variable>();

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            Token currentToken = scanner.PullOneToken();
            // read
            if (currentToken.type != TokenType.READ)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect keyword read"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // (
            currentToken = scanner.PullOneToken();
            if (currentToken.type != TokenType.LEFT_BRACKET)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ("));
                scanner.PushOneToken(currentToken);
                return null;
            }

            // variable
            Variable tmp = (Variable)new Variable().TryBuild(ref scanner);
            if (tmp == null)
                return null;
            vars.Add(tmp);

            // , variable
            currentToken = scanner.PullOneToken();
            while(currentToken.type == TokenType.COMMA)
            {
                Variable tmpv = (Variable)new Variable().TryBuild(ref scanner);
                if (tmpv == null)
                    return null;
                vars.Add(tmpv);
                currentToken = scanner.PullOneToken();
            }

            // )
            if (currentToken.type != TokenType.RIGHT_BRACKET)
            {
                ErrorHandler.PushError(new MyError(currentToken.lexeme, currentToken.lineNum, "Expect ) here"));
                scanner.PushOneToken(currentToken);
                return null;
            }

            return this;

        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitRead(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeRead(this);
        }
    }
}
