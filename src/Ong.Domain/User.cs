namespace Ong.Domain
{
    public class User
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Cpf { get; private set; }
        public string MaskedCpf => Cpf.Length == 11 ? $"***.***.***-{Cpf[9..]}" : "***.***.***-**";
        public string Role { get; private set; }

        public User(string name, string email, string passwordHash, string cpf, string role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(email));
            if (!IsValidEmail(email))
                throw new ArgumentException("Formato de email é inválido", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash não pode ser vazio ou nulo.", nameof(passwordHash));
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF não pode ser vazio ou nulo.", nameof(cpf));
            if (!CpfHelper.IsValid(cpf))
                throw new ArgumentException("CPF inválido.", nameof(cpf));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Cpf = new string(cpf.Where(char.IsDigit).ToArray()); // persiste só dígitos
            Role = role;
        }

        public User(Guid id, string name, string email, string passwordHash, string cpf, string role)
            : this(name, email, passwordHash, cpf, role)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio ou nulo.", nameof(id));

            Id = id;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash não pode ser vazio ou nulo.", nameof(passwordHash));

            PasswordHash = passwordHash;
        }

        public void SetRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Role = role;
        }

        public void UpdateEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(newEmail));
            if (!IsValidEmail(newEmail))
                throw new ArgumentException("Formato de email é inválido (Parameter 'newEmail')", nameof(newEmail));

            Email = newEmail;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}