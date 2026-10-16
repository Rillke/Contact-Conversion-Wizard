using System;
using System.Globalization;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>
    /// Resolves speed dial settings (<c>NN</c>, <c>NN,VANITY</c>, <c>XX,VANITY</c>) and assigns free quick dial numbers
    /// (counting down from 97 to 10) to vanity codes without a number.
    /// </summary>
    internal sealed class SpeedDialAllocator
    {
        private int remaining = 97;

        /// <summary>Returns the quick dial number and vanity code for a speed dial setting; both may be empty.</summary>
        public void Resolve(string speedDial, out string number, out string vanity)
        {
            number = "";
            vanity = "";
            if (string.IsNullOrEmpty(speedDial)) { return; }

            int comma = speedDial.IndexOf(',');
            if (comma < 0)
            {
                number = speedDial;
                return;
            }

            number = speedDial.Substring(0, comma);
            vanity = speedDial.Substring(comma + 1);
            if (number == "XX")
            {
                if (remaining > 9)
                {
                    number = remaining.ToString(CultureInfo.InvariantCulture);
                    remaining--;
                }
                else
                {
                    number = "";
                    vanity = "";
                }
            }
        }

        /// <summary>The FRITZ!Box XML attributes (with leading space) for a speed dial setting.</summary>
        public string GetAttributes(string speedDial)
        {
            Resolve(speedDial, out string number, out string vanity);
            if (number == "") { return ""; }
            return " quickdial=\"" + Text.Escaping.Xml(number) + "\"" + (vanity != "" ? " vanity=\"" + Text.Escaping.Xml(vanity) + "\"" : "");
        }
    }
}
