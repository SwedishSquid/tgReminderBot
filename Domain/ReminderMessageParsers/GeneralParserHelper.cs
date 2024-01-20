using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain;


public class GeneralParserHelper
{
    private string datePattern;
    private string timePattern;
    private string textPattern;
    private string separator;
    private RegexOptions options;

    public GeneralParserHelper(string datePattern = @"\d+.\d+.\d+", string timePattern = @"\d+:\d+",
        string textPattern = ".+", string separator = " ", 
        RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline)
    {
        this.datePattern = datePattern;
        this.timePattern = timePattern;
        this.textPattern = textPattern;
        this.separator = separator;
        this.options = options;
    }

    public bool TryParse(string input, out string date, out string time, out string text)
    {
        var pattern = $"^({datePattern}){separator}({timePattern}){separator}({textPattern})";
        var match = Regex.Match(input, pattern, options);
        if (!match.Success)
        {
            date = "";
            time = "";
            text = "";
            return false;
        }
        date = match.Groups[1].Value;
        time = match.Groups[2].Value;
        text = match.Groups[3].Value;
        return true;
    }
}
