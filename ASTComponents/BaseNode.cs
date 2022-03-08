using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLCompiler.ASTComponents
{
    abstract class BaseNode
    {
        public abstract BaseNode TryBuild(ref Scanner scanner);
        
    }
}
