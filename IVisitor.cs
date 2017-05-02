using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC
{
    public interface IVisitor
    {
        void Visit(Table table);
        void Visit(Column column);
    }
}
