using System;
using System.Collections.Generic;
using System.Text;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    // a very simple and naive implementation of interpreter
    class SimpleInterpreter
    {
        private Visitor visitor;

        private Dictionary<string, int> intDic = new Dictionary<string, int>(); // int and bool
        private Dictionary<string, string> strDic = new Dictionary<string, string>();   // string
        private Dictionary<Opnd, int> opndIntDic = new Dictionary<Opnd, int>();
        private Dictionary<Opnd, string> opndStrDic = new Dictionary<Opnd, string>();
        private Dictionary<Expr, int> exprIntDic = new Dictionary<Expr, int>();
        private Dictionary<Expr, string> exprStrDic = new Dictionary<Expr, string>();


        public SimpleInterpreter(Visitor v)
        {
            visitor = v;
        }

        // still use the visitor pattern

        // statements
        public void ExeStats(Statements stats)
        {
            /*
            foreach(BaseNode b in stats.statsList)
            {
                b.AcceptExe(this);
            }
            */
        }

        // opnd
        public void ExeOpnd (Opnd opnd)
        {
            /*
            if (opnd.selfToken != null)
            {
                if (opnd.selfToken.type == TokenType.INT_VAL)
                {
                    opndIntDic[opnd] = int.Parse(opnd.selfToken.lexeme);
                    return;
                }
                else if (opnd.selfToken.type == TokenType.STRING_VAL)
                {
                    opndStrDic[opnd] = opnd.selfToken.lexeme;
                    return;
                }
                else
                {
                    if (visitor.varTypeDic[opnd.selfToken.lexeme] == IdenType.BOOL || visitor.varTypeDic[opnd.selfToken.lexeme] == IdenType.INT)
                    {
                        opndIntDic[opnd] = intDic[opnd.selfToken.lexeme];
                        return;
                    }
                    else
                    {
                        opndStrDic[opnd] = strDic[opnd.selfToken.lexeme];
                        return;
                    }
                }
            }
            // expressoin
            else
            {
                ExeExpr(opnd.expression);
                if (visitor.opTypeDic[opnd] == IdenType.STRING)
                {
                    opndStrDic[opnd] = exprStrDic[opnd.expression];
                }
                else
                {
                    opndIntDic[opnd] = exprIntDic[opnd.expression];
                }
            }
            */
        }

        // expression
        public void ExeExpr (Expr expression)
        {
            /*
            // left
            if (expression.op == null)
            {
                ExeOpnd(expression.left);
                if (visitor.exprTypeDic[expression] == IdenType.STRING)
                    exprStrDic[expression] = opndStrDic[expression.left];
                else
                    exprIntDic[expression] = opndIntDic[expression.left];
            }
            // unary_op + right
            else if (expression.left == null)
            {
                ExeOpnd(expression.right);
                if (opndIntDic[expression.right] == 0)
                    exprIntDic[expression] = 1;
                else
                    exprIntDic[expression] = 0;
            }
            // left + op + right
            else
            {
                ExeOpnd(expression.left);
                ExeOpnd(expression.right);
                if (expression.op.lexeme == "+")
                {
                    if (visitor.exprTypeDic[expression] == IdenType.INT)
                        exprIntDic[expression] = opndIntDic[expression.left] + opndIntDic[expression.right];
                    else
                        exprStrDic[expression] = opndStrDic[expression.left] + opndStrDic[expression.right];
                }
                else if (expression.op.lexeme == "&")
                {
                    if (opndIntDic[expression.left] == opndIntDic[expression.right])
                        exprIntDic[expression] = 1;
                    else
                        exprIntDic[expression] = 0;
                }
                else
                {
                    int leftv = opndIntDic[expression.left];
                    int rightv = opndIntDic[expression.right];
                    if (expression.op.lexeme == "/")
                    {
                        // division by zero error (but still execute it
                        if (rightv == 0)
                            throw new Exception("Division by zero error, line: " + expression.op.lineNum.ToString());
                        exprIntDic[expression] = leftv / rightv;
                    }
                    else if (expression.op.lexeme == "*")
                        exprIntDic[expression] = leftv * rightv;
                    else if (expression.op.lexeme == "-")
                        exprIntDic[expression] = leftv - rightv;
                    else if (expression.op.lexeme == "<")
                        exprIntDic[expression] = (leftv < rightv) ? 1 : 0;
                    else if (expression.op.lexeme == "=")
                        exprIntDic[expression] = (leftv == rightv) ? 1 : 0;
                    else
                        Console.WriteLine("Attention! Unimplemented op: " + expression.op.lexeme);
                }
            }
            */
        }

        // print
        public void ExePrint(PrintStat p)
        {
            /*
            ExeExpr(p.expression);
            if (visitor.exprTypeDic[p.expression] == IdenType.STRING)
                Console.Write(exprStrDic[p.expression]);
            else
                Console.Write(exprIntDic[p.expression]);
            */
        }

        // read
        public void ExeRead(ReadStat r)
        {
            /*
            Token t = r.iden;
            string tmps = Console.ReadLine();
            // string
            if (visitor.varTypeDic[t.lexeme] == IdenType.STRING)
            {
                strDic[t.lexeme] = tmps;
                return;
            }
            // int/bool
            int tmpi;
            try
            {
                tmpi = int.Parse(tmps);
            }
            catch
            {
                throw new Exception(string.Format("Can't input a string here. Identifier {0} type is not string", t.lexeme));
            }

            // int
            if (visitor.varTypeDic[t.lexeme] == IdenType.INT)
            {
                intDic[t.lexeme] = tmpi;
            }
            // bool
            else
            {
                if (tmpi != 0 && tmpi != 1)
                    throw new Exception(string.Format("You should input a bool. Identifier {0} type is bool.", t.lexeme));
                else
                    intDic[t.lexeme] = tmpi;
            }
            */
        }

        // assert
        public void ExeAssert(AssertStat a)
        {
            /*
            ExeExpr(a.expression);
            if (exprIntDic[a.expression] == 0)
                throw new Exception("Assert condition not met. Line: " + a.assertToken.lineNum.ToString());
            */
        }

        // assign
        public void ExeAssign(AssignStat a)
        {
            /*
            ExeExpr(a.expression);
            if (visitor.varTypeDic[a.iden.lexeme] == IdenType.STRING)
            {
                strDic[a.iden.lexeme] = exprStrDic[a.expression];
            }
            else
            {
                intDic[a.iden.lexeme] = exprIntDic[a.expression];
            }
            */
        }

        // definition
        public void ExeDef(DefStat d)
        {
            /*
            if (d.expression == null)
                return;

            ExeExpr(d.expression);
            if (visitor.varTypeDic[d.iden.lexeme] == IdenType.STRING)
                strDic[d.iden.lexeme] = exprStrDic[d.expression];
            else
                intDic[d.iden.lexeme] = exprIntDic[d.expression];
            */
        }

        // for loop
        public void ExeFor(ForStat f)
        {
            /*
            ExeExpr(f.left);
            ExeExpr(f.right);

            int lefti = exprIntDic[f.left];
            int righti = exprIntDic[f.right];
            // loop
            int i;
            for (i = lefti; i <= righti; i++)
            {
                intDic[f.iden.lexeme] = i;
                ExeStats(f.stats);
            }
            intDic[f.iden.lexeme] = i;
            */
        }


    }
}
