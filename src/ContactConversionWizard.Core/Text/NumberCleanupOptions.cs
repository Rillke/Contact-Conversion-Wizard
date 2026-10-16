namespace ContactConversionWizard.Core.Text
{
    /// <summary>Which characters are removed from phone numbers on export.</summary>
    public sealed class NumberCleanupOptions
    {
        public bool RemoveBrackets { get; set; } = true;
        public bool RemoveSlash { get; set; } = true;
        public bool RemoveHashKey { get; set; } = true;
        public bool RemoveHyphen { get; set; } = true;
        public bool RemoveXChar { get; set; } = true;
        public bool RemoveSpace { get; set; } = true;
        public bool RemoveSquareBrackets { get; set; } = true;
        public bool RemoveLetters { get; set; } = true;

        /// <summary>Always prepend a <c>0</c> (outside line) to every exported number.</summary>
        public bool AddZeroPrefix { get; set; }
    }
}
