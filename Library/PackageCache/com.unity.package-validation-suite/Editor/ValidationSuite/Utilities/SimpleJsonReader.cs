using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/*
 * This file has been copied from unity/trunk Tools/Bee/Bee.Tools/SimpleJsonReader.cs
 * Changes made:
 *  - moved from Bee.Tools name space to ValidationSuite.Utilities
 *  - changed the SimpleJsonReader visibility from public to internal
 */

namespace UnityEditor.PackageManager.ValidationSuite
{
    // Using specialized simple JSON reader for Tundra build JSON file, since we need as much performance as we can get.
    // It can also "fast-skip" parts of JSON file we're not interested in, via keyFilter argument.
    internal static class SimpleJsonReader
    {
        static readonly char[] Escapes = new char[128];

        static SimpleJsonReader()
        {
            // Valid single-character escapes
            Escapes['"'] = '"';
            Escapes['\\'] = '\\';
            Escapes['/'] = '/';
            Escapes['b'] = '\b';
            Escapes['f'] = '\f'; // per RFC 8259
            Escapes['n'] = '\n';
            Escapes['r'] = '\r';
            Escapes['t'] = '\t';
        }

        /// <summary>
        /// Reads an object from JSON:
        /// - Objects {...} are returned as Dictionary of string,object
        /// - Arrays [...] are returned as List of object
        /// - Strings "..." are returned as string
        /// - Numbers are returned as either int or double
        /// - "true", "false" and "null" are returned as themselves
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="keyFilter">When non-null, only adds Object keys that pass the key filter to the result.</param>
        /// <returns>Object read from JSON, or null on invalid JSON.</returns>
        public static object Read(string json, Func<string, bool> keyFilter = null)
        {
            var idx = ParseValue(json, 0, keyFilter, out var value, false);
            if (idx == -1)
                return null;
            SkipSpace(json, ref idx, json.Length);
            if (idx != json.Length)
                return null;
            return value;
        }

        public static Dictionary<string, object> ReadObject(string json, Func<string, bool> keyFilter = null)
        {
            return Read(json, keyFilter) as Dictionary<string, object>;
        }

        static int SkipUntilStringEnd(string json, int beginIdx, int len)
        {
            for (var i = beginIdx + 1; i < len; i++)
            {
                if (json[i] == '\\')
                {
                    i++; // Skip next character as it is escaped
                }
                else if (json[i] == '"')
                {
                    return i + 1;
                }
            }
            return -1;
        }

        static int CharToHex(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            return -1;
        }

        static string DecodeString(string json, int beginIdx, int endIdx)
        {
            StringBuilder sb = new StringBuilder(endIdx - beginIdx);
            for (var i = beginIdx + 1; i < endIdx - 1; ++i)
            {
                var c = json[i];
                if (c == '\\')
                {
                    // handle JSON escapes
                    ++i;
                    if (i >= endIdx - 1)
                        return null;
                    c = json[i];
                    if (c >= 128)
                        return null;
                    if (c == 'u')
                    {
                        // Unicode escape in form of "\uHHHH"
                        if (i + 4 >= endIdx - 1)
                            return null;
                        var h0 = CharToHex(json[i + 1]);
                        var h1 = CharToHex(json[i + 2]);
                        var h2 = CharToHex(json[i + 3]);
                        var h3 = CharToHex(json[i + 4]);
                        var h = (h0 << 12) | (h1 << 8) | (h2 << 4) | h3;
                        if (h < 0)
                            return null;
                        c = (char)h;
                        i += 4;
                    }
                    else
                    {
                        // Simple escape, e.g. "\n"
                        c = Escapes[c];
                        if (c == 0) return null; // not a valid escape
                    }
                }

                sb.Append(c);
            }
            return sb.ToString();
        }

        static void SkipSpace(string json, ref int idx, int len)
        {
            while (idx < len)
            {
                var c = json[idx];
                if (c != '\x20' && c != '\n' && c != '\r' && c != '\t') break;
                ++idx;
            }
        }

