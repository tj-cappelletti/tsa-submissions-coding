using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ValueAsCharacterModelTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TestSetInput_ValueAsJson_Is_Character_Array_Model_Should_Deserialize()
    {
        // Arrange
        var json = "{\"value\": \"a\" }";

        var obj = JsonConvert.DeserializeObject<ValueAsCharacterModel>(json);

        Assert.NotNull(obj);
        Assert.Equal('a', obj.Value);
    }
}