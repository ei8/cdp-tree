using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ei8.Cortex.Diary.Plugins.Tree;
using ei8.Cortex.Graph.Common;

namespace cdp.test
{
    public class TreeQueryTests
    {
        [Fact]
        public void TreeQuery_DefaultConstructor_ShouldCreateEmptyQuery()
        {
            // Act
            var query = new TreeQuery();

            // Assert
            Assert.NotNull(query);
            Assert.Null(query.Postsynaptic);
            Assert.Null(query.Presynaptic);
            Assert.Null(query.Tag);
            Assert.Null(query.Id);
            Assert.Null(query.RegionId);
        }

        [Fact]
        public void TreeQuery_ToString_WithEmptyQuery_ShouldReturnEmptyString()
        {
            // Arrange
            var query = new TreeQuery();

            // Act
            string result = query.ToString();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void TreeQuery_ToString_WithSingleParameter_ShouldReturnCorrectQueryString()
        {
            // Arrange
            var query = new TreeQuery
            {
                Id = new[] { "test-id" }
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Equal("?Id=test-id", result);
        }

        [Fact]
        public void TreeQuery_ToString_WithMultipleParameters_ShouldReturnCorrectQueryString()
        {
            // Arrange
            var query = new TreeQuery
            {
                Id = new[] { "id1", "id2" },
                Tag = new[] { "tag1" },
                PageSize = 10,
                Page = 1
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Contains("Id=id1", result);
            Assert.Contains("Id=id2", result);
            Assert.Contains("Tag=tag1", result);
            Assert.Contains("pagesize=10", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void TreeQuery_ToString_WithEnumValues_ShouldReturnCorrectQueryString()
        {
            // Arrange
            var query = new TreeQuery
            {
                SortBy = (SortByValue)0,
                SortOrder = (SortOrderValue)0,
                NeuronActiveValues = (ActiveValues)0,
                TerminalActiveValues = (ActiveValues)1
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Contains("sortby=0", result);
            Assert.Contains("sortorder=0", result);
            Assert.Contains("nactive=0", result);
            Assert.Contains("tactive=1", result);
        }

        [Fact]
        public void TreeQuery_TryParse_WithValidQueryString_ShouldReturnTrue()
        {
            // Arrange
            string queryString = "?Id=test-id&Tag=test-tag&PageSize=10";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("test-id", parsedQuery.Id);
            Assert.Contains("test-tag", parsedQuery.Tag);
            Assert.Equal(10, parsedQuery.PageSize);
        }

        [Fact]
        public void TreeQuery_TryParse_WithEmptyString_ShouldReturnFalse()
        {
            // Arrange
            string queryString = "";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.False(result);
            Assert.Null(parsedQuery);
        }

        [Fact]
        public void TreeQuery_TryParse_WithWhitespaceString_ShouldReturnFalse()
        {
            // Arrange
            string queryString = "   ";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.False(result);
            Assert.Null(parsedQuery);
        }

        [Fact]
        public void TreeQuery_TryParse_WithNullString_ShouldReturnFalse()
        {
            // Arrange
            string queryString = null;

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.False(result);
            Assert.Null(parsedQuery);
        }

        [Fact]
        public void TreeQuery_TryParse_WithUrlEncodedValues_ShouldDecodeCorrectly()
        {
            // Arrange
            string queryString = "?Id=test%20id&Tag=test%2Ftag";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("test id", parsedQuery.Id);
            Assert.Contains("test/tag", parsedQuery.Tag);
        }

        [Fact]
        public void TreeQuery_TryParse_WithMultipleValues_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "?Id=id1&Id=id2&Tag=tag1&Tag=tag2";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Equal(2, parsedQuery.Id.Count());
            Assert.Contains("id1", parsedQuery.Id);
            Assert.Contains("id2", parsedQuery.Id);
            Assert.Equal(2, parsedQuery.Tag.Count());
            Assert.Contains("tag1", parsedQuery.Tag);
            Assert.Contains("tag2", parsedQuery.Tag);
        }

        [Fact]
        public void TreeQuery_TryParse_WithEnumValues_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "?sortby=1&sortorder=1&nactive=0&tactive=1&relative=0&direction=1";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Equal((SortByValue)1, parsedQuery.SortBy);
            Assert.Equal((SortOrderValue)1, parsedQuery.SortOrder);
            Assert.Equal((ActiveValues)0, parsedQuery.NeuronActiveValues);
            Assert.Equal((ActiveValues)1, parsedQuery.TerminalActiveValues);
            Assert.Equal((RelativeValues)0, parsedQuery.RelativeValues);
            Assert.Equal((DirectionValues)1, parsedQuery.DirectionValues);
        }

        [Fact]
        public void TreeQuery_TryParse_WithNumericValues_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "?PageSize=25&Page=3&Depth=5";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Equal(25, parsedQuery.PageSize);
            Assert.Equal(3, parsedQuery.Page);
            Assert.Equal(5, parsedQuery.Depth);
        }

        [Fact]
        public void TreeQuery_TryParse_WithBooleanValue_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "?tagcontainsiw=true";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.True(parsedQuery.TagContainsIgnoreWhitespace);
        }

        [Fact]
        public void TreeQuery_TryParse_WithAvatarUrl_ShouldExtractQueryParameters()
        {
            // Arrange
            string queryString = "?avatarurl=https://example.com/avatar?sortby=1&page=2";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("https://example.com/avatar?sortby=1", parsedQuery.AvatarUrl);
            Assert.Equal((SortByValue)1, parsedQuery.SortBy);
            Assert.Equal(2, parsedQuery.Page);
        }

        [Fact]
        public void TreeQuery_TryParse_WithComplexAvatarUrl_ShouldExtractAllParameters()
        {
            // Arrange
            string queryString = "?Id=main-id&avatarurl=https://example.com/avatar?Id=avatar-id&Tag=avatar-tag&PageSize=15";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            // Main query parameters should take precedence
            Assert.Contains("main-id", parsedQuery.Id);
            // Avatar URL parameters should fill in missing values
            Assert.Contains("avatar-tag", parsedQuery.Tag);
            Assert.Equal(15, parsedQuery.PageSize);
        }

        [Fact]
        public void TreeQuery_TryParse_WithInvalidQueryString_ShouldHandleGracefully()
        {
            // Arrange
            string queryString = "?invalid=value&Id=valid-id&=";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("valid-id", parsedQuery.Id);
        }

        [Fact]
        public void TreeQuery_TryParse_WithQueryStringStartingWithQuestionMark_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "?Id=test-id";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("test-id", parsedQuery.Id);
        }

        [Fact]
        public void TreeQuery_TryParse_WithQueryStringWithoutQuestionMark_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "Id=test-id&Tag=test-tag";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("test-id", parsedQuery.Id);
            Assert.Contains("test-tag", parsedQuery.Tag);
        }

