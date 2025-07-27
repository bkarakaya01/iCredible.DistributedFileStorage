using System.Security.Cryptography;

namespace DistributedFileStorage.Application.Utils;

public static class ChecksumHelper
{
    public static string CalculateFileChecksum(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

    public static string CalculateChunkId(byte[] chunkData)
    {
        var hash = SHA256.HashData(chunkData);
        return Convert.ToHexString(hash);
    }
}
