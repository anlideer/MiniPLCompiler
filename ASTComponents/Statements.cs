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
            throw new NotImplementedException();
        }
    }
}
