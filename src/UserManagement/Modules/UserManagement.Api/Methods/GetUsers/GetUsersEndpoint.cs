using Carter;
using Common.Pagination;
using MediatR;
using UserManagement.Api.Methods.GetUsers.Models;

namespace UserManagement.Api.Methods.GetUsers;

public class GetUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async ([AsParameters] PaginationRequest request, ISender sender) =>
        {
            var query = new GetUsersQuery(request);

            var result = await sender.Send(query);

            var response = new GetUsersResponse(request, result);

            return Results.Ok(response);
        })
            .RequireAuthorization()
            .WithName("GetUsers")
            .WithSummary("GetUsers")
            .WithDescription("Get all users")
            .Produces<GetUsersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
