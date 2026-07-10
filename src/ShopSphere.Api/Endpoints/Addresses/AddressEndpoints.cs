using MediatR;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Api.Extensions;
using ShopSphere.Application.Features.Addresses.CreateAddress;
using ShopSphere.Application.Features.Addresses.DeleteAddress;
using ShopSphere.Application.Features.Addresses.GetAddresses;
using ShopSphere.Application.Features.Addresses.SetDefaultAddress;
using ShopSphere.Application.Features.Addresses.UpdateAddress;

namespace ShopSphere.Api.Endpoints.Addresses;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/addresses")
            .WithTags("Addresses")
            .RequireAuthorization();

        // Create Address
        group.MapPost(
            "/",
            async (
                CreateAddressCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    command,
                    cancellationToken);

                return result.ToCreatedHttpResult(
                    $"/api/addresses/{result.Value}");
            })
            .WithName("CreateAddress")
            .WithSummary("Create Address")
            .WithDescription("Creates a new shipping address.");

        // Get My Addresses
        group.MapGet(
            "/",
            async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetAddressesQuery(),
                    cancellationToken);

                return result.ToHttpResult();
            })
            .WithName("GetAddresses")
            .WithSummary("Get My Addresses")
            .WithDescription("Returns all addresses of the authenticated customer.");

        // Set Default Address
        group.MapPut(
            "/{id:guid}/default",
            async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new SetDefaultAddressCommand(id),
                    cancellationToken);

                return result.ToHttpResult();
            })
            .WithName("SetDefaultAddress")
            .WithSummary("Set Default Address")
            .WithDescription("Marks the selected address as the default address.");

        group.MapPut(
            "/{id:guid}",
            async (
                Guid id,
                UpdateAddressCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    command with { AddressId = id },
                    cancellationToken);

                return result.ToHttpResult();
            });

        group.MapDelete(
            "/{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new DeleteAddressCommand(id),
                    cancellationToken);

                return result.ToHttpResult();
            });

        return app;
    }
}