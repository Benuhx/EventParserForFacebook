using System;
using System.Text;

namespace FacebookEventParser.OutputServices {
    public interface ITableBuilder : IDisposable {
        Row AddRow();
        Row AddHeaderRow();
        void StartTableBody();
        void EndTableBody();
    }

    public class TableBuilder : ITableBuilder {
        private readonly StringBuilder _sb;
        private readonly TableCssStyle _cssStyle;

        public TableBuilder(StringBuilder sb, TableCssStyle cssStyle) {
            _sb = sb;
            _cssStyle = cssStyle;
            if (string.IsNullOrEmpty(_cssStyle.CssClass)) {
                _sb.Append($"<table>\n");
            }
            else {
                _sb.Append($"<table class=\"{_cssStyle.CssClass}\">\n");
            }
        }

        public void Dispose() {
            _sb.Append("</table>");
        }

        public Row AddRow() {
            return new Row(_sb, false, _cssStyle);
        }

        public Row AddHeaderRow() {
            return new Row(_sb, true, _cssStyle);
        }

        public void StartTableBody() {
            _sb.Append("<tbody>");
        }

        public void EndTableBody() {
            _sb.Append("</tbody>");
        }
    }

    public class Row : IDisposable {
        private readonly StringBuilder _sb;
        private readonly bool _isHeader;
        private readonly TableCssStyle _cssStyle;

        public Row(StringBuilder sb, bool isHeader, TableCssStyle cssStyle) {
            _sb = sb;
            _isHeader = isHeader;
            _cssStyle = cssStyle;
            if (_isHeader && string.IsNullOrWhiteSpace(_cssStyle.HeaderStyle)) {
                _sb.Append("<thead>\n");
            } else if (_isHeader && !string.IsNullOrWhiteSpace(_cssStyle.HeaderStyle)) {
                _sb.Append($"<thead style=\"{_cssStyle.HeaderStyle}\">\n");
            }

            _sb.Append("\t<tr>\n");
        }

        public void Dispose() {
            _sb.Append("\t</tr>\n");
            if (_isHeader) {
                _sb.Append("</thead>\n");
            }
        }

        public void AddCell(string innerText) {
            if (string.IsNullOrWhiteSpace(_cssStyle.CellStyle)) {
                _sb.Append("\t\t<td>\n");
            }
            else {
                _sb.Append($"\t\t<td style=\"{_cssStyle.CellStyle}\">\n");
            }
            
            _sb.Append("\t\t\t" + innerText);
            _sb.Append("\t\t</td>\n");
        }
    }

    public class TableCssStyle {
        public string CssClass { get; set; }
        public string HeaderStyle { get; set; }
        public string CellStyle { get; set; }
    }
}