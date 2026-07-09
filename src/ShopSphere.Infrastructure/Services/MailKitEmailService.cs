using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure.Settings;

namespace ShopSphere.Infrastructure.Services;

public sealed class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public MailKitEmailService(
        IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            _settings.UseSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None,
            cancellationToken);

        await smtp.AuthenticateAsync(
            _settings.Username,
            _settings.Password,
            cancellationToken);

        await smtp.SendAsync(
            message,
            cancellationToken);

        await smtp.DisconnectAsync(
            true,
            cancellationToken);
    }
}