        [Fact]
        public void TreeQuery_ToString_WithAllProperties_ShouldIncludeAllParameters()
        {
            // Arrange
            var query = new TreeQuery
            {
                Id = new[] { "id1", "id2" },
                IdNot = new[] { "id3" },
                Tag = new[] { "tag1" },
                TagNot = new[] { "tag2" },
                TagContains = new[] { "contains" },
                TagContainsNot = new[] { "notcontains" },
                TagContainsIgnoreWhitespace = true,
                Postsynaptic = new[] { "post1" },
                PostsynapticNot = new[] { "post2" },
                Presynaptic = new[] { "pre1" },
                PresynapticNot = new[] { "pre2" },
                RegionId = new[] { "region1" },
                RegionIdNot = new[] { "region2" },
                ExternalReferenceUrl = new[] { "url1" },
                ExternalReferenceUrlContains = new[] { "urlcontains" },
                PostsynapticExternalReferenceUrl = new[] { "posturl" },
                AvatarUrl = new[] { "avatar1" },
                RelativeValues = (RelativeValues)1,
                PageSize = 20,
                Page = 2,
                NeuronActiveValues = (ActiveValues)0,
                TerminalActiveValues = (ActiveValues)1,
                SortBy = (SortByValue)1,
                SortOrder = (SortOrderValue)1,
                Depth = 3,
                DirectionValues = (DirectionValues)1,
                TraversalDepthPostsynaptic = new[] { new DepthIdsPair { Depth = 1, Ids = new[] { Guid.NewGuid() } } }
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Contains("Id=id1", result);
            Assert.Contains("Id=id2", result);
            Assert.Contains("IdNot=id3", result);
            Assert.Contains("Tag=tag1", result);
            Assert.Contains("TagNot=tag2", result);
            Assert.Contains("TagContains=contains", result);
            Assert.Contains("TagContainsNot=notcontains", result);
            Assert.Contains("tagcontainsiw=True", result);
            Assert.Contains("Postsynaptic=post1", result);
            Assert.Contains("PostsynapticNot=post2", result);
            Assert.Contains("Presynaptic=pre1", result);
            Assert.Contains("PresynapticNot=pre2", result);
            Assert.Contains("RegionId=region1", result);
            Assert.Contains("RegionIdNot=region2", result);
            Assert.Contains("erurl=url1", result);
            Assert.Contains("erurlcontains=urlcontains", result);
            Assert.Contains("posterurl=posturl", result);
            Assert.Contains("avatarurl=avatar1", result);
            Assert.Contains("relative=1", result);
            Assert.Contains("pagesize=20", result);
            Assert.Contains("page=2", result);
            Assert.Contains("nactive=0", result);
            Assert.Contains("tactive=1", result);
            Assert.Contains("sortby=1", result);
            Assert.Contains("sortorder=1", result);
            Assert.Contains("Depth=3", result);
            Assert.Contains("direction=1", result);
        }

        [Fact]
        public void TreeQuery_TryParse_WithJsonSerializedObject_ShouldParseCorrectly()
        {
            // Arrange
            var depthIdsPair = new DepthIdsPair { Depth = 2, Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } };
            string json = System.Text.Json.JsonSerializer.Serialize(depthIdsPair);
            string queryString = $"?trdeppost={Uri.EscapeDataString(json)}";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.NotNull(parsedQuery.TraversalDepthPostsynaptic);
            Assert.Single(parsedQuery.TraversalDepthPostsynaptic);
            var parsed = parsedQuery.TraversalDepthPostsynaptic.First();
            Assert.Equal(2, parsed.Depth);
            Assert.Equal(2, parsed.Ids.Count());
        }

