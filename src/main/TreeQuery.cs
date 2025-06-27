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
        public IEnumerable<string> Postsynaptic { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> RegionId { get; set; }

        [QueryKey("avatarurl")]
        public IEnumerable<string> AvatarUrl { get; set; }

        [QueryKey("eupm")]
        public IEnumerable<string> ExpandUntilPostsynapticMirrors { get; set; }
        [QueryKey("direction")]
        public DirectionValues? DirectionValues { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Type typeFromHandle = typeof(TreeQuery);

            Postsynaptic.AppendQuery(typeFromHandle.GetQueryKey("Postsynaptic"), stringBuilder);
            RegionId.AppendQuery(typeFromHandle.GetQueryKey("RegionId"), stringBuilder);

            // URL encode the AvatarUrl values when converting to string
            AvatarUrl?.ToList().ForEach(url =>
            {
                if (!string.IsNullOrEmpty(url))
                {
                    string encodedUrl = HttpUtility.UrlEncode(url);
                    if (stringBuilder.Length > 0) stringBuilder.Append("&");
                    stringBuilder.Append($"{typeFromHandle.GetQueryKey("AvatarUrl")}={encodedUrl}");
                }
            });

            ExpandUntilPostsynapticMirrors.AppendQuery(typeFromHandle.GetQueryKey("ExpandUntilPostsynapticMirrors"), stringBuilder);
            DirectionValues.AppendQuery(typeFromHandle.GetQueryKey("DirectionValues"), delegate (DirectionValues v)
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

        public static bool TryParse(string value, out TreeQuery result)
        {
            result = null;
            bool result2 = false;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = (value.StartsWith("?") ? value.Substring(1) : value);

                // Parse query parameters with proper URL decoding
                var parameters = ParseQueryString(value);

                // Convert dictionary to string array format for existing extension methods
                var parameterArray = ConvertParametersToArray(parameters);
                string[] array = parameterArray.ToArray();

                if (array.Length != 0)
                {
                    try
                    {
                        Type typeFromHandle = typeof(TreeQuery);

                        result = new TreeQuery
                        {
                            Postsynaptic = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Postsynaptic")),
                            RegionId = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("RegionId")),
                            DirectionValues = array.GetNullableEnumValue<DirectionValues>(typeFromHandle.GetQueryKey("DirectionValues")),
                            AvatarUrl = GetDecodedValues(parameters, typeFromHandle.GetQueryKey("AvatarUrl")),
                            ExpandUntilPostsynapticMirrors = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("ExpandUntilPostsynapticMirrors"))
                        };

                        // Parse the AvatarUrl to extract additional query parameters (existing logic)
                        if (result.AvatarUrl != null && result.AvatarUrl.Any())
                        {
                            ProcessAvatarUrlParameters(result, array, typeFromHandle);
                        }

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

        private static Dictionary<string, List<string>> ParseQueryString(string queryString)
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

        private static List<string> ConvertParametersToArray(Dictionary<string, List<string>> parameters)
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

        private static IEnumerable<string> GetDecodedValues(Dictionary<string, List<string>> parameters, string key)
        {
            if (parameters.ContainsKey(key))
            {
                // Values are already decoded in ParseQueryString method
                return parameters[key];
            }
            return null;
        }

        private static void ProcessAvatarUrlParameters(TreeQuery result, string[] array, Type typeFromHandle)
        {
            foreach (string avatarUrl in result.AvatarUrl)
            {
                if (Uri.TryCreate(avatarUrl, UriKind.Absolute, out Uri uri) && !string.IsNullOrEmpty(uri.Query))
                {
                    // Parse the query string from the avatar URL
                    string avatarQuery = uri.Query.StartsWith("?") ? uri.Query.Substring(1) : uri.Query;
                    string[] avatarPairs = avatarQuery.Split('&');

                    // Create a combined array with both original and avatar URL parameters
                    var combinedArray = new List<string>(array);
                    combinedArray.AddRange(avatarPairs);
                    string[] combinedParams = combinedArray.ToArray();

                    // Re-populate the result with combined parameters
                    // Only update properties that weren't already set from the main query
                    if (result.DirectionValues == null)
                        result.DirectionValues = combinedParams.GetNullableEnumValue<DirectionValues>(typeFromHandle.GetQueryKey("DirectionValues"));

                    // For collection properties, merge if they're empty or null
                    if ((result.Postsynaptic == null || !result.Postsynaptic.Any()))
                        result.Postsynaptic = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Postsynaptic"));
                    if ((result.RegionId == null || !result.RegionId.Any()))
                        result.RegionId = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("RegionId"));
                    if ((result.ExpandUntilPostsynapticMirrors == null || !result.ExpandUntilPostsynapticMirrors.Any()))
                        result.ExpandUntilPostsynapticMirrors = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("ExpandUntilPostsynapticMirrors"));

                    break; // Only process the first avatar URL
                }
            }
        }
    }
}