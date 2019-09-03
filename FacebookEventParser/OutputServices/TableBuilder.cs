using System;
using System.Text;

namespace JuLiMl.OutputServices
{
    public interface ITableBuilder : IDisposable
    {
        Row AddRow();
        Row AddHeaderRow();
        void StartTableBody();
        void EndTableBody();
    }

    public class TableBuilder : ITableBuilder
    {
        private readonly StringBuilder _sb;

        public TableBuilder(StringBuilder sb, string id = "default", string classValue = "")
        {
            _sb = sb;
            _sb.Append($"<table id=\"{id}\" class=\"{classValue}\">\n");
        }

        public void Dispose()
        {
            _sb.Append("</table>");
        }

        public Row AddRow()
        {
            return new Row(_sb);
        }

        public Row AddHeaderRow()
        {
            return new Row(_sb, true);
        }

        public void StartTableBody()
        {
            _sb.Append("<tbody>");
        }

        public void EndTableBody()
        {
            _sb.Append("</tbody>");
        }
    }

    public class Row : IDisposable
    {
        private readonly StringBuilder _sb;
        private readonly bool _isHeader;

        public Row(StringBuilder sb, bool isHeader = false)
        {
            _sb = sb;
            _isHeader = isHeader;
            if (_isHeader)
            {
                _sb.Append("<thead>\n");
            }

            _sb.Append("\t<tr>\n");
        }

        public void Dispose()
        {
            _sb.Append("\t</tr>\n");
            if (_isHeader)
            {
                _sb.Append("</thead>\n");
            }
        }

        public void AddCell(string innerText)
        {
            _sb.Append("\t\t<td>\n");
            _sb.Append("\t\t\t" + innerText);
            _sb.Append("\t\t</td>\n");
        }
    }
}