        [Fact]
        public void TreeQuery_TryParse_WithUrlEncodedAvatarUrlContainingQueryParams_ShouldExtractParameters()
        {
            // Arrange
            string url = "?avatarurl=http%3A%2F%2F172.20.10.3%3A65101%2Fcortex%2Fneurons%3Fsortby%3D1%26sortorder%3D1";

            // Act
            bool result = TreeQuery.TryParse(url, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("http://172.20.10.3:65101/cortex/neurons?sortby=1&sortorder=1", parsedQuery.AvatarUrl);
            Assert.Equal((SortByValue)1, parsedQuery.SortBy);
            Assert.Equal((SortOrderValue)1, parsedQuery.SortOrder);
        }

        [Fact]
        public void TreeQuery_TryParse_WithEupmParameter_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "?eupm=value1&eupm=value2";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.NotNull(parsedQuery.Eupm);
            Assert.Equal(2, parsedQuery.Eupm.Count());
            Assert.Contains("value1", parsedQuery.Eupm);
            Assert.Contains("value2", parsedQuery.Eupm);
        }

        [Fact]
        public void TreeQuery_TryParse_WithSingleEupmParameter_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "?eupm=single-value";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.NotNull(parsedQuery.Eupm);
            Assert.Single(parsedQuery.Eupm);
            Assert.Contains("single-value", parsedQuery.Eupm);
        }

        [Fact]
        public void TreeQuery_TryParse_WithUrlEncodedEupmParameter_ShouldDecodeCorrectly()
        {
            // Arrange
            string queryString = "?eupm=encoded%20value%2Fwith%2Fslashes";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.NotNull(parsedQuery.Eupm);
            Assert.Single(parsedQuery.Eupm);
            Assert.Contains("encoded value/with/slashes", parsedQuery.Eupm);
        }

