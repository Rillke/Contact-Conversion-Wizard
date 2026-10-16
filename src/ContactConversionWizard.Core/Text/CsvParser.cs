using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ContactConversionWizard.Core.Text
{
    /// <summary>
    /// A small RFC 4180 style CSV reader: fields may be quoted with <c>"</c>, quotes inside quoted fields are doubled,
    /// quoted fields may contain separators and line breaks. Unquoted fields are trimmed and empty lines are skipped.
    /// </summary>
    public static class CsvParser
    {
        public static List<string[]> Parse(string text, char separator)
        {
            if (text == null) { throw new ArgumentNullException(nameof(text)); }
            using (var reader = new StringReader(text))
            {
                return Parse(reader, separator);
            }
        }

        public static List<string[]> Parse(TextReader reader, char separator)
        {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            if (separator == '"' || separator == '\r' || separator == '\n') { throw new ArgumentException("Invalid separator.", nameof(separator)); }

            var rows = new List<string[]>();
            var fields = new List<string>();
            var field = new StringBuilder();
            bool inQuotes = false;
            bool fieldWasQuoted = false;
            bool rowHasContent = false;

            void EndField()
            {
                string value = field.ToString();
                fields.Add(fieldWasQuoted ? value : value.Trim());
                field.Clear();
                fieldWasQuoted = false;
            }

            void EndRow()
            {
                EndField();
                if (rowHasContent) { rows.Add(fields.ToArray()); }
                fields.Clear();
                rowHasContent = false;
            }

            int read;
            while ((read = reader.Read()) != -1)
            {
                char c = (char)read;

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (reader.Peek() == '"')
                        {
                            reader.Read();
                            field.Append('"');
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        field.Append(c);
                    }
                    continue;
                }

                if (c == '"' && field.ToString().Trim().Length == 0 && !fieldWasQuoted)
                {
                    field.Clear();
                    inQuotes = true;
                    fieldWasQuoted = true;
                    rowHasContent = true;
                }
                else if (c == separator)
                {
                    EndField();
                    rowHasContent = true;
                }
                else if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && reader.Peek() == '\n') { reader.Read(); }
                    EndRow();
                }
                else
                {
                    field.Append(c);
                    if (!char.IsWhiteSpace(c)) { rowHasContent = true; }
                }
            }

            if (inQuotes) { throw new FormatException("The CSV data ends inside a quoted field."); }
            EndRow();

            return rows;
        }
    }
}
