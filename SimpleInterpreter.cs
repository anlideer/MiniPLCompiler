using System;
using System.Collections.Generic;
using System.Text;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    class MyVariable
    {
        public string cName;
        public VarType ty;

        public MyVariable(string n, VarType t)
        {
            cName = n;
            ty = t;
        }
    }

    // translate the AST to simplified C codes.
    class SimpleInterpreter
    {

        private int currentScope;   // scope level
        private List<Dictionary<string, string>> vars = new List<Dictionary<string, string>> ();    // local variables

        private int varCounter = 0; // use for variable name generating
        private int flagCounter = 0;    // use for flag name generating (for if and while
        private Stack<MyVariable> vStack = new Stack<MyVariable> ();   // simple stack
        private string outc;    // output c codes

        // type map
        private Dictionary<VarType, string> cTypeName = new Dictionary<VarType, string>
        {
            {VarType.INT, "int" }, {VarType.INT_ARR, "int"},
            {VarType.REAL, "float" }, {VarType.REAL_ARR, "float"},
            {VarType.BOOL, "int" }, {VarType.BOOL_ARR, "int"},
            {VarType.STR, "char" }, {VarType.STR_ARR, "char"},  // use with caution
        };

        
        public SimpleInterpreter()
        {
            currentScope = 0;
            vars = new List<Dictionary<string, string>>();
            vars.Add(new Dictionary<string, string>());

            // add basic import
            AddLine("#include <stdio.h>");
            AddLine("#include <string.h>");
        }
        
        // helper - add one line to the output (automatically add tabs)
        private void AddLine(string s)
        {
            string res = "";
            for (int j = 0; j < currentScope; j++)
                res += "\t";
            res += s + "\n";
            outc += res;
        }

        // Enter scope
        private void EnterScope()
        {
            currentScope++;
            vars.Add(new Dictionary<string, string>());
        }

        // Exit scope
        private void ExitScope()
        {
            vars.RemoveAt(currentScope);
            currentScope--;
        }


        // find a local variable's reference
        private string GetVar(string varname)
        {
            if (currentScope >= vars.Count)
            {
                Console.WriteLine("Shouldn't happen. Variable map error.");
                return null;
            }

            int sc = currentScope;
            while (sc >= 0)
            {
                var dic = vars[sc];
                if (dic.ContainsKey(varname))
                    return dic[varname];
                else
                    sc--;
            }
            Console.WriteLine("Shouldn't happen. Variable map error.");
            return null;
        }
        
        // help allocate variables to use
        // A+num (A1, A2, ...)
        // function/procedure use their own name defined. (all in lower case, so won't conflict with my higher-case)
        private string GetNewVarName()
        {
            string res = string.Format("A{0}", varCounter);
            varCounter++;
            return res;
        }

        // L+num (L1, L2, ...)
        private string GetNewFlagName()
        {
            string res = string.Format("L{0}", flagCounter);
            flagCounter++;
            return res;
        }

        // helper - dynamic type change (int<->float)
        private void FixTop2Type()
        {
            var v1 = vStack.Pop();
            var v2 = vStack.Pop();
            if (v1.ty == v2.ty)
            {
                vStack.Push(v2);
                vStack.Push(v1);
                return;
            }
            else if (v1.ty == VarType.INT && v2.ty == VarType.REAL)
            {
                string newv = GetNewVarName();
                AddLine(string.Format("float {0} = (float){1}", newv, v1.cName));
                MyVariable tmpv = new MyVariable(newv, VarType.REAL);
                vStack.Push(v2);
                vStack.Push(tmpv);
            }
            else if (v1.ty == VarType.REAL && v2.ty == VarType.INT)
            {
                string newv = GetNewVarName();
                AddLine(string.Format("float {0} = (float){1};", newv, v2.cName));
                MyVariable tmpv = new MyVariable(newv, VarType.REAL);
                vStack.Push(tmpv);
                vStack.Push(v1);
            }
        }


        // term
        public void ExeTerm(Term t)
        {
            t.factor.AcceptExe(this);
            for (int i = 0; i < t.facs.Count; i++)
            {
                t.facs[i].AcceptExe(this);
                FixTop2Type();
                if (t.ops[i].type == TokenType.AND)
                {
                    var v1 = vStack.Pop();
                    var v2 = vStack.Pop();
                    AddLine(string.Format("{0} = {0} & {1};", v1.cName, v2.cName));
                    vStack.Push(v1);
                }
                // *, /, %
                else
                {
                    var v1 = vStack.Pop();
                    var v2 = vStack.Pop();
                    AddLine(string.Format("{0} = {0} {1} {2};", v2.cName, t.ops[i].lexeme, v1.cName));
                    vStack.Push(v2);
                }
            }
        }


        // factor
        public void ExeFactor(Factor fac)
        {
            if (fac.literal != null)
            {
                if (fac.literal.type == TokenType.BOOL_VAL)
                {
                    int val;
                    if (fac.literal.lexeme == "true")
                        val = 1;
                    else
                        val = 0;
                    string newName = GetNewVarName();
                    string typeName = cTypeName[fac.ty];
                    AddLine(string.Format("{0} {1} = {2};", typeName, newName, val));
                    vStack.Push(new MyVariable(newName, fac.ty));
                }
                else if (fac.literal.type != TokenType.STRING_VAL)
                {
                    string newName = GetNewVarName();
                    string typeName = cTypeName[fac.ty];
                    AddLine(string.Format("{0} {1} = {2};", typeName, newName, fac.literal.lexeme));
                    vStack.Push(new MyVariable(newName, fac.ty));
                }
                else
                {
                    string newName = GetNewVarName();
                    AddLine(string.Format("char *{0} = {1};",newName, fac.literal.lexeme));
                    vStack.Push(new MyVariable(newName, fac.ty));

                }
            }
            else if (fac.callStat != null)
            {
                fac.callStat.AcceptExe(this);
            }
            else if (fac.expr != null)
            {
                fac.expr.AcceptExe(this);
            }
            else if (fac.variable != null)
            {
                fac.variable.AcceptExe(this);
                // use
                if (fac.variable.expr != null)
                {
                    // arr[index]
                    var ind = vStack.Pop();
                    string newv = GetNewVarName();
                    string arrName = GetVar(fac.variable.iden.lexeme);
                    AddLine(string.Format("{0} = {1}[{2}];", newv, arrName, ind.cName));
                    vStack.Push(new MyVariable(newv, fac.variable.ty));
                }
            }

            // not
            if (fac.ifNot != null)
            {
                var v = vStack.Pop();
                AddLine(string.Format("{0} = !{0};", v.cName));
                vStack.Push(v);
            }
            // .size
            if (fac.ifSize != null)
            {
                var v = vStack.Pop();
                string newvname = GetNewVarName();
                AddLine(string.Format("int {0} = {1}.size;", newvname, v.cName));
                vStack.Push(new MyVariable(newvname, VarType.INT));
            }
        }

        // simple expression
        public void ExeSimpleExpr(SimpleExpr se)
        {
            se.term.AcceptExe(this);
            // sign
            if (se.sign != null)
            {
                var v = vStack.Pop();
                AddLine(string.Format("{0} = {1}{0};", v.cName, se.sign.lexeme));
                vStack.Push(v);
            }

            for (int i = 0; i < se.terms.Count; i++)
            {
                se.terms[i].AcceptExe(this);
                FixTop2Type();  // dynamic type change, if necessary
                // or
                if (se.ops[i].type == TokenType.OR)
                {
                    var v1 = vStack.Pop();
                    var v2 = vStack.Pop();
                    AddLine(string.Format("{0} = {0} | {1};", v1.cName, v2.cName));
                    vStack.Push(v1);
                }
                // -
                else if (se.ops[i].lexeme == "-")
                {
                    var v1 = vStack.Pop();
                    var v2 = vStack.Pop();
                    AddLine(string.Format("{0} = {0} - {1}", v2.cName, v1.cName));
                    vStack.Push(v2);
                }
                // +
                else if (se.ops[i].lexeme == "+")
                {
                    // string
                    if (se.ty == VarType.STR)
                    {
                        // concat string
                        var v1 = vStack.Pop();
                        var v2 = vStack.Pop();
                        // get length
                        string tmpi = GetNewVarName();
                        AddLine(string.Format("unsigned int {0} = strlen({1});", tmpi, v1.cName));
                        string tmpi2 = GetNewVarName();
                        AddLine(string.Format("unsigned int {0} = strlen({1});", tmpi2, v2.cName));
                        AddLine(string.Format("{0} = {0} + {1};", tmpi, tmpi2));
                        // concat
                        string tmps = GetNewVarName();
                        AddLine(string.Format("char {0}[{1}];", tmps, tmpi));
                        AddLine(string.Format("strcpy({0}, {1});", tmps, v2.cName));
                        AddLine(string.Format("strcat({0}, {1});", tmps, v1.cName));
                        vStack.Push(new MyVariable(tmps, VarType.STR));
                    }
                    // number
                    else
                    {
                        var v1 = vStack.Pop();
                        var v2 = vStack.Pop();
                        AddLine(string.Format("{0} = {0} + {1}", v2.cName, v1.cName));
                        vStack.Push(v2);
                    }
                }
            }
        }

        // expression
        public void ExeExpr (Expr expression)
        {
            expression.left.AcceptExe(this);
            if (expression.right != null && expression.op != null)
            {
                expression.right.AcceptExe(this);
                var v1 = vStack.Pop();
                var v2 = vStack.Pop();
                string tmpb = GetNewVarName();
                AddLine(string.Format("int {0} = {1} {2} {3};", tmpb, v2.cName, expression.op.lexeme, v1.cName));
                vStack.Push(new MyVariable(tmpb, VarType.BOOL));
            }
        }

        // type (only needs to resolve expression)
        public void ExePLType(PLType t)
        {
            if (t.expr != null)
                t.expr.AcceptExe(this);
        }

        // var declaration
        public void ExeDef(DefStat d)
        {
            d.idenType.AcceptExe(this);
            // simple type
            if (d.idenType.isArray == false)
            {
                string star = "";
                if (d.idenType.ty == VarType.STR)
                    star = "*";

                string res = cTypeName[d.idenType.ty];
                // just define
                foreach (var token in d.idens)
                {
                    string tmp = GetNewVarName();
                    res += string.Format(" {0}{1},", star, tmp);
                    // map
                    vars[currentScope][token.lexeme] = tmp;
                }
                res = res.Remove(res.Length - 1);
                res += ";";
                AddLine(res);
            }
            // arr
            else
            {
                string star = "";
                if (d.idenType.ty == VarType.STR_ARR)
                    star = "*";
                string res = cTypeName[d.idenType.ty];
                // all arrs length are idenType.expr
                foreach(var token in d.idens)
                {
                    string tmp = GetNewVarName();
                    res += string.Format(" {0}{1}[{2}],", star, tmp, d.idenType.expr);
                    // map
                    vars[currentScope][token.lexeme] = tmp;
                }
                res = res.Remove(res.Length - 1);
                res += ";";
                AddLine(res);
            }
        }

        // variable
        public void ExeVariable(Variable v)
        {
            // arr[index]
            if (v.expr != null)
            {
                v.expr.AcceptExe(this); // in stack
            }
            // doing anything to variable (assign/use) is other one's duty
        }

        // assert
        public void ExeAssert(AssertStat a)
        {
            a.expression.AcceptExe(this);
            var v = vStack.Pop();
            AddLine(string.Format("assert({0});", v.cName));
        }

        // assign
        public void ExeAssign(AssignStat a)
        {
            // C will do the dynamic casting (int->float, float->int)
            a.expression.AcceptExe(this);
            a.variable.AcceptExe(this);
            // not arr
            if (a.variable.expr == null)
            {
                var val = vStack.Pop();
                string vname = GetVar(a.variable.iden.lexeme);
                AddLine(string.Format("{0} = {1};", vname, val.cName));
            }
            // arr
            else
            {
                // stack pop is index
                var ind = vStack.Pop();
                var val = vStack.Pop();
                string vname = GetVar(a.variable.iden.lexeme);
                AddLine(string.Format("{0}[{1}] = {2};", vname, ind.cName, val.cName));
            }
        }

        // arguments
        public void ExeArguments(Arguments args)
        {
            foreach(var e in args.exprs)
            {
                e.AcceptExe(this);
            }
        }


        // print
        // split with a space, end with a \n
        public void ExePrint(PrintStat stat)
        {
            stat.args.AcceptExe(this);
            // %d %f %s
            List<MyVariable> myArgs = new List<MyVariable>();
            while(vStack.Count > 0)
            {
                myArgs.Add(vStack.Pop());
            }
            // printf("", ...);
            string res = "";
            string argstr = "";
            for (int i = myArgs.Count - 1; i >= 0; i--)
            {
                argstr += myArgs[i].cName + ",";
                if (myArgs[i].ty == VarType.INT || myArgs[i].ty == VarType.BOOL)
                    res += "%d ";
                else if (myArgs[i].ty == VarType.REAL)
                    res += "%f ";
                else
                    res += "%s ";
            }
            if (res.Length > 0)
                res.Remove(res.Length - 1); // delete space
            if (argstr.Length > 0)
                argstr.Remove(argstr.Length - 1);   // delete ,
            AddLine(string.Format("printf(\"{0}\n\", {1});", res, argstr));
        }

        // read
        // split the read to many different scanf
        public void ExeRead(ReadStat r)
        {
            foreach(var v in r.vars)
            {
                v.AcceptExe(this);
                // arr[ind]
                if (v.expr != null)
                {
                    v.expr.AcceptExe(this);
                    var ind = vStack.Pop();
                    if (v.ty == VarType.INT || v.ty == VarType.BOOL)
                        AddLine(string.Format("scanf(\"%d\", {0}[{1}]);", GetVar(v.iden.lexeme), ind.cName));
                    else if (v.ty == VarType.STR)
                        AddLine(string.Format("scanf(\"%s\", {0}[{1}]);", GetVar(v.iden.lexeme), ind.cName));
                    else
                        AddLine(string.Format("scanf(\"%f\", {0}[{1}]);", GetVar(v.iden.lexeme), ind.cName));
                }
                // simple
                else
                {
                    if (v.ty == VarType.INT || v.ty == VarType.BOOL)
                        AddLine(string.Format("scanf(\"%d\", {0});", GetVar(v.iden.lexeme)));
                    else if (v.ty == VarType.STR)
                        AddLine(string.Format("scanf(\"%s\", {0});", GetVar(v.iden.lexeme)));
                    else
                        AddLine(string.Format("scanf(\"%f\", {0});", GetVar(v.iden.lexeme)));
                }
            }
        }

        // call
        public void ExeCall(CallStat stat)
        {
            stat.args.AcceptExe(this);
            List<MyVariable> myArgs = new List<MyVariable>();
            while (vStack.Count > 0)
            {
                myArgs.Add(vStack.Pop());
            }

            string argstr = "";
            for (int i = myArgs.Count - 1; i >= 0; i--)
            {
                argstr += myArgs[i].cName + ",";
            }
            if (argstr.Length > 0)
                argstr.Remove(argstr.Length - 1);   // delete ,

            AddLine(string.Format("{0}({1});", stat.iden.lexeme, argstr));
        }

        // simple statement
        public void ExeSimpleStat(SimpleStat stat)
        {
            stat.AcceptExe(this);
        }

        // if statement
        // use if-goto
        // if (condition) goto L1 (skip else statement)
        // else statement goto L2 (skip if statement)
        // L1: if statement
        // L2: continue
        public void ExeIfStat(IfStat stat)
        {
            stat.expr.AcceptExe(this);
            string L1 = GetNewFlagName();
            string L2 = GetNewFlagName();
            var condition = vStack.Pop();
            // condition
            AddLine(string.Format("if ({0}) {goto {1};}", condition.cName, L1));
            // else
            if (stat.elseStat != null)
            {
                stat.elseStat.AcceptExe(this);
            }
            AddLine(string.Format("goto {0};", L2));
            // if
            AddLine(string.Format("{0}:", L1));
            stat.thenStat.AcceptExe(this);
            // continue
            AddLine(string.Format("{0}:", L2));
        }

        // while statement
        // use if-goto
        // change condition to neg condition
        // L1:
        // negcon = !condition
        // if (negcon) goto L2
        // while statement content........goto L1
        // L2: continue
        public void ExeWhileStat(WhileStat stat)
        {
            stat.expr.AcceptExe(this);
            string negvar = GetNewVarName();
            AddLine(string.Format("int {0};", negvar));
            string L1 = GetNewFlagName();
            AddLine(string.Format("{0}:", L1));
            var condition = vStack.Pop();
            AddLine(string.Format("{0} = !{1};", negvar, condition.cName));
            string L2 = GetNewFlagName();
            AddLine(string.Format("if({0}) {goto {1};}", negvar, L2));
            stat.stat.AcceptExe(this);  // while stat
            AddLine(string.Format("goto {0};", L1));
            AddLine(string.Format("{0}:", L2));
        }

        // block
        public void ExeBlock(Block b)
        {
            // just add scope, and accept. (no need for {} because if and while are all if-goto structure now)
            EnterScope();
            foreach (var s in b.stats)
                s.AcceptExe(this);
            ExitScope();
        }

        // structured statement
        public void ExeStructStat(StructuredStat stat)
        {
            // just accept
            stat.stat.AcceptExe(this);
        }

        // statement
        public void ExeStatement(Statement stat)
        {
            stat.stat.AcceptExe(this);
        }



    }
}