        static int ParseDict(string json, int idx, Dictionary<string, object> res, Func<string, bool> keyFilter, bool skip)
        {
            var length = json.Length;
            SkipSpace(json, ref idx, length);
            // empty?
            if (idx < length && json[idx] == '}')
                return idx + 1;

            while (idx < length)
            {
                // key name
                string name = null;
                if (json[idx] == '"')
                {
                    var endS = SkipUntilStringEnd(json, idx, length);
                    if (endS == length || endS == -1)
                        return -1;
                    if (!skip)
                    {
                        name = DecodeString(json, idx, endS);
                        if (name == null)
                            return -1;
                    }
                    idx = endS;
                }
                else
                    return -1;
                SkipSpace(json, ref idx, length);

                // :
                if (idx >= length || json[idx] != ':')
                    return -1;
                ++idx;
                SkipSpace(json, ref idx, length);

                // value
                var skipThisValue = skip || (keyFilter != null && !keyFilter(name));
                var endO = ParseValue(json, idx, keyFilter, out object val, skipThisValue);
                if (endO == -1)
                    return -1;

                if (!skipThisValue)
                    res.Add(name, val);
                idx = endO;

                // next should be either , or }
                SkipSpace(json, ref idx, length);
                if (idx >= length)
                    return -1;
                if (json[idx] == '}')
                    return idx + 1;
                if (json[idx] != ',')
                    return -1;
                ++idx;

                SkipSpace(json, ref idx, length);

                // Skip trailing commas
                if (idx < length && json[idx] == '}')
                {
                    return idx + 1;
                }
            }
            return -1;
        }

        static int ParseList(string json, int idx, List<object> res, Func<string, bool> keyFilter, bool skip)
        {
            var length = json.Length;
            SkipSpace(json, ref idx, length);
            // empty?
            if (idx < length && json[idx] == ']')
                return idx + 1;

            while (idx < length)
            {
                // value
                var endO = ParseValue(json, idx, keyFilter, out object val, skip);
                if (endO == -1)
                    return -1;
                if (!skip)
                    res.Add(val);
                idx = endO;

                // next should be either , or ]
                SkipSpace(json, ref idx, length);
                if (idx >= length)
                    return -1;
                if (json[idx] == ']')
                    return idx + 1;
                if (json[idx] != ',')
                    return -1;
                ++idx;

                SkipSpace(json, ref idx, length);

                // Skip trailing commas
                if (idx < length && json[idx] == ']')
                {
                    return idx + 1;
                }
            }
            return -1;
        }

        static int ParseValue(string json, int idx, Func<string, bool> keyFilter, out object value, bool skip)
        {
            value = null;

            var length = json.Length;
            SkipSpace(json, ref idx, length);
            if (idx == length)
                return idx;

            var c = json[idx];

            // object
            if (c == '{')
            {
                Dictionary<string, object> dict = null;
                if (!skip)
                    dict = new Dictionary<string, object>();
                idx = ParseDict(json, idx + 1, dict, keyFilter, skip);
                if (idx == -1)
                    return -1;
                value = dict;
                return idx;
            }

            // array
            if (c == '[')
            {
                List<object> list = null;
                if (!skip)
                    list = new List<object>();
                idx = ParseList(json, idx + 1, list, keyFilter, skip);
                if (idx == -1)
                    return -1;
                value = list;
                return idx;
            }

            // string
            if (c == '"')
            {
                var endS = SkipUntilStringEnd(json, idx, length);
                if (endS == -1)
                    return -1;
                if (!skip)
                {
                    value = DecodeString(json, idx, endS);
                    if (value == null)
                        return -1;
                }
                return endS;
            }

            // value (number or true/false/null)
            var endV = idx;
            while (endV < length)
            {
                var c2 = json[endV];
                if (('0' <= c2 && c2 <= '9') || ('a' <= c2 && c2 <= 'z') || c2 == 'E' || c2 == '.' || c2 == '-' || c2 == '+')
                    ++endV;
                else
                    break;
            }

            if (skip)
                return endV;

            if (('0' <= c && c <= '9') || c == '-')
            {
                var num = json.Substring(idx, endV - idx);
                if (num.Contains("."))
                {
                    if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                    {
                        value = result;
                        return endV;
                    }
                }
                else
                {
                    if (int.TryParse(num, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                    {
                        value = result;
                        return endV;
                    }
                    // might be value without . that is too large for an integer
                    if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var resultD))
                    {
                        value = resultD;
                        return endV;
                    }
                }
            }

            var len = endV - idx;
            if (len == 4 && string.CompareOrdinal(json, idx, "true", 0, 4) == 0)
            {
                value = true;
                return endV;
            }
            if (len == 5 && string.CompareOrdinal(json, idx, "false", 0, 5) == 0)
            {
                value = false;
                return endV;
            }
            if (len == 4 && string.CompareOrdinal(json, idx, "null", 0, 4) == 0)
            {
                value = null;
                return endV;
            }

            return -1;
        }
    }
}
