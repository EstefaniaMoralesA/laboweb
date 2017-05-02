
using System;
using System.IO;

namespace ABC
{
    public class AddVisitor : IVisitor
    {
        private TextWriter _writer;
        private Table _table;
        private int _indentation = 0;

        private string _indent = "    ";

        public AddVisitor(TextWriter writer, Table table)
        {
            _writer = writer;
            _table = table;
        }

        public void Visit(Table table)
        {
            WriteHeader(table);
            WriteContent(table);
            WriteFooter();
        }

        public void Visit(Column column)
        {
            if(column.ReadOnly)
            {
                return;
            }
            _writer.WriteLine("<div class='form-group'>");
            switch(column.DataType)
            {
                case ColumnDataType.varchar:
                _writer.WriteLine($"<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='{column.Name}' placeholder='{column.Header}' type='text' value=''>");
                break;
                case ColumnDataType.numInt:
                _writer.WriteLine($"<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='{column.Name}' placeholder='{column.Header}' type='number' value=''>");
                break;
                case ColumnDataType.tinyInt:
                    if(column.IsDropdown){
                        _writer.WriteLine("<select>");
                        foreach(var option in column.Options)
                        {
                            _writer.WriteLine($"<option value='{option.Key}'>{option.Value}</option>");
                        }
                        _writer.WriteLine("</select>");
                    }
                break;
                
            }
            _writer.WriteLine("</div>");

        }

        public void WriteContent(Table table)
        {
            _writer.WriteLine("<div class='col-lg-10 col-lg-offset-1'>");
            _writer.WriteLine("<form action='' method='post'>");
            foreach(var col in table.Columns.Values)
            {
                Visit(col);
            }

            _writer.WriteLine("<div class='form-group' style='width: 100%; margin-bottom: 0px;'>");
            _writer.WriteLine("<div class='col-sm-6 col-sm-offset-3' style='text-align: center;'>");
            _writer.WriteLine("<input type='submit' value='Guardar' class='btn btn-primary btn-lg' style='padding: 7px 0px !important; width: 100%; text-align: center;'>");
            _writer.WriteLine("</div>");

            _writer.WriteLine("<div class='col-sm-6 col-sm-offset-3' style='text-align: center; margin-top:10px;'>");
            _writer.WriteLine($"<a class='btn btn-default btn-lg' href='{table.Name}.php' style='padding: 7px 0px !important; width: 100%; text-align: center;'>Cancelar</a>");
            _writer.WriteLine("</div>");

            _writer.WriteLine("</div>");
            _writer.WriteLine("</form>");
            _writer.WriteLine("</div>");
        }

         private void Indent()
        {
            _indentation++;
        }

        private void Unindent()
        {
            _indentation--;
        }

        private void WriteIndentation()
        {
            for (int i = 0; i < _indentation; i++)
            {
                _writer.Write(_indent);
            }
        }

        private void WriteHeader(Table table)
        {
            _writer.WriteLine("<?php");
            Indent();
            WriteIndentation();
            _writer.WriteLine($"include_once(\"../header.php\");");
            Unindent();
            WriteIndentation();
            _writer.WriteLine("?>");
            _writer.WriteLine("<div class='col-sm-12' style='margin-top: 70px; text-align: center;'>");
            _writer.WriteLine($"<h2 style='margin-top: 0px; font-weight: bold; font-size: 35px;color: #E42B22;'>Añadir {table.Name}</h2>");
            _writer.WriteLine("</div>");
        }

        private void WriteFooter()
        {
            _writer.WriteLine("<?php");
            Indent();
            WriteIndentation();
            _writer.WriteLine($"include_once(\"../footer.php\");");
            Unindent();
            WriteIndentation();
            _writer.WriteLine("?>");
        }
    }
}