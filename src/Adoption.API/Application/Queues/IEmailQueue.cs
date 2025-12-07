using Adoption.API.Application.Services.Email;
using MimeKit;

namespace Adoption.API.Application.Queues;

public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailRequest email);
    ValueTask<EmailRequest> DequeueAsync(CancellationToken cancellationToken);
    bool TryDequeue(out EmailRequest? email);
}
