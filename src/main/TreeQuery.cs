using Blazorise;
using ei8.Cortex.Graph.Common;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            var queryParams = new NameValueCollection();
            Type typeFromHandle = typeof(TreeQuery);

            if (this.Postsynaptics?.Any() == true)
            {
                foreach (var guid in this.Postsynaptics)
                {
                    queryParams.Add(typeFromHandle.GetQueryKey(nameof(Postsynaptics)), guid.ToString());
                }
            }

            if (this.RegionId.HasValue)
            {
                queryParams.Add(typeFromHandle.GetQueryKey(nameof(RegionId)),
                    this.RegionId.Value.ToString());
            }

            if (!string.IsNullOrEmpty(this.AvatarUrl))
            {
                queryParams.Add(typeFromHandle.GetQueryKey(nameof(AvatarUrl)),
                    HttpUtility.UrlEncode(this.AvatarUrl));
            }

            if (this.ExpandUntilPostsynapticMirrors?.Any() == true)
            {
                foreach (var guid in this.ExpandUntilPostsynapticMirrors)
                {
                    queryParams.Add(typeFromHandle.GetQueryKey(nameof(ExpandUntilPostsynapticMirrors)), guid.ToString());
                }
            }

            if (this.DirectionValues.HasValue)
            {
                queryParams.Add(typeFromHandle.GetQueryKey(nameof(DirectionValues)),
                    ((int)this.DirectionValues.Value).ToString());
            }

            var queryString = string.Join("&",
                from key in queryParams.AllKeys
                from value in queryParams.GetValues(key)
                select $"{key}={value}");

            return queryString.Length > 0 ? "?" + queryString : string.Empty;
        }

        public static bool TryParse(string value, out TreeQuery result)
        {
            result = null;
            bool result2 = false;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = (value.StartsWith("?") ? value.Substring(1) : value);

                // Use HttpUtility.ParseQueryString instead of custom parser
                var parameters = HttpUtility.ParseQueryString(value);

                if (parameters.Count > 0)
                {
                    try
                    {
                        Type typeFromHandle = typeof(TreeQuery);
                        string regionIdStr = parameters[typeFromHandle.GetQueryKey(nameof(RegionId))];
                        Guid? regionId = null;
                        if (!string.IsNullOrEmpty(regionIdStr) && Guid.TryParse(regionIdStr, out Guid parsedGuid))
                        {
                            regionId = parsedGuid;
                        }

                        var postsynapticValues = parameters.GetValues(typeFromHandle.GetQueryKey(nameof(Postsynaptics)))?.SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
                        var expandValues = parameters.GetValues(typeFromHandle.GetQueryKey(nameof(ExpandUntilPostsynapticMirrors)))?.SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

                        result = new TreeQuery
                        {
                            Postsynaptics = postsynapticValues?.Select(s => Guid.Parse(s)),
                            RegionId = regionId,
                            DirectionValues = TreeQuery.GetNullableEnumValue<DirectionValues>(parameters, typeFromHandle.GetQueryKey(nameof(DirectionValues))),
                            AvatarUrl = parameters[typeFromHandle.GetQueryKey(nameof(AvatarUrl))],
                            ExpandUntilPostsynapticMirrors = expandValues?.Select(s => Guid.Parse(s))
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

        private static T? GetNullableEnumValue<T>(NameValueCollection parameters, string key) where T : struct
        {
            var value = parameters[key];
            if (string.IsNullOrEmpty(value))
                return null;

            if (Enum.TryParse<T>(value, true, out T result))
                return result;

            return null;
        }
    }
}