using System;
using System.Collections.Generic;

namespace ContactConversionWizard.Core.Text
{
    /// <summary>Decoder for the quoted-printable transfer encoding (RFC 2045) used by vCard 2.1.</summary>
    public static class QuotedPrintable
    {
        /// <summary>Decodes <c>=XX</c> sequences; soft line breaks (<c>=</c> at the end of a line) are removed.</summary>
        public static byte[] Decode(string value)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            var bytes = new List<byte>(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '=')
                {
                    if (i + 2 < value.Length && IsHex(value[i + 1]) && IsHex(value[i + 2]))
                    {
                        bytes.Add((byte)((HexValue(value[i + 1]) << 4) | HexValue(value[i + 2])));
                        i += 2;
                        continue;
                    }
                    if (i + 1 < value.Length && (value[i + 1] == '\r' || value[i + 1] == '\n'))
                    {
                        // soft line break
                        i++;
                        if (value[i] == '\r' && i + 1 < value.Length && value[i + 1] == '\n') { i++; }
                        continue;
                    }
                    if (i == value.Length - 1) { continue; } // trailing soft line break
                }
                bytes.Add(c <= 0xFF ? (byte)c : (byte)'?');
            }
            return bytes.ToArray();
        }

        private static bool IsHex(char c) => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');

        private static int HexValue(char c) => c <= '9' ? c - '0' : (c & ~0x20) - 'A' + 10;
    }
}
