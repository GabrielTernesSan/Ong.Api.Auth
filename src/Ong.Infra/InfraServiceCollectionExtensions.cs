using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ong.Domain.Queries;
using Ong.Domain.Repositories;
using Ong.Domain.Services;
using Ong.Infra.Queries;
using Ong.Infra.Repositories;
using Ong.Infra.Services;

namespace Ong.Infra
{
    public static class InfraServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OngDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOutboxMessageQuery, OutboxMessageQuery>();

            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
