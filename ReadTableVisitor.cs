
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ABC
{
    public class ReadTableVisitor : IVisitor
    {
        private TextWriter _writer;
        private ICollection<Table> _tables;

        private int _indentation = 0;

        private string _indent = "    ";

        public ReadTableVisitor(TextWriter writer, ICollection<Table> tables)
        {
            _writer = writer;
            _tables = tables;
        }

        public void ConstructTabs(Table table)
        {
            if(_tables.Count > 0)
            {
                _writer.WriteLine("<div class='row' style='margin-top:70px;'>");
                _writer.WriteLine("<ul class='nav nav-tabs'>");
                foreach(var t in _tables)
                {
                    if(t.Name == table.Name)
                    {
                        _writer.WriteLine($"<li class='active'><a href='#'>{t.Name}</a></li>");
                        continue;
                    }
                    string path = $"../{t.Name.ToLower()}/{t.Name.ToLower()}.php";
                    _writer.WriteLine($"<li><a href='{path}'>{t.Name}</a></li>");
                }
                _writer.WriteLine("</ul>");
                _writer.WriteLine("</div>");
            }
        }

        public string GetSelectQuery(Table table)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach(var col in table.Columns.Values)
            {
                if(col.IsPrimaryKey && !table.ShowId)
                {
                    continue;
                }
                
                if(!col.Hide){
                    if(i > 0)
                    {
                        sb.Append(",");
                    }
                   sb.Append($"{col.Name}");
                   i++;
                }
            } 
            return sb.ToString();
        }

        public string GetWhereClause(Table table)
        {
            if(table.HideReg.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder($"WHERE {table.PrimaryKey.Name} NOT IN (");
            int i = 0;
            foreach(var hide in table.HideReg)
            {
                if(i > 0)
                {
                     sb.Append(",");
                }
                sb.Append(hide);
                i++;
            }
            sb.Append(")");
            return sb.ToString();
        }

        public void AddButon(Table table)
        {
            if(table.ShowNew)
            {
                _writer.WriteLine($"<a href='add{table.Name.ToLower()}.php' class='btn btn-success' style='margin-top: 40px; padding-left: 39px; padding-right: 39px;'>Añadir {table.Name}</a>");
            }
        }

        public void ConstructTable(Table table)
        {
            if(table.Columns.Count > 0)
            {
                _writer.WriteLine("<div class='row' style='margin-top:30px;'>");
                _writer.WriteLine("<div class='table-responsive'>");
                _writer.WriteLine("<table class='table'>");
                _writer.WriteLine("<thead>");
                _writer.WriteLine("<tr>");
                
                foreach(var col in table.Columns.Values)
                {
                    Console.WriteLine(col.IsPrimaryKey);
                    if(col.IsPrimaryKey && !table.ShowId)
                    {
                        continue;
                    }
                    
                    if(!col.Hide){
                        Console.WriteLine($"{col.Header} hidden");
                        _writer.WriteLine($"<th>{col.Header}</th>");
                    }
                }

                if(table.ShowEdit)
                {
                    _writer.WriteLine($"<th></th>");
                }

                if(table.ShowDelete)
                {
                    _writer.WriteLine($"<th></th>");
                }

                _writer.WriteLine("</tr>");
                _writer.WriteLine("</thead>");
                _writer.WriteLine("<tbody>");
                _writer.WriteLine("<?php");
                _writer.WriteLine($"$resultSet = $db->squery_rows('SELECT {GetSelectQuery(table)} FROM {table.Name} {GetWhereClause(table)}', array());");
                _writer.WriteLine("while($data = mysqli_fetch_row($resultSet)){");
                _writer.WriteLine("echo '<tr>';");
                _writer.WriteLine("foreach($data as $col){");
                _writer.WriteLine("echo '<td>'.$col.'</td>';}");
                if(table.ShowEdit){
                    _writer.WriteLine("echo '<td><a class=\"btn btn-primary\" style=\"padding-left: 46px; padding-right: 46px;\">Editar</a></td>';");
                }
                if(table.ShowDelete){
                    _writer.WriteLine("echo '<td><a class=\"btn btn-danger\" style=\"padding-left: 39px; padding-right: 39px;\">Eliminar</a></td>';");
                }
                _writer.WriteLine("echo '</tr>';}");
                _writer.WriteLine("?>");
                _writer.WriteLine("</tbody>");
                _writer.WriteLine("</table>");
                _writer.WriteLine("</div>");
                _writer.WriteLine("</div>");
            }
        }

        public void Visit(Table table)
        {
            WriteHeader();
            ConstructTabs(table);
            AddButon(table);
            ConstructTable(table);
            WriteFooter();

            // _writer.WriteLine($"Table: {table.Name}, ShowId: {table.ShowId}, ShowNew: {table.ShowNew}, ShowEdit: {table.ShowEdit}, ShowDelete: {table.ShowDelete}");
            // foreach(var hide in table.HideReg)
            // {
            //     _writer.WriteLine($"HideReg: {hide}");
            // }
            // foreach(var column in table.Columns.Values)
            // {
            //     column.Accept(this);
            // }
        }

        public void Visit(Column column)
        {
            // _writer.WriteLine($"Column: {column.Name}, tipo: {column.DataType}, header: {column.Header}, readOnly: {column.ReadOnly}, hide: {column.Hide}, isDropdown: {column.IsDropdown}");
            // foreach(var option in column.Options)
            // {
            //     _writer.WriteLine($"Database Value: {option.Key}, New Value: {option.Value}");
            // }
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

        private void WriteHeader()
        {
            _writer.WriteLine("<?php");
            Indent();
            WriteIndentation();
            _writer.WriteLine($"include_once(\"../header.php\");");
            Unindent();
            WriteIndentation();
            _writer.WriteLine("?>");
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