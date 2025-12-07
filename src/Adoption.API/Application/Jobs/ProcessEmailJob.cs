using Adoption.API.Application.Queues;
using Adoption.API.Application.Services.Email;
using Quartz;

namespace Adoption.API.Application.Jobs;

public class ProcessEmailJob(IEmailService emailService, IEmailQueue emailQueue, ILogger<ProcessEmailJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        while (emailQueue.TryDequeue(out var email))
        {
            try
            {
                logger.LogInformation("Enter on {ProcessEmailJob}", nameof(ProcessEmailJob));
                await emailService.SendEmailAsync(email!);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error sending email from job: {ProcessEmailJob}",  nameof(ProcessEmailJob));
                //throw
            }
        }
    }
}
