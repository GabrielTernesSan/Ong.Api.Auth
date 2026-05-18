using Ong.Domain;
using Xunit;

namespace Ong.Tests;

public class UserTests
{
    [Fact]
    public void User_DeveSerCriadoComSucesso()
    {
        var user = new User("João da Silva", "joao@email.com", "hash123", "GestorONG");
        Assert.Equal("João da Silva", user.Name);
        Assert.Equal("joao@email.com", user.Email);
        Assert.Equal("GestorONG", user.Role);
    }

    [Fact]
    public void User_DeveFalharComEmailInvalido()
    {
        Assert.Throws<ArgumentException>(() => new User("João da Silva", "emailinvalido", "hash123", "GestorONG"));
    }

    [Fact]
    public void User_DeveFalharComEmailVazio()
    {
        Assert.Throws<ArgumentException>(() => new User("João da Silva", "", "hash123", "GestorONG"));
    }
}

public class OutboxMessageTests
{
    [Fact]
    public void OutboxMessage_DeveSerCriadoComSucesso()
    {
        var id = Guid.NewGuid();
        var message = new OutboxMessage(id, "TestEvent", "{}", DateTime.UtcNow);
        Assert.Equal(id, message.Id);
        Assert.Equal("TestEvent", message.Type);
    }
}
