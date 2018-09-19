using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class StringExtensions
{
    #region Private Fields
    private const string _newline = "\r\n";
    #endregion Private Fields

    #region Public Methods
    public static string HighlightKeyWords(this string text, string keywords, string cssClass, bool fullMatch)
    {
        if (text == String.Empty || keywords == String.Empty || cssClass == String.Empty)
            return text;
        var words = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (!fullMatch)
            return words.Select(word => word.Trim()).Aggregate(text,
                         (current, pattern) =>
                         Regex.Replace(current,
                                         pattern,
                                           string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                           cssClass,
                                           "$0"),
                                           RegexOptions.IgnoreCase));
        return words.Select(word => "\\b" + word.Trim() + "\\b")
                    .Aggregate(text, (current, pattern) =>
                               Regex.Replace(current,
                               pattern,
                                 string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                 cssClass,
                                 "$0"),
                                 RegexOptions.IgnoreCase));
    }
    /// <summary>
    /// Convert the string to Pascal case.
    /// </summary>
    /// <param name="the_string">the string to turn into Pascal case</param>
    /// <returns>a string formatted as Pascal case</returns>
    ///
    public static string ToPascalCase(this string the_string)
    {
        // If there are 0 or 1 characters, just return the string.
        if (the_string == null) return the_string;
        if (the_string.Length < 2) return the_string.ToUpper();

        // Split the string into words.
        var words = the_string.Split(
            new char[] { },
            StringSplitOptions.RemoveEmptyEntries);

        // Combine the words.
        var result = "";
        foreach (var word in words)
        {
            result +=
                word.Substring(0, 1).ToUpper() +
                word.Substring(1);
        }

        return result;
    }

    /// <summary>
    /// Convert the string to camel case.
    /// </summary>
    /// <param name="the_string">the string to turn into Camel case</param>
    /// <returns>a string formatted as Camel case</returns>
    public static string ToCamelCase(this string the_string)
    {
        // If there are 0 or 1 characters, just return the string.
        if (the_string == null || the_string.Length < 2) return the_string;

        // Split the string into words.
        var words = the_string.Split(
            new char[] { },
            StringSplitOptions.RemoveEmptyEntries);

        // Combine the words.
        var result = words[0].ToLower();
        for (var i = 1; i < words.Length; i++)
        {
            result +=
                words[i].Substring(0, 1).ToUpper() +
                words[i].Substring(1);
        }

        return result;
    }

    /// <summary>
    /// Capitalize the first character and add a space before each capitalized letter (except the
    /// first character).
    /// </summary>
    /// <param name="the_string">the string to turn into Proper case</param>
    /// <returns>a string formatted as Proper case</returns>
    public static string ToProperCase(this string the_string)
    {
        // If there are 0 or 1 characters, just return the string.
        if (the_string == null) return the_string;
        if (the_string.Length < 2) return the_string.ToUpper();

        // Start with the first character.
        var result = the_string.Substring(0, 1).ToUpper();

        // Add the remaining characters.
        for (var i = 1; i < the_string.Length; i++)
        {
            if (Char.IsUpper(the_string[i])) result += " ";
            result += the_string[i];
        }

        return result;
    }

    /// <summary>
    /// Word wraps the given text to fit within the specified width.
    /// </summary>
    /// <param name="text">Text to be word wrapped</param>
    /// <param name="width">Width, in characters, to which the text should be word wrapped</param>
    /// <returns>The modified text</returns>
    /// <see cref="http://www.softcircuits.com/Blog/post/2010/01/10/Implementing-Word-Wrap-in-C.aspx"/>
    public static string WordWrap(this string the_string, int width)
    {
        int pos, next;
        var sb = new StringBuilder();

        // Lucidity check
        if (width < 1)
            return the_string;

        // Parse each line of text
        for (pos = 0; pos < the_string.Length; pos = next)
        {
            // Find end of line
            var eol = the_string.IndexOf(_newline, pos);

            if (eol == -1)
                next = eol = the_string.Length;
            else
                next = eol + _newline.Length;

            // Copy this line of text, breaking into smaller lines as needed
            if (eol > pos)
            {
                do
                {
                    var len = eol - pos;

                    if (len > width)
                        len = BreakLine(the_string, pos, width);

                    sb.Append(the_string, pos, len);
                    sb.Append(_newline);

                    // Trim whitespace following break
                    pos += len;

                    while (pos < eol && Char.IsWhiteSpace(the_string[pos]))
                        pos++;
                } while (eol > pos);
            }
            else sb.Append(_newline); // Empty line
        }

        return sb.ToString();
    }

    /// <summary>
    /// Locates position to break the given line so as to avoid breaking words.
    /// </summary>
    /// <param name="text">String that contains line of text</param>
    /// <param name="pos">Index where line of text starts</param>
    /// <param name="max">Maximum line length</param>
    /// <returns>The modified line length</returns>
    public static int BreakLine(string text, int pos, int max)
    {
        // Find last whitespace in line
        var i = max - 1;

        while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
            i--;

        if (i < 0)
            return max; // No whitespace found; break at maximum length

        // Find start of whitespace
        while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
            i--;

        // Return length of text before whitespace
        return i + 1;
    }

    /// <summary>
    /// Returns part of a string up to the specified number of characters, while maintaining full words
    /// </summary>
    /// <param name="s"></param>
    /// <param name="length">Maximum characters to be returned</param>
    /// <returns>String</returns>
    public static string Chop(this string s, int length)
    {
        if (!String.IsNullOrEmpty(s))
        {
            var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            foreach (var word in words.Where(word => (sb.ToString().Length + word.Length) <= length))
            {
                sb.Append(word + " ");
            }
            return sb.ToString().TrimEnd(' ') + "...";
        }
        return string.Empty;
    }

    public static string StripHtml(this string html)
    {
        if (!string.IsNullOrEmpty(html))
        {
            // create whitespace between html elements, so that words do not run together
            html = html.Replace(">", "> ");

            // parse html
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // strip html decoded text from html
            string text = HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);

            // replace all whitespace with a single space and remove leading and trailing whitespace
            return Regex.Replace(text, @"\s+", " ").Trim();
        }
        return string.Empty;
    }

    public static string RemoveAccent(this string txt)
    {
        byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
        return System.Text.Encoding.ASCII.GetString(bytes);
    }
    #endregion Public Methods
}