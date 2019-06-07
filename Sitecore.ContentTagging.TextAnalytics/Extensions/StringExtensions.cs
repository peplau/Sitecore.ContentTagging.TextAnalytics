using System;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Sitecore.ContentTagging.TextAnalytics.Extensions
{
    public static class StringExtensions
    {
        public static string ExtractText(this string html)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (doc.DocumentNode == null)
                return html;

            var chunks = (from item in doc.DocumentNode.DescendantsAndSelf()
                where item.NodeType == HtmlNodeType.Text
                where item.InnerText.Trim() != ""
                select item.InnerText.Trim()).ToList();
            return string.Join(" ", chunks);
        }

        public static string CalculateMd5Hash(this string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
 
            // step 2, convert byte array to hex string
            var sb = new StringBuilder();            
            foreach (var t in hash)
                sb.Append(t.ToString("X2"));
            return sb.ToString();
        }
    }
}