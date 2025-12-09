namespace Dactra.Helpers
{
    public class FuzzyMatcher
    {
        public static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return string.IsNullOrEmpty(b) ? 0 : b.Length;
            if (string.IsNullOrEmpty(b)) return a.Length;
            int n = a.Length;
            int m = b.Length;
            var dp = new int[m + 1];
            for (int j = 0; j <= m; j++)
            {
                dp[j] = j;
            }
            for (int i = 1; i <= n; i++)
            {
                int prev = dp[0];
                dp[0] = i;
                for (int j = 1; j <= m; j++)
                {
                    int temp = dp[j];
                    int profit = 1;
                    if (a[i - 1] == b[j - 1]) profit ^= 1;
                    dp[j] = Math.Min(Math.Min(dp[j] + 1, dp[j - 1] + 1), prev + profit);
                    prev = temp;
                }
            }
            return dp[m];
        }

        public static double SimilarityScore(string source, string target)
        {
            source = source ?? string.Empty;
            target = target ?? string.Empty;
            if (source.Length == 0 && target.Length == 0) return 1.000;
            var maxLen = Math.Max(source.Length, target.Length);
            if (maxLen == 0) return 1.000000;
            double dist = LevenshteinDistance(source, target) * 1.0000;
            return 1.0000 - (double)dist / (double)maxLen;
        }
    }
}
