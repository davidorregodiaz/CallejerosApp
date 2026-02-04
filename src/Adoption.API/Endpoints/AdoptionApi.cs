using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.AdoptionRequests;
using Adoption.API.Application.Commands.Appointments;
using Adoption.API.Application.Models;
using Adoption.API.Application.Queries;
using Adoption.API.Application.Queries.AdoptionRequests;
using Adoption.API.Extensions;
using Adoption.Infrastructure.Migrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Adoption.API.Endpoints;

public static class AdoptionApi
{
    public static IEndpointRouteBuilder MapAdoptionEndpoints(this IEndpointRouteBuilder app)
    {
        var adoptionApi = app
            .MapGroup("/adoptions")
            .WithTags("Adoptions");
        
        adoptionApi.MapPost("/{adoptionRequestId:guid}/appointments", CreateAppointmentAsync)
            .Produces<Ok<AdoptionViewModel>>()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("OwnerPolicy");

        adoptionApi.MapPut("/{adoptionRequestId:guid}/appointments/{appointmentId:guid}/reschedule", RescheduleAppointmentAsync)
            .WithSummary("Marks an appointment from an adoption request as requested for reschedulation.")
            .Produces<Ok>(statusCode: StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerRequesterPolicy");
        
        adoptionApi.MapPut("/{adoptionRequestId:guid}/appointments/{appointmentId:guid}/schedule", ScheduleAppointment)
            .WithSummary("Marks an appointment from an adoption request as scheduled")
            .Produces<Ok>(statusCode: StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerRequesterPolicy");
        
        adoptionApi.MapPut("/{adoptionRequestId:guid}/appointments/{appointmentId:guid}/complete", CompleteAppointment)
            .WithSummary("Marks an appointment from an adoption request as cancelled")
            .Produces<Ok>(statusCode: StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerRequesterPolicy");
        
        adoptionApi.MapPut("/{adoptionRequestId:guid}/appointments/{appointmentId:guid}/cancel", CancelAppointment)
            .WithSummary("Marks an appointment from an adoption request as cancelled")
            .Produces<Ok>(statusCode: StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerRequesterPolicy");
        
        adoptionApi.MapDelete("/{adoptionRequestId:guid}/appointments/{AppointmentId:guid}/", DeleteAppointmentAsync)
            .WithSummary("Deletes an appointment from an adoption request")
            .Produces<Ok>()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerPolicy");

        adoptionApi.MapPost("/", CreateAdoptionRequestAsync)
            .Produces<CreatedAtRoute<AdoptionViewModel>>(StatusCodes.Status201Created)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("RequesterPolicy");
        
        adoptionApi.MapGet("/{Id:guid}", GetAdoptionRequestByIdAsync)
            .WithName("GetAdoptionRequestById")
            .WithSummary("Gets an adoption request by id")
            .Produces<Ok<AdoptionViewModel>>()
            .Produces<NotFound<string>>(StatusCodes.Status404NotFound)
            .AllowAnonymous();
        
        adoptionApi.MapGet("/", GetAllAdoptionsRequestAsync)
            .WithName("GetAllAdoptionRequests")
            .WithSummary("Lists all the adoption requests.")
            .Produces<NoContent>()
            .Produces<Ok<PaginatedResponse<AdoptionViewModel>>>()
            .AllowAnonymous();

        adoptionApi.MapDelete("/{Id:guid}", DeleteAdoptionRequestAsync)
            .WithSummary("Deletes an adoption request")
            .Produces<NoContent>()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("RequesterPolicy", "OwnerPolicy");

        adoptionApi.MapPut("/{Id:guid}", UpdateAdoptionRequestAsync)
            .WithSummary("Updates an adoption request")
            .Produces<Ok<AdoptionViewModel>>(statusCode: StatusCodes.Status200OK)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerPolicy");
        
        return app;
    }

    private static async Task<Results<Ok, ProblemHttpResult>> CancelAppointment(
        [FromRoute] Guid adoptionRequestId,
        [FromRoute] Guid appointmentId,
        ICommandHandler<CancelAppointmentCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CancelAppointmentCommand(
            AdoptionRequestId: adoptionRequestId,
            AppointmentId: appointmentId);
            
        await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok();
    }
    
    private static async Task<Results<Ok, ProblemHttpResult>> CompleteAppointment(
        [FromRoute] Guid adoptionRequestId,
        [FromRoute] Guid appointmentId,
        ICommandHandler<CompleteAppointmentCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CompleteAppointmentCommand(
            AdoptionRequestId: adoptionRequestId,
            AppointmentId: appointmentId);
            
        await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok();
    }
    
    private static async Task<Results<Ok, ProblemHttpResult>> ScheduleAppointment(
        [FromRoute] Guid adoptionRequestId,
        [FromRoute] Guid appointmentId,
        ICommandHandler<ScheduleAppointmentCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new ScheduleAppointmentCommand(
            AdoptionRequestId: adoptionRequestId,
            AppointmentId: appointmentId);
            
        await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, ProblemHttpResult>> DeleteAppointmentAsync(
        [FromRoute] Guid adoptionRequestId,
        [FromRoute] Guid appointmentId,
        ICommandHandler<DeleteAppointmentCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAppointmentCommand(
            AppointmentId: appointmentId,
            AdoptionRequestId: adoptionRequestId);
        
        await handler.HandleAsync(command, cancellationToken);
        return TypedResults.Ok();
    }
    
    private static async Task<Results<Ok, ProblemHttpResult>> RescheduleAppointmentAsync(
        [FromRoute] Guid adoptionRequestId,
        [FromRoute] Guid appointmentId,
        [FromBody] RescheduleAppointmentRequest request,
        ICommandHandler<RescheduleAppointmentCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RescheduleAppointmentCommand(
            AdoptionRequestId: adoptionRequestId,
            AppointmentId: appointmentId,
            DateProposed: request.DateProposed,
            RescheduleMessage: string.IsNullOrEmpty(request.RescheduleMessage) ? string.Empty : request.RescheduleMessage);
        
        await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok();
    }
    
    private static async Task<Results<Ok<AppointmentViewModel>, ProblemHttpResult>> CreateAppointmentAsync(
        [FromRoute] Guid adoptionRequestId,
        CreateAppointmentRequest request,
        ICommandHandler<CreateAppointmentCommand, AppointmentViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAppointmentCommand(
            AdoptionRequestid: adoptionRequestId, 
            Date: request.Date,
            Notes: request.Notes,
            Location: request.Location);
        
        var response = await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok(response);
    }
    private static async Task<Results<Ok<AdoptionViewModel>, ProblemHttpResult>> UpdateAdoptionRequestAsync(
        [FromRoute] Guid id,
        UpdateAdoptionRequest request,
        ICommandHandler<UpdateAdoptionRequestCommand, AdoptionViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAdoptionRequestCommand(
            Id: id,
            Status: request.Status);
        var response = await handler.HandleAsync(command, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteAdoptionRequestAsync(
        [AsParameters] DeleteAdoptionRequest request,
        ICommandHandler<DeleteAdoptionRequestCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAdoptionRequestCommand(request.Id);
        
        await handler.HandleAsync(command, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<PaginatedResponse<AdoptionViewModel>>,NoContent>> GetAllAdoptionsRequestAsync(
        [AsParameters] GetAllAdoptionsQueryRequest request,
        HttpContext httpContext,
        IQueryHandler<GetAllAdoptionsQuery, PaginatedResponse<AdoptionViewModel>> handler,
        CancellationToken cancellationToken = default)
    {

        var userId = httpContext.GetUserIdFromContext();
        
        var query = new GetAllAdoptionsQuery(
            UserId: userId ?? Guid.Empty,
            Status: request.Status,
            Date: request.Date,
            Page:  request.Page,
            PageSize: request.PageSize,
            SortBy: request.SortBy,
            IsDescending: request.IsDescending);
        
        var result = await handler.HandleAsync(query, cancellationToken);
        
        if (!result.IsSuccessful(out var response))
            return TypedResults.NoContent();
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AdoptionViewModel>, NotFound<string>>> GetAdoptionRequestByIdAsync(
        [AsParameters] GetAdoptionRequestByIdQuery query,
        IQueryHandler<GetAdoptionRequestByIdQuery,  AdoptionViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        if (!result.IsSuccessful(out var response))
            return TypedResults.NotFound(result.Message);
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<CreatedAtRoute<AdoptionViewModel>, ProblemHttpResult>> CreateAdoptionRequestAsync(
        CreateAdoptionRequest request,
        ICommandHandler<CreateAdoptionRequestCommand, AdoptionViewModel> handler,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserIdFromContext();
        
        var command = new CreateAdoptionRequestCommand(
            RequesterId: userId ?? Guid.Empty,
            Comments: request.Comments,
            AnimalId: request.AnimalId);
        
        var response = await handler.HandleAsync(command,cancellationToken);
        return TypedResults.CreatedAtRoute(response, routeName: "GetAdoptionRequestById",
            routeValues: new { id = response.AdoptionRequestId });
    }
}
