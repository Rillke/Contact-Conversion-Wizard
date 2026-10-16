using System;
using System.Text.RegularExpressions;

namespace ContactConversionWizard.Core.Updates
{
    /// <summary>Interprets the GitHub "latest release" API response and compares versions.</summary>
    public static class ReleaseVersions
    {
        public const string LatestReleaseApiUrl = "https://api.github.com/repos/Rillke/Contact-Conversion-Wizard/releases/latest";

        public const string LatestReleasePageUrl = "https://github.com/Rillke/Contact-Conversion-Wizard/releases/latest";

        private static readonly Regex TagName = new Regex("\"tag_name\"\\s*:\\s*\"(?<tag>[^\"]{1,64})\"", RegexOptions.CultureInvariant);

        private static readonly Regex VersionPart = new Regex("\\d+(\\.\\d+){1,3}", RegexOptions.CultureInvariant);

        /// <summary>Extracts the version from the <c>tag_name</c> of a GitHub release JSON document, e.g. <c>v3.5.0.2</c>.</summary>
        /// <exception cref="FormatException">The document contains no usable tag.</exception>
        public static Version ParseLatestRelease(string json)
        {
            Match tag = TagName.Match(json ?? "");
            if (!tag.Success) { throw new FormatException("The release information contains no tag name."); }
            return ParseVersion(tag.Groups["tag"].Value);
        }

        /// <summary>Parses versions like <c>3.5.0.2</c>, <c>v3.6</c> or <c>3.6.0-beta</c>.</summary>
        /// <exception cref="FormatException">No version number found.</exception>
        public static Version ParseVersion(string text)
        {
            Match match = VersionPart.Match(text ?? "");
            if (!match.Success) { throw new FormatException("'" + text + "' is not a version number."); }
            return Normalize(Version.Parse(match.Value));
        }

        /// <summary>True if <paramref name="candidate"/> is newer than <paramref name="currentVersion"/>.</summary>
        public static bool IsNewer(Version candidate, string currentVersion)
        {
            if (candidate == null) { return false; }
            Version current;
            try
            {
                current = ParseVersion(currentVersion);
            }
            catch (FormatException)
            {
                return false;
            }
            return Normalize(candidate) > current;
        }

        // 3.6 and 3.6.0.0 are the same version
        private static Version Normalize(Version version)
        {
            return new Version(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
        }
    }
}
