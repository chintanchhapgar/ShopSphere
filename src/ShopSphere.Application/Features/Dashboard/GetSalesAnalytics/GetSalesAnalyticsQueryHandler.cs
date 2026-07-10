using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Dashboard.GetSalesAnalytics;

public sealed class GetSalesAnalyticsQueryHandler
    : IRequestHandler<GetSalesAnalyticsQuery, Result<SalesAnalyticsResponse>>
{
    private readonly IStatisticsReadRepository _repository;

    public GetSalesAnalyticsQueryHandler(
        IStatisticsReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SalesAnalyticsResponse>> Handle(
        GetSalesAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetSalesAnalyticsAsync(
            request.Days,
            cancellationToken);

        return Result<SalesAnalyticsResponse>.Success(result);
    }
}