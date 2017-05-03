using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC
{
    public class Column : IVisitable
    {
        public void setColumnDataType(string dt)
        {
            switch(dt)
            {
                case "varchar":
                    DataType = ColumnDataType.varchar;
                    break;
                case "int":
                    DataType = ColumnDataType.numInt;
                    break;
                case "float":
                    DataType = ColumnDataType.numFloat;
                    break;
                case "tinyint":
                    DataType = ColumnDataType.tinyInt;
                    break;
                default: 
                    DataType = ColumnDataType.nill;
                    break;
            }
        }
        public Column(string _name, string _dataType)
        {
            Name = _name;
            setColumnDataType(_dataType);
            Header = _name;
        }

        public string Name { get; set; }

        public ColumnDataType DataType { get; set; }

        public string Header { get; set; }

        public bool ReadOnly { get; set; } = false;

        public bool Hide { get; set; } = false;

        public bool IsPrimaryKey { get; set; } = false;
        public bool IsForeignKey { get; set; } = false;

        public bool IsForeignKeyMapped { get; set; } = false;
        
        public ForeignKey ForeignKeyMap { get; set; }

        public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();

        public bool IsDropdown => Options.Count > 0;

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public struct ForeignKey
    {
        public Table ToTable { get; set; }

        public string Column { get; set; }
    }
}