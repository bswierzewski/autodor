using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;

namespace Autodor.Tests.Unit.Modules.Invoicing.Services.IFirma.Clients.Extensions;

public class HmacSha1Tests
{
    [Fact]
    public void Compute_WithHexadecimalKey_ReturnsCorrectHash()
    {
        // Arrange
        string keyHex = "111111";
        string message = "222222";
        string expectedHash = "cec153ee6350475f117a307111e2bd7d83034925";

        // Act
        string hash = HmacSha1.Compute(keyHex, message);

        // Assert
        hash.Should().Be(expectedHash);
    }

    [Fact]
    public void Compute_WithPolishCharacters_EncodesAsUtf8()
    {
        // Arrange
        string keyHex = "7365637265746b6579"; // "secretkey" in hex
        string message = "zażółwgęśćJakoMówićŚwiatłoDanych";

        // Act
        string hash = HmacSha1.Compute(keyHex, message);

        // Assert - Hash should be computed without errors and be a valid hex string
        hash.Should().NotBeNullOrEmpty();
        hash.Length.Should().Be(40); // SHA1 produces 20 bytes = 40 hex characters
        hash.Should().MatchRegex("[a-f0-9]{40}");
    }

    [Fact]
    public void Compute_WithDifferentKeys_ProducesDifferentHashes()
    {
        // Arrange
        string message = "testmessage";
        string key1Hex = "0102030405";
        string key2Hex = "0506070809";

        // Act
        string hash1 = HmacSha1.Compute(key1Hex, message);
        string hash2 = HmacSha1.Compute(key2Hex, message);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Compute_WithEmptyContent_StillProducesHash()
    {
        // Arrange
        string keyHex = "deadbeef";
        string message = ""; // Empty content

        // Act
        string hash = HmacSha1.Compute(keyHex, message);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Length.Should().Be(40);
    }

    [Fact]
    public void Compute_WithMultipleInvocations_ProducesConsistentResults()
    {
        // Arrange
        string keyHex = "abcdef0123456789";
        string message = "consistencytestmessage";

        // Act
        string hash1 = HmacSha1.Compute(keyHex, message);
        string hash2 = HmacSha1.Compute(keyHex, message);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Theory]
    [InlineData("6162636465", "message1", 40)] // abc hex key with message1
    [InlineData("deadbeef", "test", 40)]
    [InlineData("00000000", "empty", 40)]
    public void Compute_ProducesCorrectHashLength(string keyHex, string message, int expectedHashLength)
    {
        // Act
        string hash = HmacSha1.Compute(keyHex, message);

        // Assert
        hash.Length.Should().Be(expectedHashLength);
    }

    [Fact]
    public void Compute_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        string? keyHex = null;
        string message = "message";

        // Act & Assert
        var action = () => HmacSha1.Compute(keyHex!, message);
        action.Should().Throw<ArgumentNullException>().WithParameterName("keyHex");
    }

    [Fact]
    public void Compute_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string keyHex = "abc123";
        string? message = null;

        // Act & Assert
        var action = () => HmacSha1.Compute(keyHex, message!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("message");
    }

    [Fact]
    public void Compute_WithInvalidHexKey_ThrowsFormatException()
    {
        // Arrange
        string invalidKeyHex = "ZZZZZZ"; // Invalid hex
        string message = "message";

        // Act & Assert
        var action = () => HmacSha1.Compute(invalidKeyHex, message);
        action.Should().Throw<FormatException>();
    }
}
