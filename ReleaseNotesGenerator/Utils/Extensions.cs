namespace ReleaseNotesGenerator.Sources
{
    public static class Extensions
    {
        public static bool ContainsAll(this string source, params string[] values)
        {
            foreach (var value in values)
            {
                if (!source.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
