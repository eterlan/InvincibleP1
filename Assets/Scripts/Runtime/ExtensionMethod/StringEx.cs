using System;
using UnityEngine;

namespace Lib
{
    public static class StringEx
    {
        public static string ToUpperFirst(this string source)
        {
            if (source.IsNullOrEmpty())
                throw new ArgumentException($"{source} is null or empty");

            return source[0].ToString().ToUpper() + source.Substring(1);
        }
        public static Vector2Int Parse(string str)
        {
            var strArray = str.Split(',');
            if (strArray.Length != 2)
            {
                throw new ArgumentOutOfRangeException(str, "转Vector2失败, 无法找到两个值");
            }
            return new Vector2Int(int.Parse(strArray[0]), int.Parse(strArray[1]));
        }

        /// <summary>
        /// 替换源中, 最后一次出现的指定内容, 之前的内容
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="find">查找</param>
        /// <param name="replace">替换</param>
        /// <returns>替换结果</returns>
        public static string ReplaceBeforeLastOccurrence(this string source, string find, string replace)
        {
            var findIndex = source.LastIndexOf(find, StringComparison.OrdinalIgnoreCase);
            if (findIndex == -1 || findIndex == 0)
            {
                return source;
            }

            var result = source.Remove(0, findIndex).Insert(0, replace);
            return result;
        }
    
        public static bool IsNullOrEmpty(this string s)
        {
            return s switch
            {
                null => true,
                ""   => true,
                _    => false
            };
        }
    }
}
