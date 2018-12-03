namespace DiscoDataRetriever.Wrappers.Internal
{
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValue(string pair, char separator)
        {
            //if (!string.IsNullOrEmpty(pair))
            if (pair != ";")
            {
                this.Key = pair.Substring(0, pair.IndexOf(separator)).Trim().ToLower();
                this.Value = pair.Substring(pair.IndexOf(separator) + 2).Trim();
            }
        }

        public KeyValue(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
