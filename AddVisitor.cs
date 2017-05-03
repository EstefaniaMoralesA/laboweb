
using System;
using System.IO;
using System.Text;

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
            if(column.ReadOnly || column.Hide)
            {
                return;
            }
            _writer.WriteLine("<div class='form-group'>");
            if(column.IsForeignKey)
            {
                WriteForeignKeyInput(column);
            }
            else
            {
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
                            _writer.WriteLine($"<select name='{column.Name}'>");
                            _writer.WriteLine($"<option selected disabled value=''>Selecciona {column.Name}</option>");                        
                            foreach(var option in column.Options)
                            {
                                _writer.WriteLine($"<option value='{option.Key}'>{option.Value}</option>");
                            }
                            _writer.WriteLine("</select>");
                        }
                    break;
                }
            }
            _writer.WriteLine("</div>");

        }

        public void WriteForeignKeyInput(Column column)
        {
            _writer.WriteLine($"<?php $fkquery = 'SELECT * FROM {column.ForeignKeyMap.ToTable.Name}';");
            _writer.WriteLine($"$fkresult = $db->squery_rows($fkquery, array()); ?>");
            _writer.WriteLine($"<select name='{column.Name}'>");
            _writer.WriteLine($"<option selected disabled value=''>Selecciona {column.Header}</option>");
            _writer.WriteLine("<?php while($data = mysqli_fetch_assoc($fkresult)) : ?>");
            _writer.Write($"<option value=\"<?php echo $data['{column.ForeignKeyMap.ToTable.PrimaryKey.Name}'];?>\">");
            _writer.WriteLine($"<?php echo $data['{column.ForeignKeyMap.Column}']; ?></option>");
            _writer.WriteLine("<?php endwhile; ?>");
            _writer.WriteLine("</select>");
        }

        public void WriteContent(Table table)
        {
            _writer.WriteLine("<div class='col-lg-10 col-lg-offset-1'>");
            _writer.WriteLine($"<form action='add{table.Name}.php' method='post'>");
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

        private string GetColumnsString(Table table)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach(var col in table.Columns.Values)
            {
                if(i > 0)
                {
                    sb.Append(',');
                }
                sb.Append($"`{col.Name}`");
                i++;
            }
            return sb.ToString();
        }

        private string GetWildCardString(Table table)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach(var col in table.Columns.Values)
            {
                if(i > 0)
                {
                    sb.Append(',');
                }
                sb.Append($"'{SharedContainer.AppConfigInstance.WildCard}'");
                i++;
            }
            return sb.ToString();
        }

        private string GetValuesToInsert(Table table)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach(var col in table.Columns.Values)
            {
                if(i > 0)
                {
                    sb.Append(',');
                }
                if(col.IsPrimaryKey)
                {
                    sb.Append("NULL");
                }
                else 
                {
                    sb.Append($"${col.Name}");
                }
                i++;
            }
            return sb.ToString();
        }

        private void WriteHeader(Table table)
        {
            _writer.WriteLine("<?php");
            Indent();
            WriteIndentation();
            _writer.WriteLine($"include_once(\"../header.php\");");
            Unindent();
            WriteIndentation();
            _writer.WriteLine("if(!empty($_POST)){");
            _writer.WriteLine("$error = 0;");
            foreach(var col in table.Columns.Values)
            {
                if(!col.IsPrimaryKey)
                {
                    if(col.IsDropdown || col.IsForeignKey)
                    {
                        _writer.WriteLine($"if(!isset($_POST['{col.Name}']) || $_POST['{col.Name}'] == '')");
                    }
                    else{
                        _writer.WriteLine($"if(empty($_POST['{col.Name}']))");
                    }
                    _writer.WriteLine("{");
                    _writer.WriteLine("$error = 1;");
                    _writer.WriteLine("}");
                    _writer.WriteLine("else{");
                    _writer.WriteLine($"${col.Name} = $_POST['{col.Name}'];");
                    _writer.WriteLine("}");
                }
            }
            _writer.WriteLine("if($error == 1){");
            _writer.WriteLine("echo '<div class=\"alert alert-danger\">ERROR se deben de llenar todos los campos de la forma.</div>';}");
            _writer.WriteLine("else{");
            _writer.WriteLine($"$query = \"INSERT INTO {table.Name}({GetColumnsString(table)}) VALUES ({GetWildCardString(table)})\";");
            _writer.WriteLine($"$result = $db->squery_rows($query, array({GetValuesToInsert(table)}));");
            _writer.WriteLine("if($result == 1){");
            _writer.WriteLine($"header('Location: {table.Name}.php');");
            _writer.WriteLine("}");
            _writer.WriteLine("}");
            _writer.WriteLine("}");
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