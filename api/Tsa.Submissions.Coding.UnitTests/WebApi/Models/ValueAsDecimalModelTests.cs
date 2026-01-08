using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ValueAsDecimalModelTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TestSetInput_ValueAsJson_Is_Character_Array_Model_Should_Deserialize()
    {
        // Arrange
        var json = "{ \"value\": 1.1 }";

        var obj = JsonConvert.DeserializeObject<ValueAsDecimalModel>(json);

        Assert.NotNull(obj);
        Assert.NotNull(obj.Value);
        Assert.Equal(1.1m, obj.Value);
    }
}
