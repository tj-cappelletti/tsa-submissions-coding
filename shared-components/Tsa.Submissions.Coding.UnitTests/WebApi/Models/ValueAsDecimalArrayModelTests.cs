using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ValueAsDecimalArrayModelTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TestSetInput_ValueAsJson_Is_Character_Array_Model_Should_Deserialize()
    {
        // Arrange
        var json = "{ \"value\": [ 1.1, 1.2, 1.3] }";

        var obj = JsonConvert.DeserializeObject<ValueAsDecimalArrayModel>(json);

        Assert.NotNull(obj);
        Assert.NotNull(obj.Value);
        Assert.Equal(3, obj.Value.Length);
        Assert.Equal(1.1m, obj.Value[0]);
        Assert.Equal(1.2m, obj.Value[1]);
        Assert.Equal(1.3m, obj.Value[2]);
    }
}
