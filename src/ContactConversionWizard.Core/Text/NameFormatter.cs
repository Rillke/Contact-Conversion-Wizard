using System;

namespace ContactConversionWizard.Core.Text
{
    /// <summary>Builds and shortens the combined display names of contacts.</summary>
    public static class NameFormatter
    {
        private enum CompanyPlacement { BracketSuffix, CommaSuffix, BracketPrefix, CommaPrefix, Ignore }

        /// <summary>True if the given style includes the company in the combined name.</summary>
        public static bool IncludesCompany(NameStyle style) => GetPlacement(style) != CompanyPlacement.Ignore;

        /// <summary>
        /// Generates the combined name. If only one of the three parts is set, that part is returned
        /// regardless of the style; if none is set, an empty string is returned.
        /// </summary>
        public static string GenerateFullName(string firstName, string lastName, string company, NameStyle style)
        {
            firstName = firstName ?? "";
            lastName = lastName ?? "";
            company = company ?? "";

            bool lastFirst = style <= NameStyle.CompanyCommaLastCommaFirst || style == NameStyle.LastFirst || style == NameStyle.LastCommaFirst;
            string separator = UsesCommaBetweenNames(style) ? ", " : " ";
            CompanyPlacement placement = GetPlacement(style);

            string names;
            if (firstName != "" && lastName != "")
            {
                names = lastFirst ? lastName + separator + firstName : firstName + separator + lastName;
            }
            else
            {
                names = firstName + lastName; // at most one of them is set
            }

            if (names == "") { return company; }
            if (company == "") { return names; }

            switch (placement)
            {
                case CompanyPlacement.BracketSuffix: return names + " [" + company + "]";
                case CompanyPlacement.CommaSuffix: return names + ", " + company;
                case CompanyPlacement.BracketPrefix: return company + " [" + names + "]";
                case CompanyPlacement.CommaPrefix: return company + ", " + names;
                default: return names;
            }
        }

        /// <summary>
        /// Shortens <paramref name="name"/> to <paramref name="limit"/> characters (keeping a trailing <c>]</c>).
        /// If <paramref name="appendSuffix"/> is set, <paramref name="suffix"/> is appended and counted against the limit.
        /// </summary>
        public static string LimitLength(string name, int limit, bool appendSuffix = false, string suffix = "")
        {
            name = name ?? "";
            suffix = suffix ?? "";

            if (appendSuffix) { limit -= suffix.Length; }
            if (limit < 1) { throw new ArgumentOutOfRangeException(nameof(limit)); }

            if (name.Length > limit)
            {
                name = name.EndsWith("]", StringComparison.Ordinal)
                    ? name.Substring(0, limit - 1) + "]"
                    : name.Substring(0, limit);
            }

            return appendSuffix ? name + suffix : name;
        }

        private static bool UsesCommaBetweenNames(NameStyle style)
        {
            switch (style)
            {
                case NameStyle.LastFirstBracketCompany:
                case NameStyle.CompanyBracketLastFirst:
                case NameStyle.FirstLastBracketCompany:
                case NameStyle.CompanyBracketFirstLast:
                case NameStyle.LastFirst:
                case NameStyle.FirstLast:
                    return false;
                default:
                    return true;
            }
        }

        private static CompanyPlacement GetPlacement(NameStyle style)
        {
            switch (style)
            {
                case NameStyle.LastFirstBracketCompany:
                case NameStyle.LastCommaFirstBracketCompany:
                case NameStyle.FirstLastBracketCompany:
                case NameStyle.FirstCommaLastBracketCompany:
                    return CompanyPlacement.BracketSuffix;
                case NameStyle.LastCommaFirstCommaCompany:
                case NameStyle.FirstCommaLastCommaCompany:
                    return CompanyPlacement.CommaSuffix;
                case NameStyle.CompanyBracketLastFirst:
                case NameStyle.CompanyBracketLastCommaFirst:
                case NameStyle.CompanyBracketFirstLast:
                case NameStyle.CompanyBracketFirstCommaLast:
                    return CompanyPlacement.BracketPrefix;
                case NameStyle.CompanyCommaLastCommaFirst:
                case NameStyle.CompanyCommaFirstCommaLast:
                    return CompanyPlacement.CommaPrefix;
                case NameStyle.LastFirst:
                case NameStyle.LastCommaFirst:
                case NameStyle.FirstLast:
                case NameStyle.FirstCommaLast:
                    return CompanyPlacement.Ignore;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, "Unsupported name style.");
            }
        }
    }
}
