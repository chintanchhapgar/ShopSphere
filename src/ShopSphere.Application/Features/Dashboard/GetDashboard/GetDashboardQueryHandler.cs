using MediatR;
using ShopSphere.Application.Features.Dashboard.GetDashboard;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;

public sealed class GetDashboardQueryHandler
    : IRequestHandler<GetDashboardQuery, Result<DashboardResponse>>
{
    private readonly IStatisticsReadRepository _repository;

    public GetDashboardQueryHandler(
        IStatisticsReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DashboardResponse>> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var dashboard =
            await _repository.GetDashboardAsync(
                cancellationToken);

        return Result<DashboardResponse>.Success(
            dashboard);
    }
}