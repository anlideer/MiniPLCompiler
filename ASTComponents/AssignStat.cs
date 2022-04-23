using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class AssignStat : BaseNode
    {
        public Variable variable;
        public Expr expression;

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // variable
            variable = (Variable)new Variable().TryBuild(ref scanner);
            if (variable == null)
                return null;

            // :=
            Token nextToken = scanner.PullOneToken();
            if (nextToken.type != TokenType.ASSIGN)
            {
                ErrorHandler.PushError(new MyError(nextToken.lexeme, nextToken.lineNum, "Lack of := here"));
                return null;
            }

            // expr
            expression = (Expr)new Expr().TryBuild(ref scanner);
            if (expression == null)
                return null;

            return this;

        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitAssign(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeAssign(this);
        }

    }
}
