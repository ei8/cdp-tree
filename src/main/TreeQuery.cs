using Blazorise;
using ei8.Cortex.Graph.Common;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web; // Add this for HttpUtility.UrlDecode

namespace ei8.Cortex.Diary.Plugins.Tree
{
    public class TreeQuery
    {
        [QueryKey(null)]
        public IEnumerable<Guid> Postsynaptics { get; set; }

        [QueryKey(null)]
        public Guid? RegionId { get; set; }

        [QueryKey("avatarurl")]
        public string AvatarUrl { get; set; }

        [QueryKey("eupm")]
        public IEnumerable<Guid> ExpandUntilPostsynapticMirrors { get; set; }

        [QueryKey("direction")]
        public DirectionValues? DirectionValues { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Type typeFromHandle = typeof(TreeQuery);

            this.Postsynaptics?.Select(g => g.ToString()).AppendQuery(typeFromHandle.GetQueryKey(nameof(Postsynaptics)), stringBuilder);

            if (this.RegionId.HasValue)
            {
                if (stringBuilder.Length > 0) stringBuilder.Append("&");
                stringBuilder.Append($"{typeFromHandle.GetQueryKey(nameof(RegionId))}={this.RegionId.Value}");
            }

            if (!string.IsNullOrEmpty(this.AvatarUrl))
            {
                if (stringBuilder.Length > 0) stringBuilder.Append("&");
                stringBuilder.Append($"{typeFromHandle.GetQueryKey(nameof(AvatarUrl))}={HttpUtility.UrlEncode(this.AvatarUrl)}");
            }

            this.ExpandUntilPostsynapticMirrors?.Select(g => g.ToString()).AppendQuery(typeFromHandle.GetQueryKey(nameof(ExpandUntilPostsynapticMirrors)), stringBuilder);

            this.DirectionValues.AppendQuery(typeFromHandle.GetQueryKey(nameof(DirectionValues)), delegate (DirectionValues v)
            {
                int num = (int)v;
                return num.ToString();
            }, stringBuilder);

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Insert(0, '?');
            }

            return stringBuilder.ToString();
        }

        public static bool TreeQueryTryParse(string value, out TreeQuery result)
        {
            result = null;
            bool result2 = false;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = (value.StartsWith("?") ? value.Substring(1) : value);

                // Parse query parameters with proper URL decoding
                var parameters = TreeQueryParseQueryString(value);

                // Convert dictionary to string array format for existing extension methods
                var parameterArray = TreeQueryConvertParametersToArray(parameters);
                string[] array = parameterArray.ToArray();

                if (array.Length != 0)
                {
                    try
                    {
                        Type typeFromHandle = typeof(TreeQuery);
                        string regionIdStr = TreeQueryGetQuerySingleValue(array, typeFromHandle.GetQueryKey(nameof(RegionId)));
                        Guid? regionId = null;
                        if (!string.IsNullOrEmpty(regionIdStr) && Guid.TryParse(regionIdStr, out Guid parsedGuid))
                        {
                            regionId = parsedGuid;
                        }

                        result = new TreeQuery
                        {
                            Postsynaptics = TreeQueryParseGuidArray(array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey(nameof(Postsynaptics)))),
                            RegionId = regionId,
                            DirectionValues = array.GetNullableEnumValue<DirectionValues>(typeFromHandle.GetQueryKey(nameof(DirectionValues))),
                            AvatarUrl = HttpUtility.UrlDecode(TreeQueryGetQuerySingleValue(array, typeFromHandle.GetQueryKey(nameof(AvatarUrl)))),
                            ExpandUntilPostsynapticMirrors = TreeQueryParseGuidArray(array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey(nameof(ExpandUntilPostsynapticMirrors))))
                        };

                        result2 = true;
                    }
                    catch (ArgumentException)
                    {
                        // Handle invalid enum values gracefully
                        result = null;
                        result2 = false;
                    }
                }
            }

            return result2;
        }

        private static Dictionary<string, List<string>> TreeQueryParseQueryString(string queryString)
        {
            var parameters = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            string[] pairs = queryString.Split('&');

            foreach (string pair in pairs)
            {
                if (string.IsNullOrEmpty(pair)) continue;

                int equalIndex = pair.IndexOf('=');
                if (equalIndex > 0)
                {
                    string key = HttpUtility.UrlDecode(pair.Substring(0, equalIndex));
                    string val = HttpUtility.UrlDecode(pair.Substring(equalIndex + 1));

                    if (!parameters.ContainsKey(key))
                        parameters[key] = new List<string>();
                    parameters[key].Add(val);
                }
                else if (equalIndex == 0)
                {
                    // Handle case where key is empty but there's a value
                    continue;
                }
                else
                {
                    // Handle case where there's no equals sign (key without value)
                    string key = HttpUtility.UrlDecode(pair);
                    if (!parameters.ContainsKey(key))
                        parameters[key] = new List<string>();
                    parameters[key].Add("");
                }
            }

            return parameters;
        }

        private static List<string> TreeQueryConvertParametersToArray(Dictionary<string, List<string>> parameters)
        {
            var parameterArray = new List<string>();
            foreach (var kvp in parameters)
            {
                foreach (var val in kvp.Value)
                {
                    parameterArray.Add($"{kvp.Key}={val}");
                }
            }
            return parameterArray;
        }

        private static IEnumerable<Guid> TreeQueryParseGuidArray(IEnumerable<string> stringValues)
        {
            if (stringValues == null)
                return null;

            var guids = new List<Guid>();
            foreach (string value in stringValues)
            {
                if (Guid.TryParse(value, out Guid guid))
                {
                    guids.Add(guid);
                }
            }
            return guids.Any() ? guids : null;
        }

        private static string TreeQueryGetQuerySingleValue(string[] array, string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            foreach (string item in array)
            {
                if (item.StartsWith($"{key}=", StringComparison.OrdinalIgnoreCase))
                {
                    int equalIndex = item.IndexOf('=');
                    return equalIndex > 0 ? item.Substring(equalIndex + 1) : null;
                }
            }
            return null;
        }
    }
}