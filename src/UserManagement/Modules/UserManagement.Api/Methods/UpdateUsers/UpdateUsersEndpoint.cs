using Carter;
using Mapster;
using MediatR;
using UserManagement.Api.Methods.UpdateUsers.Models;

namespace UserManagement.Api.Methods.UpdateUsers;

public class UpdateUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/users", async (UpdateUsersRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateUsersCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateUsersResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization()
            .WithName("EditUsers")
            .WithSummary("EditUsers")
            .WithDescription("Edit users")
            .Produces<UpdateUsersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
