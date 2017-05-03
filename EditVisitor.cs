
using System;
using System.IO;
using System.Text;

namespace ABC
{
    public class EditVisitor : IVisitor
    {
        private TextWriter _writer;
        private Table _table;
        private int _indentation = 0;

        private string _indent = "    ";

        public EditVisitor(TextWriter writer, Table table)
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
                    _writer.WriteLine($"<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='{column.Name}' placeholder='{column.Header}' type='text' value='<?php echo $result['{column.Name}'] ?>'>");
                    break;
                    case ColumnDataType.numInt:
                    _writer.WriteLine($"<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='{column.Name}' placeholder='{column.Header}' type='number' value='<?php echo $result['{column.Name}'] ?>'>");
                    break;
                    case ColumnDataType.tinyInt:
                        if(column.IsDropdown){
                            _writer.WriteLine($"<select name='{column.Name}'>");
                            _writer.WriteLine($"");                        
                            foreach(var option in column.Options)
                            {
                                _writer.WriteLine($"<?php if($result['{column.Name}'] == {option.Key}) : ?>");
                                _writer.WriteLine($"<option selected value='<?php echo $result['{column.Name}']; ?>'>{option.Value}</option>");
                                _writer.WriteLine("<?php else : ?>");
                                _writer.WriteLine($"<option value='{option.Key}'>{option.Value}</option>");
                                _writer.WriteLine("<?php endif; ?>");
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
            _writer.WriteLine("<?php while($data = mysqli_fetch_assoc($fkresult)) :");
            _writer.WriteLine($"$value = $data['{column.ForeignKeyMap.ToTable.PrimaryKey.Name}'];");
            _writer.WriteLine("$selected='';");
            _writer.WriteLine($"if($value == $result['{column.Name}'])");
            _writer.WriteLine("{ $selected = 'selected';} ?>");
            _writer.Write($"<option <?php echo $selected; ?> value=\"<?php echo $value;?>\">");
            _writer.WriteLine($"<?php echo $data['{column.ForeignKeyMap.Column}']; ?></option>");
            _writer.WriteLine("<?php endwhile; ?>");
            _writer.WriteLine("</select>");
        }

        public void WriteContent(Table table)
        {
            _writer.WriteLine("<div class='col-lg-10 col-lg-offset-1'>");
            _writer.WriteLine($"<form action='edit{table.Name}.php?pk=<?php echo $pk; ?>' method='post'>");
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
                if(col.ReadOnly)
                {
                    continue;
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
                if(!col.ReadOnly && !col.IsPrimaryKey)
                {
                    sb.Append($"{col.Name} = '{SharedContainer.AppConfigInstance.WildCard}'");
                    i++;
                }
            }
            return sb.ToString();
        }

        private string GetValuesToEdit(Table table)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach(var col in table.Columns.Values)
            {
                if(i > 0)
                {
                    sb.Append(',');
                }
                if(!col.ReadOnly && !col.IsPrimaryKey)
                {
                    sb.Append($"${col.Name}");
                    i++;
                }
            }
            sb.Append(", $pk");
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
            _writer.WriteLine("$result = NULL;");
            _writer.WriteLine("if(empty($_GET['pk'])){");
            _writer.WriteLine("echo 'ERROR no se ha encontrado un registro para editar'; exit();}");
            _writer.WriteLine("$pk = $_GET['pk'];");

            _writer.WriteLine("function getValues($db, $pk){");
            _writer.WriteLine($"$query = \"SELECT {GetColumnsString(table)} FROM {table.Name} WHERE {table.PrimaryKey.Name} = \" . '' . \"{SharedContainer.AppConfigInstance.WildCard}\";");
            _writer.WriteLine($"return $db->qarray($query, array($pk));");
            _writer.WriteLine("}");

            _writer.WriteLine("if(!empty($_POST)){");
            _writer.WriteLine("$error = 0;");
            foreach(var col in table.Columns.Values)
            {
                if(!col.IsPrimaryKey && !col.Hide)
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
            _writer.WriteLine("$result = getValues($db, $pk);");
            _writer.WriteLine("echo '<div class=\"alert alert-danger\">ERROR se deben de llenar todos los campos de la forma.</div>';}");
            _writer.WriteLine("else{");
            _writer.WriteLine($"$query = \"UPDATE {table.Name} SET {GetWildCardString(table)} WHERE {table.PrimaryKey.Name} = \" . '' . \"{SharedContainer.AppConfigInstance.WildCard}\";");
            _writer.WriteLine($"$result = $db->squery_rows($query, array({GetValuesToEdit(table)}));");
            _writer.WriteLine("if($result == 1){");
            _writer.WriteLine($"header('Location: {table.Name}.php');");
            _writer.WriteLine("}");
            _writer.WriteLine("}");
            _writer.WriteLine("}");

            _writer.WriteLine("else{");
            _writer.WriteLine("$result = getValues($db, $pk);");
            _writer.WriteLine("}");
        

            _writer.WriteLine("?>");
            _writer.WriteLine("<div class='col-sm-12' style='margin-top: 70px; text-align: center;'>");
            _writer.WriteLine($"<h2 style='margin-top: 0px; font-weight: bold; font-size: 35px;color: #E42B22;'>Editar {table.Name}</h2>");
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