using FluentEmail.Smtp;

namespace MansorySupplyHub.Extensions
{
    public static class FluentEmailExtensions
    {
        public static void AddFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {
            var senderEmail = configuration["SMTPConfig:EmailSenderAddress"];
            var senderName = configuration["SMTPConfig:EmailSenderName"];
            var senderPassword = configuration["SMTPConfig:EmailSenderPassword"];
            var smtpServerAddress = configuration["SMTPConfig:SMTPServerAddress"];
            var smtpServerPort = int.Parse(configuration["SMTPConfig:SMTPServerPort"]);
            var smtpServerEnableSSL = bool.Parse(configuration["SMTPConfig:SMTPServerEnableSSL"]);
            
            services.AddFluentEmail(senderName)
            .AddSmtpSender(smtpServerAddress, smtpServerPort, senderEmail, senderPassword);
        }
    }
}
