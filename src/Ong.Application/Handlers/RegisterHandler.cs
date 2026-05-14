using MediatR;
using Ong.Application.Requests;
using Ong.Commom;
using Ong.Domain;
using Ong.Domain.Enums;
using Ong.Domain.Repositories;
using System.Text.Json;

namespace Ong.Application.Handlers
{
    public class RegisterHandler : IRequestHandler<RegisterRequest, Response>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOutboxMessageRepository _outboxRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterHandler(IUserRepository userRepository, IOutboxMessageRepository outboxRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _outboxRepository = outboxRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                response.AddError("Já existe um usuário com este email.");
                return response;
            }

            if (!Enum.TryParse<ERole>(request.Role, out _))
                return response.AddError($"Role inválida. Valores permitidos: {string.Join(", ", Enum.GetNames<ERole>())}.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User(Guid.NewGuid(), request.Name, request.Email, passwordHash, request.Role);

            var userCreatedEvent = new UserCreated(user.Id, user.Name, user.Email, user.PasswordHash, user.Role, DateTime.UtcNow);

            var outboxMessage = new OutboxMessage(
                Guid.NewGuid(),
                typeof(UserCreated).Name!,
                JsonSerializer.Serialize(userCreatedEvent),
                DateTime.UtcNow
            );

            await _outboxRepository.CreateAsync(outboxMessage, cancellationToken);

            await _userRepository.CreateAsync(user, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            response.SetResult(new { user.Id, user.Name, user.Email, user.Role });

            return response;
        }
    }
}
