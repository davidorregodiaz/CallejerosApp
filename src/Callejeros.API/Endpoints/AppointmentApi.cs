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

        

        return app;
    }
    
    
    
    
}
