using System.Threading.Channels;
using Adoption.API.Application.Services.Email;
using MimeKit;

namespace Adoption.API.Application.Queues;

public class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailRequest> _channel;

    public EmailQueue()
    {
        _channel = Channel.CreateUnbounded<EmailRequest>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public async ValueTask EnqueueAsync(EmailRequest email)
    {
        await _channel.Writer.WriteAsync(email);
    }

    public async ValueTask<EmailRequest> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
    
    public bool TryDequeue(out EmailRequest? email)
    {
        return _channel.Reader.TryRead(out email);
    }
}
