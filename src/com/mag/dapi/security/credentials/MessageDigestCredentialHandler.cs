// ============================================================
// File Name:     MessageDigestCredentialHandler.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Provides SHA/MD-style credential mutation and verification.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using System.Security.Cryptography;
using System.Text;
using log4net;

namespace com.mag.dapi.security.credentials;

/// <summary>
/// Digest-based credential handler inspired by Tomcat's MessageDigestCredentialHandler.
/// Supported algorithms depend on .NET but common values include SHA256, SHA384, and SHA512.
/// Stored format produced here: {ALGORITHM}:{ITERATIONS}:{HEX_DIGEST}
/// </summary>
public sealed class MessageDigestCredentialHandler : ICredentialHandler
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(MessageDigestCredentialHandler));

    public MessageDigestCredentialHandler(string algorithm = "SHA256", int iterations = 1)
    {
        Algorithm = algorithm;
        Iterations = Math.Max(1, iterations);
        Log.Info($"Created digest credential handler with algorithm '{Algorithm}' and {Iterations} iteration(s).");
    }

    public string Algorithm { get; }

    public int Iterations { get; }

    public bool Matches(string suppliedPassword, string storedCredential)
    {
        if (string.IsNullOrWhiteSpace(storedCredential))
        {
            Log.Info("Digest credential comparison failed because the stored credential was empty.");
            return false;
        }

        var parts = storedCredential.Split(':');
        if (parts.Length != 3)
        {
            Log.Info("Digest credential comparison failed because the stored credential format was invalid.");
            return false;
        }

        var algorithm = parts[0];
        if (!int.TryParse(parts[1], out var iterations))
        {
            Log.Info("Digest credential comparison failed because the iteration count was invalid.");
            return false;
        }

        var expected = parts[2];
        var actual = DigestHex(suppliedPassword ?? string.Empty, algorithm, Math.Max(1, iterations));

        var matches = FixedTimeEqualsHex(expected, actual);
        Log.Info(matches
            ? $"Digest credential comparison succeeded using algorithm '{algorithm}'."
            : $"Digest credential comparison failed using algorithm '{algorithm}'.");
        return matches;
    }

    public string Mutate(string clearTextPassword)
    {
        var digest = DigestHex(clearTextPassword ?? string.Empty, Algorithm, Iterations);
        Log.Info($"Created digest credential using algorithm '{Algorithm}' and {Iterations} iteration(s).");
        return $"{Algorithm}:{Iterations}:{digest}";
    }

    private static string DigestHex(string input, string algorithm, int iterations)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(input);

        for (int i = 0; i < iterations; i++)
        {
            buffer = HashData(algorithm, buffer);
        }

        return ToHexString(buffer);
    }

    private static byte[] HashData(string algorithm, byte[] data)
    {
        var normalizedAlgorithm = algorithm.Replace("-", string.Empty).ToUpperInvariant();
        using var hashAlgorithm = HashAlgorithm.Create(normalizedAlgorithm);
        if (hashAlgorithm == null)
        {
            throw new NotSupportedException($"Unsupported digest algorithm: {algorithm}");
        }

        return hashAlgorithm.ComputeHash(data);
    }

    private static bool FixedTimeEqualsHex(string expectedHex, string actualHex)
    {
        var expectedBytes = Encoding.ASCII.GetBytes(expectedHex ?? string.Empty);
        var actualBytes = Encoding.ASCII.GetBytes(actualHex ?? string.Empty);
        return FixedTimeEquals(expectedBytes, actualBytes);
    }

    private static string ToHexString(byte[] data)
    {
        var builder = new StringBuilder(data.Length * 2);
        foreach (var b in data)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    private static bool FixedTimeEquals(byte[] left, byte[] right)
    {
        var diff = left.Length ^ right.Length;
        var length = Math.Min(left.Length, right.Length);

        for (var i = 0; i < length; i++)
        {
            diff |= left[i] ^ right[i];
        }

        return diff == 0;
    }
}
