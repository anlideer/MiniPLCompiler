using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler
{
    abstract class BaseNode
    {
        public abstract BaseNode TryBuild(ref Scanner scanner);
        public abstract void Accept(Visitor visitor);

        public abstract void AcceptExe(SimpleInterpreter interpreter);
    }
}
