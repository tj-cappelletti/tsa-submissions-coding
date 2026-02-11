using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ValueAsStringArrayModelTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TestSetInput_ValueAsJson_Is_Character_Array_Model_Should_Deserialize()
    {
        // Arrange
        var json = "{ \"value\": [ \"1\", \"2\", \"3\"] }";

        var obj = JsonConvert.DeserializeObject<ValueAsStringArrayModel>(json);

        Assert.NotNull(obj);
        Assert.NotNull(obj.Value);
        Assert.Equal(3, obj.Value.Length);
        Assert.Equal("1", obj.Value[0]);
        Assert.Equal("2", obj.Value[1]);
        Assert.Equal("3", obj.Value[2]);
    }
}
