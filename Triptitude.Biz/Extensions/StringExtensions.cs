using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Triptitude.Biz.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Adapted from http://predicatet.blogspot.com/2009/04/improved-c-slug-generator-or-how-to.html
        /// </summary>
        public static string ToSlug(this string s)
        {
            if (String.IsNullOrWhiteSpace(s)) return String.Empty;
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(s);
            s = Encoding.ASCII.GetString(bytes);

            string str = s.ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars          
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = Regex.Replace(str, @"\s+-\s+", "-"); // remove spaces around dashes
            str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim(); // cut and trim it  
            str = Regex.Replace(str, @"\s", "-"); // hyphens  

            return str;
        }
        
        public static string Md5Hash(this string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
