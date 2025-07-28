using DistributedFileStorage.Application.Utils;
using System.Text;

namespace DistributedFileStorage.UnitTests.Application.Utils;

public class ChecksumHelperTests
{
    [Fact]
    public void CalculateChunkId_ShouldReturn_ValidSha256Checksum()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("test data");

        // Act
        var checksum = ChecksumHelper.CalculateChunkId(data);

        // Assert
        Assert.NotNull(checksum);
        Assert.Equal(64, checksum.Length); // SHA256 hash in hex: 64 chars
    }

    [Fact]
    public void CalculateFileChecksum_ShouldReturn_ExpectedChecksum_ForGivenFile()
    {
        // Arrange
        string tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, "unit test content");

        // Act
        var checksum = ChecksumHelper.CalculateFileChecksum(tempFilePath);

        // Assert
        Assert.NotNull(checksum);
        Assert.Equal(64, checksum.Length);

        // Clean up
        File.Delete(tempFilePath);
    }

    [Fact]
    public void Checksum_ShouldBeSame_ForSameContent()
    {
        // Arrange
        var data1 = Encoding.UTF8.GetBytes("same content");
        var data2 = Encoding.UTF8.GetBytes("same content");

        // Act
        var hash1 = ChecksumHelper.CalculateChunkId(data1);
        var hash2 = ChecksumHelper.CalculateChunkId(data2);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Checksum_ShouldBeDifferent_ForDifferentContent()
    {
        // Arrange
        var data1 = Encoding.UTF8.GetBytes("content one");
        var data2 = Encoding.UTF8.GetBytes("content two");

        // Act
        var hash1 = ChecksumHelper.CalculateChunkId(data1);
        var hash2 = ChecksumHelper.CalculateChunkId(data2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
}
