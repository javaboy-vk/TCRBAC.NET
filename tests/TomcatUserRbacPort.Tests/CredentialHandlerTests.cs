using com.mag.dapi.security.credentials;

namespace TomcatUserRbacPort.Tests;

public sealed class CredentialHandlerTests
{
    [Fact]
    public void PlainTextCredentialHandler_MatchesExactPassword()
    {
        var handler = new PlainTextCredentialHandler();

        Assert.True(handler.Matches("secret", handler.Mutate("secret")));
    }

    [Fact]
    public void PlainTextCredentialHandler_RejectsDifferentPassword()
    {
        var handler = new PlainTextCredentialHandler();

        Assert.False(handler.Matches("secret", "SECRET"));
    }

    [Fact]
    public void MessageDigestCredentialHandler_MutatesAndMatchesPassword()
    {
        var handler = new MessageDigestCredentialHandler("SHA256", 2);

        var stored = handler.Mutate("secret");

        Assert.StartsWith("SHA256:2:", stored);
        Assert.True(handler.Matches("secret", stored));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-digest")]
    [InlineData("SHA256:not-number:abcdef")]
    public void MessageDigestCredentialHandler_RejectsMalformedStoredCredentials(string storedCredential)
    {
        var handler = new MessageDigestCredentialHandler();

        Assert.False(handler.Matches("secret", storedCredential));
    }

    [Fact]
    public void MessageDigestCredentialHandler_ThrowsForUnsupportedAlgorithmDuringMutation()
    {
        var handler = new MessageDigestCredentialHandler("NOT-A-HASH");

        Assert.Throws<NotSupportedException>(() => handler.Mutate("secret"));
    }

    [Fact]
    public void NestedCredentialHandler_MatchesUsingAnyConfiguredHandler()
    {
        var digest = new MessageDigestCredentialHandler();
        var nested = new NestedCredentialHandler(new ThrowingCredentialHandler(), new PlainTextCredentialHandler(), digest);

        Assert.True(nested.Matches("secret", "secret"));
        Assert.True(nested.Matches("secret", digest.Mutate("secret")));
    }

    [Fact]
    public void NestedCredentialHandler_ReturnsFalseWhenNoHandlerMatches()
    {
        var nested = new NestedCredentialHandler(new ThrowingCredentialHandler(), new PlainTextCredentialHandler());

        Assert.False(nested.Matches("secret", "wrong"));
    }

    [Fact]
    public void NestedCredentialHandler_MutateRequiresAtLeastOneHandler()
    {
        var nested = new NestedCredentialHandler();

        Assert.Throws<InvalidOperationException>(() => nested.Mutate("secret"));
    }

    private sealed class ThrowingCredentialHandler : ICredentialHandler
    {
        public bool Matches(string suppliedPassword, string storedCredential) => throw new FormatException("Bad format.");

        public string Mutate(string clearTextPassword) => throw new NotSupportedException();
    }
}
