using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ei8.Cortex.Diary.Plugins.Tree;
using ei8.Cortex.Graph.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tree.Test
{
    [TestClass]
    public class TreeQueryTests
    {
        #region AC1 Tests - AvatarUrl Parsing

        [TestMethod]
        public void TryParse_WithEncodedAvatarUrl_ShouldDecodeAndStoreAvatarUrl()
        {
            // Arrange
            string queryString = "avatarurl=http%3A%2F%2F192.168.1.31.nip.io%3A65111%2Fcortex%2Fneurons%3FPostsynaptic%3D8e050fbf-1f34-443e-9f2f-241024ce57e6%26relative%3D1%26pagesize%3D30%26sortorder%3D1%26sortby%3D1";
            string expectedDecodedUrl = "http://192.168.1.31.nip.io:65111/cortex/neurons?Postsynaptic=8e050fbf-1f34-443e-9f2f-241024ce57e6&relative=1&pagesize=30&sortorder=1&sortby=1";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual(expectedDecodedUrl, treeQuery.AvatarUrl);
        }

        [TestMethod]
        public void TryParse_WithFullUrlIncludingAvatarUrl_ShouldParseCorrectly()
        {
            // Arrange
            string fullUrl = "https://192.168.1.31.nip.io:65113/tree?avatarurl=http%3A%2F%2F192.168.1.31.nip.io%3A65111%2Fcortex%2Fneurons%3FPostsynaptic%3D8e050fbf-1f34-443e-9f2f-241024ce57e6%26relative%3D1%26pagesize%3D30%26sortorder%3D1%26sortby%3D1";
            string queryString = fullUrl.Substring(fullUrl.IndexOf('?'));
            string expectedDecodedUrl = "http://192.168.1.31.nip.io:65111/cortex/neurons?Postsynaptic=8e050fbf-1f34-443e-9f2f-241024ce57e6&relative=1&pagesize=30&sortorder=1&sortby=1";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual(expectedDecodedUrl, treeQuery.AvatarUrl);
        }

        [TestMethod]
        public void TryParse_WithInvalidAvatarUrl_ShouldReturnTrue()
        {
            // Arrange
            string queryString = "avatarurl=invalid%url";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result); // Should still parse, just with the decoded invalid URL
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual("invalid%url", treeQuery.AvatarUrl);
        }

        [TestMethod]
        public void TryParse_WithEmptyAvatarUrl_ShouldHandleGracefully()
        {
            // Arrange
            string queryString = "avatarurl=";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.AreEqual("", treeQuery.AvatarUrl);
        }

        #endregion

        #region AC2 Tests - AvatarUrl ToString

        [TestMethod]
        public void ToString_WithDecodedAvatarUrl_ShouldEncodeAvatarUrl()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            string decodedUrl = "http://192.168.1.31.nip.io:65111/cortex/neurons?Postsynaptic=8e050fbf-1f34-443e-9f2f-241024ce57e6&relative=1&pagesize=30&sortorder=1&sortby=1";
            treeQuery.AvatarUrl = decodedUrl;

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("avatarurl="));
            Assert.IsTrue(result.Contains(HttpUtility.UrlEncode(decodedUrl)));
        }

        [TestMethod]
        public void ToString_WithNullAvatarUrl_ShouldNotIncludeAvatarUrl()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            treeQuery.AvatarUrl = null;

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsFalse(result.Contains("avatarurl="));
        }

        #endregion

        #region AC3 Tests - Direction Parameter

        [TestMethod]
        public void TryParse_WithDirectionParameter_ShouldParseCorrectly()
        {
            // Arrange
            string queryString = "direction=1";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.DirectionValues);
            Assert.AreEqual((DirectionValues)1, treeQuery.DirectionValues.Value);
        }

        [TestMethod]
        public void ToString_WithDirectionValues_ShouldIncludeDirection()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            treeQuery.DirectionValues = (DirectionValues)1;

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("direction=1"));
        }

        [TestMethod]
        public void TryParse_WithInvalidDirection_ShouldHandleGracefully()
        {
            // Arrange
            string queryString = "direction=invalid";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsFalse(result); // Should return false for invalid enum value
            Assert.IsNull(treeQuery);
        }

        #endregion

        #region AC4 Tests - RegionId Parameter

        [TestMethod]
        public void TryParse_WithValidRegionId_ShouldParseRegionId()
        {
            // Arrange
            Guid expectedGuid = Guid.NewGuid();
            string queryString = $"regionid={expectedGuid}";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsTrue(treeQuery.RegionId.HasValue);
            Assert.AreEqual(expectedGuid, treeQuery.RegionId.Value);
        }

        [TestMethod]
        public void ToString_WithRegionId_ShouldIncludeRegionId()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            Guid regionId = Guid.NewGuid();
            treeQuery.RegionId = regionId;

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("RegionId="));
            Assert.IsTrue(result.Contains(regionId.ToString()));
        }

        [TestMethod]
        public void TryParse_WithInvalidRegionId_ShouldReturnNullRegionId()
        {
            // Arrange
            string queryString = "regionid=invalid-guid";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsFalse(treeQuery.RegionId.HasValue);
        }

        [TestMethod]
        public void ToString_WithNullRegionId_ShouldNotIncludeRegionId()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            treeQuery.RegionId = null;

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsFalse(result.Contains("RegionId="));
        }

        #endregion

        #region AC5 Tests - Postsynaptics Parameter

        [TestMethod]
        public void TryParse_WithValidPostsynaptics_ShouldParsePostsynaptics()
        {
            // Arrange
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            string queryString = $"postsynaptics={guid1}&postsynaptics={guid2}";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.Postsynaptics);
            Assert.AreEqual(2, treeQuery.Postsynaptics.Count());
            Assert.IsTrue(treeQuery.Postsynaptics.Contains(guid1));
            Assert.IsTrue(treeQuery.Postsynaptics.Contains(guid2));
        }

        [TestMethod]
        public void ToString_WithPostsynaptics_ShouldIncludePostsynaptics()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            treeQuery.Postsynaptics = new[] { guid1, guid2 };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Postsynaptics="));
            Assert.IsTrue(result.Contains(guid1.ToString()));
            Assert.IsTrue(result.Contains(guid2.ToString()));
        }

        [TestMethod]
        public void TryParse_WithInvalidPostsynaptics_ShouldIgnoreInvalidGuids()
        {
            // Arrange
            Guid validGuid = Guid.NewGuid();
            string queryString = $"postsynaptics={validGuid}&postsynaptics=invalid-guid";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.Postsynaptics);
            Assert.AreEqual(1, treeQuery.Postsynaptics.Count());
            Assert.IsTrue(treeQuery.Postsynaptics.Contains(validGuid));
        }

        #endregion

        #region AC6 Tests - ExpandUntilPostsynapticMirrors Parameter

        [TestMethod]
        public void TryParse_WithValidExpandUntilPostsynapticMirrors_ShouldParseCorrectly()
        {
            // Arrange
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            string queryString = $"eupm={guid1}&eupm={guid2}";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.ExpandUntilPostsynapticMirrors);
            Assert.AreEqual(2, treeQuery.ExpandUntilPostsynapticMirrors.Count());
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains(guid1));
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains(guid2));
        }

        [TestMethod]
        public void ToString_WithExpandUntilPostsynapticMirrors_ShouldIncludeEupm()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            treeQuery.ExpandUntilPostsynapticMirrors = new[] { guid1, guid2 };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("eupm="));
            Assert.IsTrue(result.Contains(guid1.ToString()));
            Assert.IsTrue(result.Contains(guid2.ToString()));
        }

        [TestMethod]
        public void TryParse_WithInvalidExpandUntilPostsynapticMirrors_ShouldIgnoreInvalidGuids()
        {
            // Arrange
            Guid validGuid = Guid.NewGuid();
            string queryString = $"eupm={validGuid}&eupm=invalid-guid";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.ExpandUntilPostsynapticMirrors);
            Assert.AreEqual(1, treeQuery.ExpandUntilPostsynapticMirrors.Count());
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains(validGuid));
        }

        #endregion

        #region AC7 Additional Test Cases

        [TestMethod]
        public void TryParse_WithNullOrEmptyString_ShouldReturnFalse()
        {
            // Arrange & Act & Assert
            Assert.IsFalse(TreeQuery.TreeQueryTryParse(null, out TreeQuery result1));
            Assert.IsNull(result1);

            Assert.IsFalse(TreeQuery.TreeQueryTryParse("", out TreeQuery result2));
            Assert.IsNull(result2);

            Assert.IsFalse(TreeQuery.TreeQueryTryParse("   ", out TreeQuery result3));
            Assert.IsNull(result3);
        }

        [TestMethod]
        public void TryParse_WithMalformedQueryString_ShouldHandleGracefully()
        {
            // Arrange
            string malformedQuery = "avatarurl=test&=value&noequals&another=";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(malformedQuery, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual("test", treeQuery.AvatarUrl);
        }

        [TestMethod]
        public void TryParse_WithOnlyQuestionMark_ShouldReturnFalse()
        {
            // Arrange
            string queryString = "?";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(treeQuery);
        }

        [TestMethod]
        public void ToString_WithEmptyTreeQuery_ShouldReturnEmptyString()
        {
            // Arrange
            var treeQuery = new TreeQuery();

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void RoundTrip_ParseAndToString_ShouldMaintainConsistency()
        {
            // Arrange
            Guid regionGuid = Guid.NewGuid();
            Guid postsynapticGuid = Guid.NewGuid();
            Guid eupmGuid = Guid.NewGuid();
            string originalQuery = $"avatarurl=http%3A%2F%2Ftest.com%2Fpath%3Fparam%3Dvalue&direction=1&regionid={regionGuid}&postsynaptics={postsynapticGuid}&eupm={eupmGuid}";

            // Act
            bool parseResult = TreeQuery.TreeQueryTryParse(originalQuery, out TreeQuery treeQuery);
            string reconstructedQuery = treeQuery.ToString();

            // Assert
            Assert.IsTrue(parseResult);
            Assert.IsNotNull(treeQuery);

            // Parse the reconstructed query to verify it produces the same object
            bool reparseResult = TreeQuery.TreeQueryTryParse(reconstructedQuery, out TreeQuery reparsedQuery);
            Assert.IsTrue(reparseResult);

            // Verify key properties are maintained
            Assert.AreEqual("http://test.com/path?param=value", treeQuery.AvatarUrl);
            Assert.AreEqual("http://test.com/path?param=value", reparsedQuery.AvatarUrl);
            Assert.AreEqual(treeQuery.DirectionValues, reparsedQuery.DirectionValues);
            Assert.AreEqual(treeQuery.RegionId, reparsedQuery.RegionId);
        }

        #endregion

        #region Complex Integration Tests

        [TestMethod]
        public void TryParse_WithComplexQueryString_ShouldParseAllParameters()
        {
            // Arrange
            Guid regionGuid = Guid.NewGuid();
            Guid postsynapticGuid1 = Guid.NewGuid();
            Guid postsynapticGuid2 = Guid.NewGuid();
            Guid eupmGuid1 = Guid.NewGuid();
            Guid eupmGuid2 = Guid.NewGuid();

            string complexQuery = $"avatarurl=http%3A%2F%2F192.168.1.31.nip.io%3A65111%2Fcortex%2Fneurons%3FPostsynaptic%3D8e050fbf-1f34-443e-9f2f-241024ce57e6" +
                                $"&direction=1&regionid={regionGuid}&postsynaptics={postsynapticGuid1}&postsynaptics={postsynapticGuid2}" +
                                $"&eupm={eupmGuid1}&eupm={eupmGuid2}";

            // Act
            bool result = TreeQuery.TreeQueryTryParse(complexQuery, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);

            // Verify all parameters
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual("http://192.168.1.31.nip.io:65111/cortex/neurons?Postsynaptic=8e050fbf-1f34-443e-9f2f-241024ce57e6", treeQuery.AvatarUrl);

            Assert.AreEqual((DirectionValues)1, treeQuery.DirectionValues);

            Assert.AreEqual(regionGuid, treeQuery.RegionId);

            Assert.IsNotNull(treeQuery.Postsynaptics);
            Assert.AreEqual(2, treeQuery.Postsynaptics.Count());
            Assert.IsTrue(treeQuery.Postsynaptics.Contains(postsynapticGuid1));
            Assert.IsTrue(treeQuery.Postsynaptics.Contains(postsynapticGuid2));

            Assert.IsNotNull(treeQuery.ExpandUntilPostsynapticMirrors);
            Assert.AreEqual(2, treeQuery.ExpandUntilPostsynapticMirrors.Count());
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains(eupmGuid1));
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains(eupmGuid2));
        }

        [TestMethod]
        public void ToString_WithComplexTreeQuery_ShouldGenerateCompleteQueryString()
        {
            // Arrange
            var treeQuery = new TreeQuery
            {
                AvatarUrl = "http://test.com/path?param=value",
                DirectionValues = (DirectionValues)1,
                RegionId = Guid.NewGuid(),
                Postsynaptics = new[] { Guid.NewGuid(), Guid.NewGuid() },
                ExpandUntilPostsynapticMirrors = new[] { Guid.NewGuid(), Guid.NewGuid() }
            };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.StartsWith("?"));
            Assert.IsTrue(result.Contains("avatarurl="));
            Assert.IsTrue(result.Contains("direction=1"));
            Assert.IsTrue(result.Contains("RegionId="));
            Assert.IsTrue(result.Contains("Postsynaptics="));
            Assert.IsTrue(result.Contains("eupm="));
        }

        #endregion
    }
}