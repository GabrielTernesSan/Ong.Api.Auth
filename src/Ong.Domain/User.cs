namespace Ong.Domain
{
    public class User
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Cpf { get; private set; }
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
            if (!IsValidCpf(cpf))
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

        private static bool IsValidCpf(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11) return false;
            if (cpf.Distinct().Count() == 1) return false;

            int[] mult1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] mult2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            var sum = mult1.Select((m, i) => m * (cpf[i] - '0')).Sum();
            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            sum = mult2.Select((m, i) => m * (cpf[i] - '0')).Sum();
            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cpf[9] - '0' == digit1 && cpf[10] - '0' == digit2;
        }
    }
}