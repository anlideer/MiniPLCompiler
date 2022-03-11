using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    class Statements : BaseNode
    {
        public List<BaseNode> statsList = new List<BaseNode>();

        public override BaseNode TryBuild(ref Scanner scanner)
        {
            // preread
            Token token = scanner.PullOneToken();
            while (token.type != TokenType.END_OF_PROGRAM)
            {
                // def
                if (token.type == TokenType.VAR)
                {
                    scanner.PushOneToken(token);
                    BaseNode node = new DefStat().TryBuild(ref scanner);
                    if (node != null)
                        statsList.Add(node);
                }
                // assign
                else if (token.type == TokenType.IDENTIFIER)
                {
                    scanner.PushOneToken(token);
                    BaseNode node = new AssignStat().TryBuild(ref scanner);
                    if (node != null)
                        statsList.Add(node);
                }
                // for loop
                else if (token.type == TokenType.FOR)
                {
                    scanner.PushOneToken(token);
                    BaseNode node = new ForStat().TryBuild(ref scanner);
                    if (node != null)
                        statsList.Add(node);
                }
                // read
                else if (token.type == TokenType.READ)
                {
                    scanner.PushOneToken(token);
                    BaseNode node = new ReadStat().TryBuild(ref scanner);
                    if (node != null)
                        statsList.Add(node);
                }
                // print
                else if (token.type == TokenType.PRINT)
                {
                    scanner.PushOneToken(token);
                    BaseNode node = new PrintStat().TryBuild(ref scanner);
                    if (node != null)
                        statsList.Add(node);
                }
                // assert
                else if (token.type == TokenType.ASSERT)
                {
                    scanner.PushOneToken(token);
                    BaseNode node = new AssertStat().TryBuild(ref scanner);
                    if (node != null)
                        statsList.Add(node);
                }
                // error
                else
                {
                    scanner.PushOneToken(token);
                    return this;
                }

                token = scanner.PullOneToken();
            }

            return this;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitStatements(this);
        }

        public override void AcceptExe(SimpleInterpreter interpreter)
        {
            interpreter.ExeStats(this);
        }
    }
}
