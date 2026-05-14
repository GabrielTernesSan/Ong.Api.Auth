using Microsoft.EntityFrameworkCore;
using Ong.Domain;
using Ong.Domain.Repositories;

namespace Ong.Infra.Repositories
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly OngDbContext _context;

        public OutboxMessageRepository(OngDbContext context)
        {
            _context = context;
        }

        public async Task<OutboxMessage?> GetOutboxMessageByIdAsync(Guid id)
        {
            return await (from o in _context.OutboxMessages
                                 where o.Id == id
                                 select new OutboxMessage(
                                     o.Id,
                                     o.Type,
                                     o.Payload,
                                     o.CreatedOn,
                                     o.ProcessedOn,
                                     o.Error)).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            var entity = new Tables.OutboxMessage
            {
                Id = message.Id,
                Type = message.Type,
                Payload = message.Payload,
                CreatedOn = message.CreatedOn,
                ProcessedOn = message.ProcessedOn,
                Error = message.Error
            };

            await _context.OutboxMessages.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            var entity = await _context.OutboxMessages
                .FindAsync([message.Id], cancellationToken) ?? throw new Exception("Entity not found");

            entity.ProcessedOn = message.ProcessedOn;
            entity.Error = message.Error;
        }
    }
}
