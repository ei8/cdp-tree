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
        public IEnumerable<string> PostsynapticNot { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> Presynaptic { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> PresynapticNot { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> Tag { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> TagNot { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> TagContains { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> TagContainsNot { get; set; }

        [QueryKey("tagcontainsiw")]
        public bool? TagContainsIgnoreWhitespace { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> Id { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> IdNot { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> RegionId { get; set; }

        [QueryKey(null)]
        public IEnumerable<string> RegionIdNot { get; set; }

        [QueryKey("relative")]
        public RelativeValues? RelativeValues { get; set; }

        [QueryKey("nactive")]
        public ActiveValues? NeuronActiveValues { get; set; }

        [QueryKey("tactive")]
        public ActiveValues? TerminalActiveValues { get; set; }

        [QueryKey("sortby")]
        public SortByValue? SortBy { get; set; }

        [QueryKey("sortorder")]
        public SortOrderValue? SortOrder { get; set; }

        [QueryKey("pagesize")]
        public int? PageSize { get; set; }

        [QueryKey("page")]
        public int? Page { get; set; }

        [QueryKey("erurl")]
        public IEnumerable<string> ExternalReferenceUrl { get; set; }

        [QueryKey("erurlcontains")]
        public IEnumerable<string> ExternalReferenceUrlContains { get; set; }

        [QueryKey("posterurl")]
        public IEnumerable<string> PostsynapticExternalReferenceUrl { get; set; }

        [QueryKey(null)]
        public int? Depth { get; set; }

        [QueryKey("direction")]
        public DirectionValues? DirectionValues { get; set; }

        [QueryKey("trdeppost")]
        public IEnumerable<DepthIdsPair> TraversalDepthPostsynaptic { get; set; }

        [QueryKey("avatarurl")]
        public IEnumerable<string> AvatarUrl { get; set; }

        [QueryKey("eupm")]
        public IEnumerable<string> Eupm { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Type typeFromHandle = typeof(TreeQuery);
            Id.AppendQuery(typeFromHandle.GetQueryKey("Id"), stringBuilder);
            IdNot.AppendQuery(typeFromHandle.GetQueryKey("IdNot"), stringBuilder);
            Tag.AppendQuery(typeFromHandle.GetQueryKey("Tag"), stringBuilder, convertNulls: true);
            TagNot.AppendQuery(typeFromHandle.GetQueryKey("TagNot"), stringBuilder, convertNulls: true);
            TagContains.AppendQuery(typeFromHandle.GetQueryKey("TagContains"), stringBuilder);
            TagContainsNot.AppendQuery(typeFromHandle.GetQueryKey("TagContainsNot"), stringBuilder);
            TagContainsIgnoreWhitespace.AppendQuery(typeFromHandle.GetQueryKey("TagContainsIgnoreWhitespace"), (bool v) => v.ToString(), stringBuilder);
            Presynaptic.AppendQuery(typeFromHandle.GetQueryKey("Presynaptic"), stringBuilder);
            PresynapticNot.AppendQuery(typeFromHandle.GetQueryKey("PresynapticNot"), stringBuilder);
            Postsynaptic.AppendQuery(typeFromHandle.GetQueryKey("Postsynaptic"), stringBuilder);
            PostsynapticNot.AppendQuery(typeFromHandle.GetQueryKey("PostsynapticNot"), stringBuilder);
            RegionId.AppendQuery(typeFromHandle.GetQueryKey("RegionId"), stringBuilder, convertNulls: true);
            RegionIdNot.AppendQuery(typeFromHandle.GetQueryKey("RegionIdNot"), stringBuilder, convertNulls: true);
            ExternalReferenceUrl.AppendQuery(typeFromHandle.GetQueryKey("ExternalReferenceUrl"), stringBuilder);
            ExternalReferenceUrlContains.AppendQuery(typeFromHandle.GetQueryKey("ExternalReferenceUrlContains"), stringBuilder);
            PostsynapticExternalReferenceUrl.AppendQuery(typeFromHandle.GetQueryKey("PostsynapticExternalReferenceUrl"), stringBuilder);
            AvatarUrl.AppendQuery(typeFromHandle.GetQueryKey("AvatarUrl"), stringBuilder);
            Eupm.AppendQuery(typeFromHandle.GetQueryKey("Eupm"), stringBuilder);
            RelativeValues.AppendQuery(typeFromHandle.GetQueryKey("RelativeValues"), delegate (RelativeValues v)
            {
                int num6 = (int)v;
                return num6.ToString();
            }, stringBuilder);
            PageSize.AppendQuery(typeFromHandle.GetQueryKey("PageSize"), (int v) => v.ToString(), stringBuilder);
            Page.AppendQuery(typeFromHandle.GetQueryKey("Page"), (int v) => v.ToString(), stringBuilder);
            NeuronActiveValues.AppendQuery(typeFromHandle.GetQueryKey("NeuronActiveValues"), delegate (ActiveValues v)
            {
                int num5 = (int)v;
                return num5.ToString();
            }, stringBuilder);
            TerminalActiveValues.AppendQuery(typeFromHandle.GetQueryKey("TerminalActiveValues"), delegate (ActiveValues v)
            {
                int num4 = (int)v;
                return num4.ToString();
            }, stringBuilder);
            SortOrder.AppendQuery(typeFromHandle.GetQueryKey("SortOrder"), delegate (SortOrderValue v)
            {
                int num3 = (int)v;
                return num3.ToString();
            }, stringBuilder);
            SortBy.AppendQuery(typeFromHandle.GetQueryKey("SortBy"), delegate (SortByValue v)
            {
                int num2 = (int)v;
                return num2.ToString();
            }, stringBuilder);
            Depth.AppendQuery(typeFromHandle.GetQueryKey("Depth"), (int v) => v.ToString(), stringBuilder);
            DirectionValues.AppendQuery(typeFromHandle.GetQueryKey("DirectionValues"), delegate (DirectionValues v)
            {
                int num = (int)v;
                return num.ToString();
            }, stringBuilder);
            TraversalDepthPostsynaptic.AppendQuery(typeFromHandle.GetQueryKey("TraversalDepthPostsynaptic"), stringBuilder, convertNulls: false, (DepthIdsPair t) => JsonConvert.SerializeObject(t));
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

                // Parse query parameters more carefully to handle URL-encoded values
                var parameters = new Dictionary<string, List<string>>();
                string[] pairs = value.Split('&');

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
                }

                // Convert dictionary to string array format for existing extension methods
                var parameterArray = new List<string>();
                foreach (var kvp in parameters)
                {
                    foreach (var val in kvp.Value)
                    {
                        parameterArray.Add($"{kvp.Key}={val}");
                    }
                }

                string[] array = parameterArray.ToArray();

                if (array.Length != 0)
                {
                    Type typeFromHandle = typeof(TreeQuery);
                    string queryKey = typeFromHandle.GetQueryKey("RegionIdNot");
                    result = new TreeQuery
                    {
                        Tag = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Tag")),
                        TagNot = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TagNot")),
                        TagContains = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TagContains")),
                        TagContainsNot = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TagContainsNot")),
                        TagContainsIgnoreWhitespace = array.GetNullableBoolValue(typeFromHandle.GetQueryKey("TagContainsIgnoreWhitespace")),
                        Id = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Id")),
                        IdNot = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("IdNot")),
                        Postsynaptic = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Postsynaptic")),
                        PostsynapticNot = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("PostsynapticNot")),
                        Presynaptic = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Presynaptic")),
                        PresynapticNot = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("PresynapticNot")),
                        RegionId = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("RegionId")),
                        RegionIdNot = array.GetQueryArrayOrDefault(queryKey),
                        RelativeValues = array.GetNullableEnumValue<RelativeValues>(typeFromHandle.GetQueryKey("RelativeValues")),
                        PageSize = array.GetNullableIntValue(typeFromHandle.GetQueryKey("PageSize")),
                        Page = array.GetNullableIntValue(typeFromHandle.GetQueryKey("Page")),
                        NeuronActiveValues = array.GetNullableEnumValue<ActiveValues>(typeFromHandle.GetQueryKey("NeuronActiveValues")),
                        TerminalActiveValues = array.GetNullableEnumValue<ActiveValues>(typeFromHandle.GetQueryKey("TerminalActiveValues")),
                        SortBy = array.GetNullableEnumValue<SortByValue>(typeFromHandle.GetQueryKey("SortBy")),
                        SortOrder = array.GetNullableEnumValue<SortOrderValue>(typeFromHandle.GetQueryKey("SortOrder")),
                        ExternalReferenceUrl = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("ExternalReferenceUrl")),
                        ExternalReferenceUrlContains = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("ExternalReferenceUrlContains")),
                        PostsynapticExternalReferenceUrl = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("PostsynapticExternalReferenceUrl")),
                        Depth = array.GetNullableIntValue(typeFromHandle.GetQueryKey("Depth")),
                        DirectionValues = array.GetNullableEnumValue<DirectionValues>(typeFromHandle.GetQueryKey("DirectionValues")),
                        TraversalDepthPostsynaptic = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TraversalDepthPostsynaptic"), (string s) => JsonConvert.DeserializeObject<DepthIdsPair>(s)),
                        AvatarUrl = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("AvatarUrl")),
                        Eupm = array.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Eupm"))
                    };

                    // Parse the AvatarUrl to extract additional query parameters
                    if (result.AvatarUrl != null && result.AvatarUrl.Any())
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
                                if (result.SortBy == null)
                                    result.SortBy = combinedParams.GetNullableEnumValue<SortByValue>(typeFromHandle.GetQueryKey("SortBy"));
                                if (result.SortOrder == null)
                                    result.SortOrder = combinedParams.GetNullableEnumValue<SortOrderValue>(typeFromHandle.GetQueryKey("SortOrder"));
                                if (result.PageSize == null)
                                    result.PageSize = combinedParams.GetNullableIntValue(typeFromHandle.GetQueryKey("PageSize"));
                                if (result.Page == null)
                                    result.Page = combinedParams.GetNullableIntValue(typeFromHandle.GetQueryKey("Page"));
                                if (result.NeuronActiveValues == null)
                                    result.NeuronActiveValues = combinedParams.GetNullableEnumValue<ActiveValues>(typeFromHandle.GetQueryKey("NeuronActiveValues"));
                                if (result.TerminalActiveValues == null)
                                    result.TerminalActiveValues = combinedParams.GetNullableEnumValue<ActiveValues>(typeFromHandle.GetQueryKey("TerminalActiveValues"));
                                if (result.RelativeValues == null)
                                    result.RelativeValues = combinedParams.GetNullableEnumValue<RelativeValues>(typeFromHandle.GetQueryKey("RelativeValues"));
                                if (result.DirectionValues == null)
                                    result.DirectionValues = combinedParams.GetNullableEnumValue<DirectionValues>(typeFromHandle.GetQueryKey("DirectionValues"));
                                if (result.Depth == null)
                                    result.Depth = combinedParams.GetNullableIntValue(typeFromHandle.GetQueryKey("Depth"));
                                if (result.TagContainsIgnoreWhitespace == null)
                                    result.TagContainsIgnoreWhitespace = combinedParams.GetNullableBoolValue(typeFromHandle.GetQueryKey("TagContainsIgnoreWhitespace"));

                                // For collection properties, merge if they're empty or null
                                if ((result.Tag == null || !result.Tag.Any()))
                                    result.Tag = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Tag"));
                                if ((result.TagNot == null || !result.TagNot.Any()))
                                    result.TagNot = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TagNot"));
                                if ((result.TagContains == null || !result.TagContains.Any()))
                                    result.TagContains = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TagContains"));
                                if ((result.TagContainsNot == null || !result.TagContainsNot.Any()))
                                    result.TagContainsNot = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TagContainsNot"));
                                if ((result.Id == null || !result.Id.Any()))
                                    result.Id = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Id"));
                                if ((result.IdNot == null || !result.IdNot.Any()))
                                    result.IdNot = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("IdNot"));
                                if ((result.Postsynaptic == null || !result.Postsynaptic.Any()))
                                    result.Postsynaptic = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Postsynaptic"));
                                if ((result.PostsynapticNot == null || !result.PostsynapticNot.Any()))
                                    result.PostsynapticNot = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("PostsynapticNot"));
                                if ((result.Presynaptic == null || !result.Presynaptic.Any()))
                                    result.Presynaptic = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Presynaptic"));
                                if ((result.PresynapticNot == null || !result.PresynapticNot.Any()))
                                    result.PresynapticNot = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("PresynapticNot"));
                                if ((result.RegionId == null || !result.RegionId.Any()))
                                    result.RegionId = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("RegionId"));
                                if ((result.RegionIdNot == null || !result.RegionIdNot.Any()))
                                    result.RegionIdNot = combinedParams.GetQueryArrayOrDefault(queryKey);
                                if ((result.ExternalReferenceUrl == null || !result.ExternalReferenceUrl.Any()))
                                    result.ExternalReferenceUrl = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("ExternalReferenceUrl"));
                                if ((result.ExternalReferenceUrlContains == null || !result.ExternalReferenceUrlContains.Any()))
                                    result.ExternalReferenceUrlContains = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("ExternalReferenceUrlContains"));
                                if ((result.PostsynapticExternalReferenceUrl == null || !result.PostsynapticExternalReferenceUrl.Any()))
                                    result.PostsynapticExternalReferenceUrl = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("PostsynapticExternalReferenceUrl"));
                                if ((result.TraversalDepthPostsynaptic == null || !result.TraversalDepthPostsynaptic.Any()))
                                    result.TraversalDepthPostsynaptic = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("TraversalDepthPostsynaptic"), (string s) => JsonConvert.DeserializeObject<DepthIdsPair>(s));
                                if ((result.Eupm == null || !result.Eupm.Any()))
                                    result.Eupm = combinedParams.GetQueryArrayOrDefault(typeFromHandle.GetQueryKey("Eupm"));

                                break; // Only process the first avatar URL
                            }
                        }
                    }

                    result2 = true;
                }
            }

            return result2;
        }
    }
}