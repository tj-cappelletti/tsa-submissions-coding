using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ValueAsCharacterArrayModelTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TestSetInput_ValueAsJson_Is_Character_Array_Model_Should_Deserialize()
    {
        // Arrange
        var json = "{ \"value\": [\"a\", \"b\", \"c\"] }";

        var obj = JsonConvert.DeserializeObject<ValueAsCharacterArrayModel>(json);

        Assert.NotNull(obj);
        Assert.NotNull(obj.Value);
        Assert.Equal(3, obj.Value.Length);
        Assert.Equal('a', obj.Value[0]);
        Assert.Equal('b', obj.Value[1]);
        Assert.Equal('c', obj.Value[2]);
    }
}
