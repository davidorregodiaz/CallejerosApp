using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.AdoptionRequests;
using Adoption.API.Application.Commands.Appointments;
using Adoption.API.Application.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Adoption.API.Endpoints;

public static class AppointmentApi
{

    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var appointmentApi = app
            .MapGroup("/appointments")
            .WithTags("Appointments");

        appointmentApi.MapPost("/", CreateAppointmentAsync)
            .Produces<Ok<AdoptionViewModel>>()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("OwnerPolicy");

        appointmentApi.MapPut("/{AppointmentId:guid}/reschedule", RescheduleAppointmentAsync)
            .WithSummary("Updates an adoption request")
            .Produces<Ok<AdoptionViewModel>>(statusCode: StatusCodes.Status200OK)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerPolicy");

        return app;
    }
    
    private static async Task<Results<Ok<AppointmentViewModel>, ProblemHttpResult>> CreateAppointmentAsync(
        CreateAppointmentRequest request,
        ICommandHandler<CreateAppointmentCommand, AppointmentViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAppointmentCommand(
            AdoptionRequestid: request.AdoptionRequestId,
            Date: request.Date,
            Notes: request.Notes,
            Location: request.Location);
        
        var response = await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok(response);
    }
    
    private static async Task<Results<Ok<AppointmentViewModel>, ProblemHttpResult>> RescheduleAppointmentAsync(
        [FromRoute] Guid AppointmentId,
        [FromBody] RescheduleAppointmentRequest request,
        ICommandHandler<RescheduleAppointmentCommand, AppointmentViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RescheduleAppointmentCommand(
            AppointmentId: AppointmentId,
            DateProposed: request.DateProposed,
            RescheduleMessage: string.IsNullOrEmpty(request.RescheduleMessage) ? string.Empty : request.RescheduleMessage);
        
        var response = await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok(response);
    }
}
