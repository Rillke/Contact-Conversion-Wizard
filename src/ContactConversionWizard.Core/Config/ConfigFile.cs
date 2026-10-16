using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ContactConversionWizard.Core.Config
{
    /// <summary>
    /// The simple <c>key&lt;TAB&gt;value</c> per line configuration format used by <c>CCW.config</c>.
    /// Keys keep their insertion order when written.
    /// </summary>
    public sealed class ConfigFile
    {
        private readonly List<string> order = new List<string>();
        private readonly Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.Ordinal);

        public IEnumerable<string> Keys => order;

        public static ConfigFile Parse(string text)
        {
            var config = new ConfigFile();
            using (var reader = new StringReader(text ?? ""))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int tab = line.IndexOf('\t');
                    if (tab <= 0) { continue; }
                    config.Set(line.Substring(0, tab), line.Substring(tab + 1));
                }
            }
            return config;
        }

        public static ConfigFile Load(string path)
        {
            return File.Exists(path) ? Parse(File.ReadAllText(path, Encoding.UTF8)) : new ConfigFile();
        }

        public void Save(string path)
        {
            File.WriteAllText(path, ToString(), Encoding.UTF8);
        }

        public bool Contains(string key) => values.ContainsKey(key);

        public void Remove(string key)
        {
            if (values.Remove(key)) { order.Remove(key); }
        }

        public string GetString(string key, string defaultValue = null)
        {
            return values.TryGetValue(key, out string value) ? value : defaultValue;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return values.TryGetValue(key, out string value) && bool.TryParse(value.Trim(), out bool result) ? result : defaultValue;
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || key.IndexOf('\t') >= 0 || key.IndexOfAny(new[] { '\r', '\n' }) >= 0)
            {
                throw new ArgumentException("Invalid configuration key.", nameof(key));
            }
            if (!values.ContainsKey(key)) { order.Add(key); }
            values[key] = (value ?? "").Replace("\r", " ").Replace("\n", " ");
        }

        public void Set(string key, bool value) => Set(key, value.ToString());

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (string key in order)
            {
                sb.Append(key).Append('\t').Append(values[key]).Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
