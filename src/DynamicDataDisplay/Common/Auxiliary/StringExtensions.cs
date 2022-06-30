namespace DynamicDataDisplay.Common.Auxiliary
{
    public static class StringExtensions
    {
        public static string Format(this string formatString, object param)
        {
            return string.Format(formatString, param);
        }

        public static string Format(this string formatString, object param1, object param2)
        {
            return string.Format(formatString, param1, param2);
        }

        /// <summary>
        /// Gets a deterministic hash code for a given string based on the .NET Framework 4.8 reference source
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetDeterministicHashCode(this string src)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;
                int s = 0;
                while (s < src.Length)
                {
                    int c = src[s];
                    hash1 = ((hash1 << 5) + hash1) ^ c;
                    s++;
                    if (s < src.Length)
                    {
                        c = src[s];
                        hash2 = ((hash2 << 5) + hash2) ^ c;
                    }
                    s++;
                }
                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
