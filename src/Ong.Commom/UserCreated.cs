using MediatR;

namespace Ong.Commom
{
    public class UserCreated : INotification
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Timestamp { get; set; }

        public UserCreated(Guid id, string name, string email, DateTime timestamp)
        {
            Id = id;
            Name = name;
            Email = email;
            Timestamp = timestamp;
        }
    }
}
