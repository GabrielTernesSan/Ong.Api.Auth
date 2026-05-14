using MediatR;

namespace Ong.Commom
{
    public class UserCreated : INotification
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime Timestamp { get; set; }

        public UserCreated(Guid id, string name, string email, string passwordHash, string role, DateTime timestamp)
        {
            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            Timestamp = timestamp;
        }
    }
}
