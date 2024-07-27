namespace MansorySupplyHub.Extensions
{
    public static class FluentEmailExtensions
    {
        public static void AddFluentEmail(this IServiceCollection services,
       ConfigurationManager _configuration)
        {
            var senderEmail = _configuration["SMTPConfig:EmailSenderAddress"];
            var senderName = _configuration["SMTPConfig:EmailSenderName"];
            var senderPassword = _configuration["SMTPConfig:EmailSenderPassword"];
            var smtpServerAddress = _configuration["SMTPConfig:SMTPServerAddress"];
            var smtpServerPort = int.Parse(_configuration["SMTPConfig:SMTPServerPort"]);
            var smtpServerEnableSSL = bool.Parse(_configuration["SMTPConfig:SMTPServerEnableSSL"]);
            services.AddFluentEmail(senderEmail)
                .AddSmtpSender(senderEmail, smtpServerPort, senderName, senderPassword);
        }
    }
}
