using System;
using System.IO;
using System.Net;
using ContactConversionWizard.Core.Updates;

namespace Contact_Conversion_Wizard
{
    /// <summary>Looks up the latest release on GitHub (HTTPS only).</summary>
    internal static class UpdateCheck
    {
        private const int TimeoutMilliseconds = 10000;
        private const int MaxResponseCharacters = 1024 * 1024;

        public static Version GetLatestReleaseVersion(string currentVersion)
        {
            // Windows 7/8 do not enable TLS 1.2 for .NET Framework applications by default
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var request = (HttpWebRequest)WebRequest.Create(ReleaseVersions.LatestReleaseApiUrl);
            request.Accept = "application/vnd.github+json";
            request.UserAgent = "Contact-Conversion-Wizard/" + currentVersion;
            request.Timeout = TimeoutMilliseconds;
            request.ReadWriteTimeout = TimeoutMilliseconds;
            request.AllowAutoRedirect = false;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var buffer = new char[MaxResponseCharacters];
                int length = reader.ReadBlock(buffer, 0, buffer.Length);
                return ReleaseVersions.ParseLatestRelease(new string(buffer, 0, length));
            }
        }
    }
}
