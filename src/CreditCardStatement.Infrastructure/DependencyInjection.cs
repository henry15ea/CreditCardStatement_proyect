using CreditCardStatement.Application.Interfaces;
using CreditCardStatement.Domain.Interfaces;
using CreditCardStatement.Infrastructure.Data;
using CreditCardStatement.Infrastructure.Repositories;
using CreditCardStatement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditCardStatement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CreditCardDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ICardHolderRepository, CardHolderRepository>();
            services.AddScoped<IStoredProcedureService, StoredProcedureService>();
            services.AddScoped<IStatementPdfService, StatementPdfService>();

            return services;
        }
    }
}
