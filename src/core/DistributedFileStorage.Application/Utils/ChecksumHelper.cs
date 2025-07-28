using System.Security.Cryptography;

namespace DistributedFileStorage.Application.Utils;

/// <summary>
/// Provides utility methods to calculate SHA-256 checksums for file integrity verification.
/// </summary>
public static class ChecksumHelper
{
    /// <summary>
    /// Calculates the SHA-256 checksum of a file located at the specified file path.
    /// This is typically used to verify the integrity of a full file.
    /// </summary>
    /// <param name="filePath">The path to the file whose checksum will be calculated.</param>
    /// <returns>A hexadecimal string representing the SHA-256 checksum of the file.</returns>
    public static string CalculateFileChecksum(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// Calculates the SHA-256 checksum of a given byte array representing a chunk of a file.
    /// The resulting hash is used as the unique chunk identifier.
    /// </summary>
    /// <param name="chunkData">The byte array containing chunk data.</param>
    /// <returns>A hexadecimal string representing the SHA-256 checksum of the chunk.</returns>
    public static string CalculateChunkId(byte[] chunkData)
    {
        var hash = SHA256.HashData(chunkData);
        return Convert.ToHexString(hash);
    }
}
