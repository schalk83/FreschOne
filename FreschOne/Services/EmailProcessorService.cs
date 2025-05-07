using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using FreschOne.DataAccess;
using FreschOne.Services;

namespace FreschOne.Services
{
    public class EmailProcessorService : BackgroundService
    {
        private readonly ILogger<EmailProcessorService> _logger;
        private readonly IConfiguration _configuration;

        public EmailProcessorService(ILogger<EmailProcessorService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ EmailProcessorService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                    conn.Open();

                    using var transaction = conn.BeginTransaction();

                    var emailService = new EmailService();

                    // You can loop over multiple template types here
                    emailService.ProcessPendingEmails(conn, transaction, "New");

                    transaction.Commit();
                    _logger.LogInformation("✅ Emails processed at {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error processing emails");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait before next run
            }
        }
    }
}
