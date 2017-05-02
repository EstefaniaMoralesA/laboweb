using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC
{
    public class Table : IVisitable
    {
        private Dictionary<string, Column> _columns = null;

        public string FullDirectoryPath => Path.Combine(SharedContainer.AppDirectory, Name.ToLower());

        public Table(string _name)
        {
            Name = _name;
            InitDirectory();
        }

        public string Name { get; set; }

        public Dictionary<string, Column> Columns { 
            get
            {
                if(_columns == null)
                {
                    _columns = SharedContainer.DbInstance.GetColumns(Name);
                }
                return _columns;
            }
        } 

        public List<string> HideReg { get; set; } = new List<string>();

        public bool ShowId { get; set; } = true;

        public bool ShowNew { get; set; } = true;

        public bool ShowEdit { get; set; } = true;

        public bool ShowDelete { get; set; } = true;

        public Column PrimaryKey => Columns.Values.Where(c => c.IsPrimaryKey).FirstOrDefault();
        
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void InitDirectory()
        {
            Directory.CreateDirectory(FullDirectoryPath);
        }
    }
}