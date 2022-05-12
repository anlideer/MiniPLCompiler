using System;
using System.Collections.Generic;
using System.Text;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    public enum VarType { INT, REAL, STR, BOOL, INT_ARR, REAL_ARR, BOOL_ARR, STR_ARR, NONE};

    class Visitor
    {
        private List<Dictionary<string, VarType>> variablesType = new List<Dictionary<string, VarType>>();
        private List<Dictionary<string, bool>> variablesInit = new List<Dictionary<string, bool>>();
        private int currentScope;
        private Dictionary<string, Func> functions = new Dictionary<string, Func>();

        private Func currentFunc;   // show that we are now in which function's scope (to check return...)
        private bool funcReturned = false;

        private Dictionary<TokenType, VarType> typeMap = new Dictionary<TokenType, VarType>
        {
            {TokenType.INT_TYPE, VarType.INT },
            {TokenType.INT_VAL, VarType.INT },
            {TokenType.REAL_TYPE, VarType.REAL },
            {TokenType.REAL_VAL, VarType.REAL },
            {TokenType.STRING_TYPE, VarType.STR },
            {TokenType.STRING_VAL, VarType.STR },
            {TokenType.BOOL_TYPE, VarType.BOOL },
            {TokenType.BOOL_VAL, VarType.BOOL },
        };
        private Dictionary<VarType, VarType> arrTypesToSimp = new Dictionary<VarType, VarType>
        {
            {VarType.BOOL_ARR, VarType.BOOL },
            {VarType.INT_ARR, VarType.INT },
            {VarType.STR_ARR, VarType.STR },
            {VarType.REAL_ARR, VarType.REAL }
        };
        private Dictionary<VarType, VarType> simpleTypesToArr = new Dictionary<VarType, VarType> 
        {
            {VarType.BOOL, VarType.BOOL_ARR },
            {VarType.INT, VarType.INT_ARR },
            {VarType.STR, VarType.STR_ARR },
            {VarType.REAL, VarType.REAL_ARR }
        };


        public Visitor()
        {
            // init
            variablesType.Add(new Dictionary<string, VarType>());
            variablesInit.Add(new Dictionary<string, bool>());
            currentScope = 0;
        }

        // Get Variable type
        private VarType GetVariableType(string varname)
        {
            if (currentScope >= variablesType.Count)
                return VarType.NONE;

            int sc = currentScope;
            while (sc >= 0)
            {
                var dic = variablesType[sc];
                if (dic.ContainsKey(varname))
                    return dic[varname];
                else
                    sc--;
            }
            return VarType.NONE;
        }

        // is variable declared in this scope?
        // used by the declaration visitor
        private bool IsVariableDecThisScope(string varname)
        {
            if (currentScope >= variablesType.Count)
                return false;
            var dic = variablesType[currentScope];
            if (dic.ContainsKey(varname))
                return true;
            else
                return false;
        }



        // get variable init?
        private bool IsVariableInit(string varname)
        {
            if (currentScope >= variablesType.Count)
                return false;

            int sc = currentScope;
            while (sc >= 0)
            {
                var dic = variablesInit[sc];
                if (dic.ContainsKey(varname))
                    return dic[varname];
                else
                    sc--;
            }
            return false;
        }

        // record variable assigned
        private void RecordVariableInit(string varname)
        {
            if (currentScope >= variablesType.Count)
                return;

            int sc = currentScope;
            while(sc >= 0)
            {
                var dic = variablesType[sc];
                if (dic.ContainsKey(varname))
                    break;
                else
                    sc--;
            }
            if (sc < 0)
            {
                Console.WriteLine("Shouldn't happen. You should first check declaration.");
            }
            else
            {
                variablesInit[sc][varname] = true;
            }
        }



        // Enter scope
        private void EnterScope()
        {
            currentScope++;
            variablesType.Add(new Dictionary<string, VarType>());
            variablesInit.Add(new Dictionary<string, bool>());
        }

        // Exit scope
        private void ExitScope()
        { 
            variablesType.RemoveAt(currentScope);
            variablesInit.RemoveAt(currentScope);
            currentScope--;
        }

        // visit argument
        public void VisitAruguments(Arguments a)
        {
            foreach(var expr in a.exprs)
            {
                expr.Accept(this);  // check type work is by outer element (writeln / call...)
            }
        }

        // visit term
        public void VisitTerm(Term t)
        {
            Factor fac = t.factor;
            fac.Accept(this);
            t.ty = fac.ty;
            for (int i = 0; i < t.ops.Count; i++)
            {
                if (t.ops[i].type == TokenType.MULTIPLY_OPERATOR)
                {
                    if (t.ty == VarType.BOOL)
                        ErrorHandler.PushError(new MyError(t.ops[i].lexeme, t.ops[i].lineNum, "Multiplying operator can't be used for bool type"));
                    // check type
                    t.facs[i].Accept(this);
                    // check op type correspond with factor types
                    // only int, real, can't be string
                    if (t.facs[i].ty == VarType.STR)
                        ErrorHandler.PushError(new MyError(t.ops[i].lexeme, t.ops[i].lineNum, "Among multiplying operators, only + can be used for strings. This is invalid."));
                }
                else if (t.ops[i].type == TokenType.AND)
                {
                    if (t.ty != VarType.BOOL)
                    {
                        ErrorHandler.PushError(new MyError(t.ops[i].lexeme, t.ops[i].lineNum, "AND operator can only be used for bool type"));
                    }
                    // check if type is bool
                    t.facs[i].Accept(this);
                    if (t.facs[i].ty != VarType.BOOL)
                    {
                        ErrorHandler.PushError(new MyError(t.ops[i].lexeme, t.ops[i].lineNum, "AND operator can only be used for bool type"));
                    }
                }
            }

        }

        // visit factor
        public void VisitFactor(Factor fac)
        {
            // solve call/variable/literal/expr
            if (fac.literal != null)
            {
                fac.ty = typeMap[fac.literal.type];
            }
            else
            {
                if (fac.callStat != null)
                {
                    fac.callStat.Accept(this);
                    fac.ty = fac.callStat.ty;
                }
                else if (fac.variable != null)
                {
                    fac.variable.Accept(this);
                    fac.ty = fac.variable.ty;
                    // declared? init?
                    var id = fac.variable.iden;
                    if (GetVariableType(id.lexeme) == VarType.NONE)
                    {
                        // not declared
                        ErrorHandler.PushError(new MyError(id.lexeme, id.lineNum, "Identifier not declared."));
                    }
                    else if (IsVariableInit(id.lexeme) == false)
                    {
                        // not init
                        ErrorHandler.PushError(new MyError(id.lexeme, id.lineNum, "Variable not initialized."));
                    }
                }
                else if (fac.expr != null)
                {
                    fac.expr.Accept(this);
                    fac.ty = fac.expr.ty;
                }
                
            }

            // not (bool)
            if (fac.ifNot != null)
            {
                if (fac.ty != VarType.BOOL)
                {
                    ErrorHandler.PushError(new MyError(fac.ifNot.lexeme, fac.ifNot.lineNum, "Keyword not can only be used for bool type"));
                }
            }

            // .size
            if (fac.ifSize != null)
            {
                if (!arrTypesToSimp.ContainsKey(fac.ty))
                {
                    ErrorHandler.PushError(new MyError(fac.ifSize.lexeme, fac.ifSize.lineNum, "Keyword .size can only be used for array type"));
                }

                // .size is int type
                fac.ty = VarType.INT;

            }

        }

        // visit simple expr
        public void VisitSimpleExpr(SimpleExpr se)
        {
            se.term.Accept(this);
            se.ty = se.term.ty;
            // sign
            if (se.sign != null)
            {
                if (se.ty == VarType.BOOL || se.ty == VarType.STR)
                    ErrorHandler.PushError(new MyError(se.sign.lexeme, se.sign.lineNum, "Sign +/- can only be used for int or real types"));
            }

            // solve all
            for (int i = 0; i < se.ops.Count; i++)
            {
                se.terms[i].Accept(this);

                // bool
                if (se.ops[i].type == TokenType.OR)
                {
                    if (se.ty != VarType.BOOL || se.terms[i].ty != VarType.BOOL)
                        ErrorHandler.PushError(new MyError(se.ops[i].lexeme, se.ops[i].lineNum, "OR can only be used for bool type"));
                }
                // +
                else if (se.ops[i].lexeme == "+")
                {
                    if (se.terms[i].ty == VarType.STR && se.ty == VarType.STR)
                    { }
                    else if (se.terms[i].ty == VarType.STR || se.ty == VarType.STR)
                    {
                        ErrorHandler.PushError(new MyError(se.ops[i].lexeme, se.ops[i].lineNum, "+ can either be used for two string type terms, or be used for two int/real type terms."));
                    }
                }
                // -
                else
                {
                    if (se.terms[i].ty == VarType.STR || se.ty == VarType.STR)
                        ErrorHandler.PushError(new MyError(se.ops[i].lexeme, se.ops[i].lineNum, "- can't be used for string type"));
                }
            }
        }


        // visit expr
        public void VisitExpr(Expr expr)
        {
            expr.left.Accept(this);
            if (expr.op == null && expr.right == null)
            {
                expr.ty = expr.left.ty;
                return;
            }

            expr.right.Accept(this);
            // type check (must be the same type)
            if (expr.left.ty != expr.right.ty)
            {
                ErrorHandler.PushError(new MyError(expr.op.lexeme, expr.op.lineNum, "The relational operator can only be used for the same type."));
            }
            expr.ty = VarType.BOOL;
        }

        // visit variable
        public void VisitVariable(Variable v)
        {
            // declared
            string lexeme = v.iden.lexeme;
            v.ty = GetVariableType(lexeme);
            if (v.ty == VarType.NONE)
            {
                ErrorHandler.PushError(new MyError(v.iden.lexeme, v.iden.lineNum, "Identifier not declared."));
                return;
            }

            if (v.expr == null)
                return;

            // integer expr
            v.expr.Accept(this);
            if (v.expr.ty != VarType.INT)
                ErrorHandler.PushError(new MyError(v.iden.lexeme, v.iden.lineNum, "arr's index must be an integer expression"));

            if (!arrTypesToSimp.ContainsKey(v.ty))
            {
                ErrorHandler.PushError(new MyError(v.iden.lexeme, v.iden.lineNum, "Only when variable is array type, you can use arr[]"));
                v.ty = VarType.NONE;
                return;
            }

            v.ty = arrTypesToSimp[v.ty];
        }

        // visit pl type
        public void VisitPLType(PLType plt)
        {
            plt.ty = typeMap[plt.pltype.type];
            if (plt.expr != null)
            {
                // integer expr
                plt.expr.Accept(this);
                if (plt.expr.ty != VarType.INT)
                {
                    ErrorHandler.PushError(new MyError(plt.pltype.lexeme, plt.pltype.lineNum, "The expression should be integer type."));
                }
                plt.ty = simpleTypesToArr[plt.ty];
            }
        }


        // visit def (binding type)
        public void VisitDef(DefStat def)
        {
            def.idenType.Accept(this);
            var ty = def.idenType.ty;
            foreach(var id in def.idens)
            {
                // not declared before
                if (IsVariableDecThisScope(id.lexeme))
                    ErrorHandler.PushError(new MyError(id.lexeme, id.lineNum, "The identifier is declared in this scope before"));
                variablesType[currentScope][id.lexeme] = ty;
            }
        }

        // visit block
        public void VisitBlock(Block b, bool isFunc=false)  // if it's func or procedure, then the scope number is handled by the func
        {
            if (!isFunc)
                EnterScope();
            foreach(var n in b.stats)
            {
                // check dead code
                if (isFunc && funcReturned)
                {
                    ErrorHandler.PushError(new MyError(currentFunc.iden.lexeme, currentFunc.iden.lineNum, "The function/procedure has dead codes after return"));
                }
                n.Accept(this);
            }
            if (!isFunc)
                ExitScope();
        }

        // visit assign
        public void VisitAssign(AssignStat a)
        {
            a.variable.Accept(this);
            a.expression.Accept(this);
            // check if variable declared
            string id = a.variable.iden.lexeme;
            var ty = GetVariableType(id);
            if (ty == VarType.NONE)
            {
                ErrorHandler.PushError(new MyError(a.variable.iden.lexeme, a.variable.iden.lineNum, "Identifiers can't be used without declaration."));
            }
            // check type
            else if (ty != a.expression.ty)
            {
                ErrorHandler.PushError(new MyError(a.variable.iden.lexeme, a.variable.iden.lineNum, "The expression type must be the same as the variable type."));
            }

            // record initialized
            RecordVariableInit(id);
        }


        // visit read
        public void VisitRead(ReadStat r)
        {
            foreach(var v in r.vars)
            {
                v.Accept(this);
                // declared
                var ty = GetVariableType(v.iden.lexeme);
                if (ty == VarType.NONE)
                    ErrorHandler.PushError(new MyError(v.iden.lexeme, v.iden.lineNum, "Variable must be declared before use"));
                // type check
                else if (arrTypesToSimp.ContainsKey(ty))
                    ErrorHandler.PushError(new MyError(v.iden.lexeme, v.iden.lineNum, "Read statement does not accept array type variable. Only simple types."));

                // record init
                RecordVariableInit(v.iden.lexeme);
            }
        }

        // visit print
        public void VisitPrint(PrintStat p)
        {
            p.args.Accept(this);
            // type check (can't be array)
            foreach(var ex in p.args.exprs)
            {
                if (arrTypesToSimp.ContainsKey(ex.ty))
                {
                    ErrorHandler.PushError(new MyError(p.printToken.lexeme, p.printToken.lineNum, "Writeln's expression can't be array type."));
                }    
            }
        }

        // visit assert
        public void VisitAssert(AssertStat a)
        {
            a.expression.Accept(this);
            // type check (must be bool)
            if (a.expression.ty != VarType.BOOL)
            {
                ErrorHandler.PushError(new MyError(a.assertToken.lexeme, a.assertToken.lineNum, "Assert expression must be boolean type."));
            }
        }

        // visit return statement
        public void VisitReturn(ReturnStat r)
        {
            if (r.expr != null)
            {
                r.expr.Accept(this);
                r.ty = r.expr.ty;
            }

            if (currentFunc != null)
            {
                // check returned type
                if (r.ty != currentFunc.ty)
                {
                    ErrorHandler.PushError(new MyError(r.token.lexeme, r.token.lineNum, "Returned type is different from function/procedure declaration"));
                }
                funcReturned = true;
            }
            else
            {
                ErrorHandler.PushError(new MyError(r.token.lexeme, r.token.lineNum, "Can't use return statement outside functions/procedures."));
            }
        }

        // visit while statement
        public void VisitWhile(WhileStat w)
        {
            w.expr.Accept(this);
            w.stat.Accept(this);

            // check type (bool expr)
            if (w.expr.ty != VarType.BOOL)
                ErrorHandler.PushError(new MyError(w.whileToken.lexeme, w.whileToken.lineNum, "while's condition expression must be boolean type."));
        }

        // visit if statement
        public void VisitIf(IfStat stat)
        {
            if (funcReturned)   // dead codes
            {
                stat.expr.Accept(this);
                stat.thenStat.Accept(this);
                if (stat.elseStat != null)
                    stat.elseStat.Accept(this);
            }
            else
            {
                funcReturned = false;
                stat.expr.Accept(this);
                bool ifReturned = funcReturned; // if any of the statement does not return, then as a whole they don't return (not every branch returns)
                funcReturned = false;
                stat.thenStat.Accept(this);
                bool thenReturned = funcReturned;
                if (stat.elseStat != null)
                {
                    funcReturned = false;
                    stat.elseStat.Accept(this);
                }
                if (!ifReturned || !thenReturned || !funcReturned)
                    funcReturned = false;
            }


            // check type expr bool
            if (stat.expr.ty != VarType.BOOL)
                ErrorHandler.PushError(new MyError(stat.ifToken.lexeme, stat.ifToken.lineNum, "if's condition expression must be boolean type."));

        }

        // visit structured statement
        public void VisitStructStat(StructuredStat ss)
        {
            ss.stat.Accept(this);
        }

        // visit statement
        public void VisitStatement(Statement stat)
        {
            stat.stat.Accept(this);
        }

        // visit parameter
        public void VisitParameter(Parameters p)
        {
            foreach(Param param in p.parameters)
            {
                param.pltype.Accept(this);
                param.ty = param.pltype.ty;
                // record declaration and init
                variablesType[currentScope][param.iden.lexeme] = param.ty;
                RecordVariableInit(param.iden.lexeme);
            }
        }

        // visit function
        public void VisitFunction(Func f)
        {
            // record id
            functions[f.iden.lexeme] = f;

            EnterScope();
            // visit
            f.parameters.Accept(this);
            if (f.isFunc)
            {
                f.returnedType.Accept(this);
                f.ty = f.returnedType.ty;
            }
            else
            {
                f.ty = VarType.NONE;
            }

            // visit block
            currentFunc = f;
            funcReturned = false;
            VisitBlock(f.block, true);
            // check return
            if (!funcReturned)
            {
                ErrorHandler.PushError(new MyError(f.iden.lexeme, f.iden.lineNum, "Sometimes the function/procedure does not return"));
            }

            ExitScope();
        }

        // visit call
        public void VisitCall(CallStat c)
        {
            c.args.Accept(this);
            if (functions.ContainsKey(c.iden.lexeme))
            {
                Func f = functions[c.iden.lexeme];
                c.ty = f.ty;
                // check if arguments are the same type, length as functions
                if (c.args.exprs.Count != f.parameters.parameters.Count)
                {
                    ErrorHandler.PushError(new MyError(c.iden.lexeme, c.iden.lineNum, "Arguments number is not the same as function/procedure definition"));
                }
                else
                {
                    // check type
                    for (int i = 0; i < f.parameters.parameters.Count; i++)
                    {
                        Param p = f.parameters.parameters[i];
                        if (p.ty != c.args.exprs[i].ty)
                        {
                            ErrorHandler.PushError(new MyError(c.iden.lexeme, c.iden.lineNum, String.Format("Argument {0}'s type is not the same as function/procedure definition.", i + 1)));
                        }
                    }

                }
            }
            else
            {
                ErrorHandler.PushError(new MyError(c.iden.lexeme, c.iden.lineNum, "Function/procedure not defined."));
            }
        }

        // visit simple statement
        public void VisitSimpleStat(SimpleStat sm)
        {
            sm.stat.Accept(this);
        }

        // visit program
        public void VisitProgram(PLProgram p)
        {
            foreach (Func f in p.funcs)
                f.Accept(this);
            p.mainBlock.Accept(this);
        }

    }

}
