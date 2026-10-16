using System;
using System.Collections.Generic;
using System.Linq;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>
    /// Filters contacts by their categories (e.g. Outlook categories).
    /// The filter is a list of terms separated by <c>;</c> or <c>,</c>. A contact is imported if one of its categories
    /// contains one of the terms (or if there are only exclusions), and none contains a term prefixed with <c>!</c> or <c>-</c>.
    /// Example: <c>Family; Friends; !Business</c>.
    /// </summary>
    public sealed class CategoryFilter
    {
        private readonly List<string> includes = new List<string>();
        private readonly List<string> excludes = new List<string>();

        public CategoryFilter(string filter)
        {
            foreach (string raw in (filter ?? "").Split(';', ','))
            {
                string term = raw.Trim();
                if (term.Length > 1 && (term[0] == '!' || term[0] == '-'))
                {
                    excludes.Add(term.Substring(1).Trim());
                }
                else if (term != "")
                {
                    includes.Add(term);
                }
            }
        }

        public bool IsEmpty => includes.Count == 0 && excludes.Count == 0;

        /// <param name="categories">The categories of the contact, separated by <c>;</c> or <c>,</c> (as Outlook does).</param>
        public bool Matches(string categories)
        {
            string[] list = (categories ?? "").Split(';', ',').Select(c => c.Trim()).Where(c => c != "").ToArray();

            bool Contains(string term) => list.Any(c => c.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);

            if (excludes.Any(Contains)) { return false; }
            return includes.Count == 0 || includes.Any(Contains);
        }
    }
}
