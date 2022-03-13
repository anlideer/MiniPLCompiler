using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    enum IdenType { INT, STRING, BOOL };

    class Visitor
    {
        private Dictionary<TokenType, IdenType> idenTypeTrans = new Dictionary<TokenType, IdenType>
        {
            { TokenType.INT_TYPE , IdenType.INT},
            { TokenType.STRING_TYPE, IdenType.STRING},
            { TokenType.BOOL_TYPE, IdenType.BOOL},
        };


        public Dictionary<string, IdenType> varTypeDic = new Dictionary<string, IdenType>();   // bind identifier type
        public Dictionary<string, bool> varInit = new Dictionary<string, bool>();    // record if variable is init
        public Dictionary<Opnd, IdenType> opTypeDic = new Dictionary<Opnd, IdenType>();    // bind opnd type (for later convenience)
        public Dictionary<Expr, IdenType> exprTypeDic = new Dictionary<Expr, IdenType>();   // bind expr calculation type (for convenience)
        
        // visit expr
        public void VisitExpr(Expr expression)
        {
            // just left
            if (expression.op == null)
            {
                VisitOpnd(expression.left);
                if (opTypeDic.ContainsKey(expression.left))
                    exprTypeDic[expression] = opTypeDic[expression.left];
            }
            // unary_op+right
            else if (expression.left == null)
            {
                VisitOpnd(expression.right);
                if (opTypeDic.ContainsKey(expression.right))
                {
                    exprTypeDic[expression] = opTypeDic[expression.right];
                    // check validity (!bool)
                    if (exprTypeDic[expression] != IdenType.BOOL)
                    {
                        // for convenience, report "!"
                        ErrorHandler.PushError(new MyError(expression.op.lexeme, expression.op.lineNum, "! must be used with bool type."));
                    }
                }
            }
            // left+op+right
            else
            {
                VisitOpnd(expression.left);
                VisitOpnd(expression.right);
                if (!opTypeDic.ContainsKey(expression.left) || !opTypeDic.ContainsKey(expression.right))
                    return;
                IdenType leftType = opTypeDic[expression.left];
                IdenType rightType = opTypeDic[expression.right];
                Token op = expression.op;
                if (leftType != rightType)
                {
                    // for convenience, also report just op
                    ErrorHandler.PushError(new MyError(op.lexeme, op.lineNum, "Expressoin left and right sides don't have the same type."));
                    return;
                }

                // &: bool
                if (op.lexeme == "&")
                {
                    if (leftType != IdenType.BOOL)
                        ErrorHandler.PushError(new MyError(op.lexeme, op.lineNum, "Invalid expression. Should be bool&bool."));
                    else
                        exprTypeDic[expression] = IdenType.BOOL;
                }
                // =, < T T return bool
                else if (op.lexeme == "=" || op.lexeme == "<")
                {
                    exprTypeDic[expression] = IdenType.BOOL;
                }
                // +
                else if (op.lexeme == "+")
                {
                    if (leftType == IdenType.INT || leftType == IdenType.STRING)
                        exprTypeDic[expression] = leftType;
                    else
                        ErrorHandler.PushError(new MyError(op.lexeme, op.lineNum, "Invalid expression. + can only be used with strings or integers."));
                }
                // - * /
                else
                {
                    if (leftType == IdenType.INT)
                        exprTypeDic[expression] = IdenType.INT;
                    else
                        ErrorHandler.PushError(new MyError(op.lexeme, op.lineNum, "Invalid expression. -/* can only be used with integers."));
                }
            }
        }

        // visit opnd
        public void VisitOpnd(Opnd opnd)
        {
            Token t = opnd.selfToken;
            if (t != null)
            {
                // int/string
                if (t.type == TokenType.INT_VAL)
                {
                    opTypeDic[opnd] = IdenType.INT;
                }
                else if (t.type == TokenType.STRING_VAL)
                {
                    opTypeDic[opnd] = IdenType.STRING;
                }
                // identifier
                else
                {
                    if (varTypeDic.ContainsKey(t.lexeme))
                    {
                        opTypeDic[opnd] = varTypeDic[t.lexeme];
                    }
                    else
                    {
                        ErrorHandler.PushError(new MyError(t.lexeme, t.lineNum, "Identifier not defined"));
                    }
                }
            }
            else
            {
                VisitExpr(opnd.expression);
                if (exprTypeDic.ContainsKey(opnd.expression))
                {
                    opTypeDic[opnd] = exprTypeDic[opnd.expression];
                }
            }
        }

        // visit def (binding type)
        public void VisitDef(DefStat def)
        {
            string iname = def.iden.lexeme;
            if (varTypeDic.ContainsKey(iname))
            {
                ErrorHandler.PushError(new MyError(def.iden.lexeme, def.iden.lineNum, "Identifier already defined."));
                return;
            }
            varTypeDic[iname] = idenTypeTrans[def.idenType.type];
            if (def.expression == null)
                varInit[iname] = false;
            else
            {
                varInit[iname] = true;
                VisitExpr(def.expression);
                if (exprTypeDic[def.expression] != varTypeDic[iname])
                {
                    ErrorHandler.PushError(new MyError(def.iden.lexeme, def.iden.lineNum, "Identifier is assigned to different type value."));
                }
            }
                
        }

        // visit assign
        public void VisitAssign(AssignStat a)
        {
            if (!varTypeDic.ContainsKey(a.iden.lexeme))
            {
                ErrorHandler.PushError(new MyError(a.iden.lexeme, a.iden.lineNum, "Identifier not defined"));
                return;
            }

            VisitExpr(a.expression);
            if (!exprTypeDic.ContainsKey(a.expression))
                return;
            if (varTypeDic[a.iden.lexeme] != exprTypeDic[a.expression])
            {
                ErrorHandler.PushError(new MyError(a.iden.lexeme, a.iden.lineNum, "Identifier's type and expression's type are different."));
            }
        }

        // visit for
        public void VisitFor(ForStat f)
        {
            if (!varTypeDic.ContainsKey(f.iden.lexeme))
            {
                ErrorHandler.PushError(new MyError(f.iden.lexeme, f.iden.lineNum, "Identifier not defined"));
                return;
            }
            if (varTypeDic[f.iden.lexeme] != IdenType.INT)
            {
                ErrorHandler.PushError(new MyError(f.iden.lexeme, f.iden.lineNum, "Identifier must be type integer"));
                return;
            }

            VisitExpr(f.left);
            VisitExpr(f.right);
            if (!exprTypeDic.ContainsKey(f.left) || !exprTypeDic.ContainsKey(f.right))
                return;
            
            if (exprTypeDic[f.left] != IdenType.INT || exprTypeDic[f.right] != IdenType.INT)
            {
                ErrorHandler.PushError(new MyError(f.iden.lexeme, f.iden.lineNum, "The ranges should also be integers"));
                return;
            }

            VisitStatements(f.stats);
        }

        // visit read
        public void VisitRead(ReadStat r)
        {
            if (!varTypeDic.ContainsKey(r.iden.lexeme))
            {
                ErrorHandler.PushError(new MyError(r.iden.lexeme, r.iden.lineNum, "Identifier not defined"));
            }
        }

        // visit print
        public void VisitPrint(PrintStat p)
        {
            VisitExpr(p.expression);
        }

        // visit assert
        public void VisitAssert(AssertStat a)
        {
            VisitExpr(a.expression);
            if (exprTypeDic.ContainsKey(a.expression))
            {
                if (exprTypeDic[a.expression] != IdenType.BOOL)
                    ErrorHandler.PushError(new MyError(a.assertToken.lexeme, a.assertToken.lineNum, "expression after assert should return bool type value"));
            }
        }

        // visit statements
        public void VisitStatements(Statements stats)
        {
            foreach(BaseNode s in stats.statsList)
            {
                s.Accept(this);
            }
        }
    }

}
