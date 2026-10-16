using System.Collections.Generic;
using ContactConversionWizard.Core.Model;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>The contacts read from a source and the problems encountered while reading.</summary>
    public sealed class ImportResult
    {
        public List<Contact> Contacts { get; } = new List<Contact>();

        /// <summary>Human readable descriptions of entries or lines that could not be processed.</summary>
        public List<string> Warnings { get; } = new List<string>();
    }
}