        [Fact]
        public void TreeQuery_ToString_WithEupmParameter_ShouldIncludeInQueryString()
        {
            // Arrange
            var query = new TreeQuery
            {
                Eupm = new[] { "eupm-value1", "eupm-value2" }
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Contains("eupm=eupm-value1", result);
            Assert.Contains("eupm=eupm-value2", result);
        }

        [Fact]
        public void TreeQuery_ToString_WithSingleEupmParameter_ShouldIncludeInQueryString()
        {
            // Arrange
            var query = new TreeQuery
            {
                Eupm = new[] { "single-eupm-value" }
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Equal("?eupm=single-eupm-value", result);
        }

        [Fact]
        public void TreeQuery_ToString_WithEupmAndOtherParameters_ShouldIncludeAllParameters()
        {
            // Arrange
            var query = new TreeQuery
            {
                Id = new[] { "test-id" },
                Eupm = new[] { "eupm-value" },
                PageSize = 10
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Contains("Id=test-id", result);
            Assert.Contains("eupm=eupm-value", result);
            Assert.Contains("pagesize=10", result);
        }

        [Fact]
        public void TreeQuery_TryParse_WithEupmInAvatarUrl_ShouldExtractFromAvatarUrl()
        {
            // Arrange
            string queryString = "?avatarurl=https://example.com/avatar?eupm=avatar-eupm-value";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.Contains("https://example.com/avatar?eupm=avatar-eupm-value", parsedQuery.AvatarUrl);
            Assert.NotNull(parsedQuery.Eupm);
            Assert.Single(parsedQuery.Eupm);
            Assert.Contains("avatar-eupm-value", parsedQuery.Eupm);
        }

        [Fact]
        public void TreeQuery_TryParse_WithEupmInMainQueryAndAvatarUrl_MainQueryShouldTakePrecedence()
        {
            // Arrange
            string queryString = "?eupm=main-eupm-value&avatarurl=https://example.com/avatar?eupm=avatar-eupm-value";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery parsedQuery);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedQuery);
            Assert.NotNull(parsedQuery.Eupm);
            Assert.Single(parsedQuery.Eupm);
            Assert.Contains("main-eupm-value", parsedQuery.Eupm);
            // Avatar URL eupm should not override main query eupm
            Assert.DoesNotContain("avatar-eupm-value", parsedQuery.Eupm);
        }

        [Fact]
        public void TreeQuery_ToString_WithAllPropertiesIncludingEupm_ShouldIncludeAllParameters()
        {
            // Arrange
            var query = new TreeQuery
            {
                Id = new[] { "id1", "id2" },
                IdNot = new[] { "id3" },
                Tag = new[] { "tag1" },
                TagNot = new[] { "tag2" },
                TagContains = new[] { "contains" },
                TagContainsNot = new[] { "notcontains" },
                TagContainsIgnoreWhitespace = true,
                Postsynaptic = new[] { "post1" },
                PostsynapticNot = new[] { "post2" },
                Presynaptic = new[] { "pre1" },
                PresynapticNot = new[] { "pre2" },
                RegionId = new[] { "region1" },
                RegionIdNot = new[] { "region2" },
                ExternalReferenceUrl = new[] { "url1" },
                ExternalReferenceUrlContains = new[] { "urlcontains" },
                PostsynapticExternalReferenceUrl = new[] { "posturl" },
                AvatarUrl = new[] { "avatar1" },
                Eupm = new[] { "eupm1", "eupm2" },
                RelativeValues = (RelativeValues)1,
                PageSize = 20,
                Page = 2,
                NeuronActiveValues = (ActiveValues)0,
                TerminalActiveValues = (ActiveValues)1,
                SortBy = (SortByValue)1,
                SortOrder = (SortOrderValue)1,
                Depth = 3,
                DirectionValues = (DirectionValues)1,
                TraversalDepthPostsynaptic = new[] { new DepthIdsPair { Depth = 1, Ids = new[] { Guid.NewGuid() } } }
            };

            // Act
            string result = query.ToString();

            // Assert
            Assert.Contains("Id=id1", result);
            Assert.Contains("Id=id2", result);
            Assert.Contains("IdNot=id3", result);
            Assert.Contains("Tag=tag1", result);
            Assert.Contains("TagNot=tag2", result);
            Assert.Contains("TagContains=contains", result);
            Assert.Contains("TagContainsNot=notcontains", result);
            Assert.Contains("tagcontainsiw=True", result);
            Assert.Contains("Postsynaptic=post1", result);
            Assert.Contains("PostsynapticNot=post2", result);
            Assert.Contains("Presynaptic=pre1", result);
            Assert.Contains("PresynapticNot=pre2", result);
            Assert.Contains("RegionId=region1", result);
            Assert.Contains("RegionIdNot=region2", result);
            Assert.Contains("erurl=url1", result);
            Assert.Contains("erurlcontains=urlcontains", result);
            Assert.Contains("posterurl=posturl", result);
            Assert.Contains("avatarurl=avatar1", result);
            Assert.Contains("eupm=eupm1", result);
            Assert.Contains("eupm=eupm2", result);
            Assert.Contains("relative=1", result);
            Assert.Contains("pagesize=20", result);
            Assert.Contains("page=2", result);
            Assert.Contains("nactive=0", result);
            Assert.Contains("tactive=1", result);
            Assert.Contains("sortby=1", result);
            Assert.Contains("sortorder=1", result);
            Assert.Contains("Depth=3", result);
            Assert.Contains("direction=1", result);
        }
    }
}