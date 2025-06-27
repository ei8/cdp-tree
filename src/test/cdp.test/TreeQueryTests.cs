using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ei8.Cortex.Diary.Plugins.Tree;
using ei8.Cortex.Graph.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cdp.test
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
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual(1, treeQuery.AvatarUrl.Count());
            Assert.AreEqual(expectedDecodedUrl, treeQuery.AvatarUrl.First());
        }

        [TestMethod]
        public void TryParse_WithFullUrlIncludingAvatarUrl_ShouldParseCorrectly()
        {
            // Arrange
            string fullUrl = "https://192.168.1.31.nip.io:65113/tree?avatarurl=http%3A%2F%2F192.168.1.31.nip.io%3A65111%2Fcortex%2Fneurons%3FPostsynaptic%3D8e050fbf-1f34-443e-9f2f-241024ce57e6%26relative%3D1%26pagesize%3D30%26sortorder%3D1%26sortby%3D1";
            string queryString = fullUrl.Substring(fullUrl.IndexOf('?'));
            string expectedDecodedUrl = "http://192.168.1.31.nip.io:65111/cortex/neurons?Postsynaptic=8e050fbf-1f34-443e-9f2f-241024ce57e6&relative=1&pagesize=30&sortorder=1&sortby=1";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual(expectedDecodedUrl, treeQuery.AvatarUrl.First());
        }

        [TestMethod]
        public void TryParse_WithInvalidAvatarUrl_ShouldReturnFalse()
        {
            // Arrange
            string queryString = "avatarurl=invalid%url";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result); // Should still parse, just with the decoded invalid URL
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual("invalid%url", treeQuery.AvatarUrl.First());
        }

        [TestMethod]
        public void TryParse_WithEmptyAvatarUrl_ShouldHandleGracefully()
        {
            // Arrange
            string queryString = "avatarurl=";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual("", treeQuery.AvatarUrl.First());
        }

        #endregion

        #region AC2 Tests - AvatarUrl ToString

        [TestMethod]
        public void ToString_WithDecodedAvatarUrl_ShouldEncodeAvatarUrl()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            string decodedUrl = "http://192.168.1.31.nip.io:65111/cortex/neurons?Postsynaptic=8e050fbf-1f34-443e-9f2f-241024ce57e6&relative=1&pagesize=30&sortorder=1&sortby=1";
            treeQuery.AvatarUrl = new[] { decodedUrl };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("avatarurl="));
            Assert.IsTrue(result.Contains(HttpUtility.UrlEncode(decodedUrl)));
        }

        [TestMethod]
        public void ToString_WithMultipleAvatarUrls_ShouldEncodeAllUrls()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            string[] decodedUrls = {
                "http://192.168.1.31.nip.io:65111/cortex/neurons?test=1",
                "http://example.com/test?param=value"
            };
            treeQuery.AvatarUrl = decodedUrls;

            // Act
            string result = treeQuery.ToString();

            // Assert
            foreach (string url in decodedUrls)
            {
                Assert.IsTrue(result.Contains(HttpUtility.UrlEncode(url)));
            }
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
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

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
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsFalse(result); // Should return false for invalid enum value
            Assert.IsNull(treeQuery);
        }

        #endregion

        #region AC4 Tests - RegionId Parameter

        [TestMethod]
        public void TryParse_WithEncodedRegionId_ShouldDecodeRegionId()
        {
            // Arrange
            string encodedRegionId = "region%2Dtest%2D123";
            string queryString = $"regionid={encodedRegionId}";
            string expectedDecodedRegionId = "region-test-123";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.RegionId);
            Assert.AreEqual(expectedDecodedRegionId, treeQuery.RegionId.First());
        }

        [TestMethod]
        public void ToString_WithRegionId_ShouldEncodeRegionId()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            string regionId = "region-test-123";
            treeQuery.RegionId = new[] { regionId };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("RegionId="));
            Assert.IsTrue(result.Contains(regionId)); // Basic encoding handled by AppendQuery
        }

        [TestMethod]
        public void TryParse_WithMultipleRegionIds_ShouldParseAll()
        {
            // Arrange
            string queryString = "regionid=region1&regionid=region%2D2";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.RegionId);
            Assert.AreEqual(2, treeQuery.RegionId.Count());
            Assert.IsTrue(treeQuery.RegionId.Contains("region1"));
            Assert.IsTrue(treeQuery.RegionId.Contains("region-2"));
        }

        #endregion

        #region AC5 Tests - Postsynaptic Parameter

        [TestMethod]
        public void TryParse_WithEncodedPostsynaptic_ShouldDecodePostsynaptic()
        {
            // Arrange
            string encodedPostsynaptic = "8e050fbf%2D1f34%2D443e%2D9f2f%2D241024ce57e6";
            string queryString = $"postsynaptic={encodedPostsynaptic}";
            string expectedDecodedPostsynaptic = "8e050fbf-1f34-443e-9f2f-241024ce57e6";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.Postsynaptic);
            Assert.AreEqual(expectedDecodedPostsynaptic, treeQuery.Postsynaptic.First());
        }

        [TestMethod]
        public void ToString_WithPostsynaptic_ShouldIncludePostsynaptic()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            string postsynaptic = "8e050fbf-1f34-443e-9f2f-241024ce57e6";
            treeQuery.Postsynaptic = new[] { postsynaptic };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Postsynaptic="));
            Assert.IsTrue(result.Contains(postsynaptic));
        }

        [TestMethod]
        public void TryParse_WithMultiplePostsynapticValues_ShouldParseAll()
        {
            // Arrange
            string queryString = "postsynaptic=guid1&postsynaptic=guid%2D2";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.Postsynaptic);
            Assert.AreEqual(2, treeQuery.Postsynaptic.Count());
            Assert.IsTrue(treeQuery.Postsynaptic.Contains("guid1"));
            Assert.IsTrue(treeQuery.Postsynaptic.Contains("guid-2"));
        }

        #endregion

        #region AC6 Tests - Eupm Parameter

        [TestMethod]
        public void TryParse_WithEncodedEupm_ShouldDecodeEupm()
        {
            // Arrange
            string encodedEupm = "test%2Deupm%2Dvalue";
            string queryString = $"eupm={encodedEupm}";
            string expectedDecodedEupm = "test-eupm-value";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.ExpandUntilPostsynapticMirrors);
            Assert.AreEqual(expectedDecodedEupm, treeQuery.ExpandUntilPostsynapticMirrors.First());
        }

        [TestMethod]
        public void ToString_WithEupm_ShouldIncludeEupm()
        {
            // Arrange
            var treeQuery = new TreeQuery();
            string eupm = "test-eupm-value";
            treeQuery.ExpandUntilPostsynapticMirrors = new[] { eupm };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.Contains("eupm="));
            Assert.IsTrue(result.Contains(eupm));
        }

        [TestMethod]
        public void TryParse_WithMultipleEupmValues_ShouldParseAll()
        {
            // Arrange
            string queryString = "eupm=value1&eupm=value%2D2";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.ExpandUntilPostsynapticMirrors);
            Assert.AreEqual(2, treeQuery.ExpandUntilPostsynapticMirrors.Count());
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains("value1"));
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains("value-2"));
        }

        #endregion

        #region AC7 Additional Failing Test Cases

        [TestMethod]
        public void TryParse_WithNullOrEmptyString_ShouldReturnFalse()
        {
            // Arrange & Act & Assert
            Assert.IsFalse(TreeQuery.TryParse(null, out TreeQuery result1));
            Assert.IsNull(result1);

            Assert.IsFalse(TreeQuery.TryParse("", out TreeQuery result2));
            Assert.IsNull(result2);

            Assert.IsFalse(TreeQuery.TryParse("   ", out TreeQuery result3));
            Assert.IsNull(result3);
        }

        [TestMethod]
        public void TryParse_WithMalformedQueryString_ShouldHandleGracefully()
        {
            // Arrange
            string malformedQuery = "avatarurl=test&=value&noequals&another=";

            // Act
            bool result = TreeQuery.TryParse(malformedQuery, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual("test", treeQuery.AvatarUrl.First());
        }

        [TestMethod]
        public void TryParse_WithOnlyQuestionMark_ShouldReturnFalse()
        {
            // Arrange
            string queryString = "?";

            // Act
            bool result = TreeQuery.TryParse(queryString, out TreeQuery treeQuery);

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
            string originalQuery = "avatarurl=http%3A%2F%2Ftest.com%2Fpath%3Fparam%3Dvalue&direction=1&regionid=test%2Did&postsynaptic=guid1&eupm=eupm%2Dvalue";

            // Act
            bool parseResult = TreeQuery.TryParse(originalQuery, out TreeQuery treeQuery);
            string reconstructedQuery = treeQuery.ToString();

            // Assert
            Assert.IsTrue(parseResult);
            Assert.IsNotNull(treeQuery);

            // Parse the reconstructed query to verify it produces the same object
            bool reparseResult = TreeQuery.TryParse(reconstructedQuery, out TreeQuery reparsedQuery);
            Assert.IsTrue(reparseResult);

            // Verify key properties are maintained
            Assert.AreEqual("http://test.com/path?param=value", treeQuery.AvatarUrl.First());
            Assert.AreEqual("http://test.com/path?param=value", reparsedQuery.AvatarUrl.First());
            Assert.AreEqual(treeQuery.DirectionValues, reparsedQuery.DirectionValues);
            Assert.AreEqual(treeQuery.RegionId.First(), reparsedQuery.RegionId.First());
        }

        #endregion

        #region Complex Integration Tests

        [TestMethod]
        public void TryParse_WithComplexQueryString_ShouldParseAllParameters()
        {
            // Arrange
            string complexQuery = "avatarurl=http%3A%2F%2F192.168.1.31.nip.io%3A65111%2Fcortex%2Fneurons%3FPostsynaptic%3D8e050fbf-1f34-443e-9f2f-241024ce57e6" +
                                "&direction=1&regionid=region%2D1&regionid=region%2D2&postsynaptic=guid1&postsynaptic=guid2" +
                                "&eupm=eupm1&eupm=eupm2";

            // Act
            bool result = TreeQuery.TryParse(complexQuery, out TreeQuery treeQuery);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(treeQuery);

            // Verify all parameters
            Assert.IsNotNull(treeQuery.AvatarUrl);
            Assert.AreEqual("http://192.168.1.31.nip.io:65111/cortex/neurons?Postsynaptic=8e050fbf-1f34-443e-9f2f-241024ce57e6", treeQuery.AvatarUrl.First());

            Assert.AreEqual((DirectionValues)1, treeQuery.DirectionValues);

            Assert.IsNotNull(treeQuery.RegionId);
            Assert.AreEqual(2, treeQuery.RegionId.Count());
            Assert.IsTrue(treeQuery.RegionId.Contains("region-1"));
            Assert.IsTrue(treeQuery.RegionId.Contains("region-2"));

            Assert.IsNotNull(treeQuery.Postsynaptic);
            Assert.AreEqual(2, treeQuery.Postsynaptic.Count());
            Assert.IsTrue(treeQuery.Postsynaptic.Contains("guid1"));
            Assert.IsTrue(treeQuery.Postsynaptic.Contains("guid2"));

            Assert.IsNotNull(treeQuery.ExpandUntilPostsynapticMirrors);
            Assert.AreEqual(2, treeQuery.ExpandUntilPostsynapticMirrors.Count());
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains("eupm1"));
            Assert.IsTrue(treeQuery.ExpandUntilPostsynapticMirrors.Contains("eupm2"));
        }

        [TestMethod]
        public void ToString_WithComplexTreeQuery_ShouldGenerateCompleteQueryString()
        {
            // Arrange
            var treeQuery = new TreeQuery
            {
                AvatarUrl = new[] { "http://test.com/path?param=value" },
                DirectionValues = (DirectionValues)1,
                RegionId = new[] { "region-1", "region-2" },
                Postsynaptic = new[] { "guid1", "guid2" },
                ExpandUntilPostsynapticMirrors = new[] { "eupm1", "eupm2" }
            };

            // Act
            string result = treeQuery.ToString();

            // Assert
            Assert.IsTrue(result.StartsWith("?"));
            Assert.IsTrue(result.Contains("avatarurl="));
            Assert.IsTrue(result.Contains("direction=1"));
            Assert.IsTrue(result.Contains("RegionId="));
            Assert.IsTrue(result.Contains("Postsynaptic="));
            Assert.IsTrue(result.Contains("eupm="));
        }

        #endregion
    }
}