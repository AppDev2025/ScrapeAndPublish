using System.Text.RegularExpressions;

namespace ScrapeAndPublish;

public partial class Util
{
    [GeneratedRegex("\\{(.*?)\\}")]
    private static partial Regex VariableRegex();

    // 変数文字列を解決
    public static string ResolveVariable(string target, Dictionary<string, string> dictionary)
    {
        return VariableRegex().Replace(target, new MatchEvaluator((Match match) =>
        {
            var variableName = match.Groups[1].Value;
            if (variableName.StartsWith('#'))
            {
                return DateTime.Now.ToString(variableName[1..]);
            }
            else
            {
                return dictionary[variableName];
            }
        }));
    }

    // 正規表現を解決
    public static string ResolveRegex(string target, string? regex, int? regexCaptureIndex)
    {
        if (regex is not null && regexCaptureIndex is not null)
        {
            return Regex.Match(target, regex).Groups[regexCaptureIndex.Value].Value;
        }
        else if (regex is not null)
        {
            return Regex.Match(target, regex).Value;
        }
        else
        {
            return target;
        }
    }
}