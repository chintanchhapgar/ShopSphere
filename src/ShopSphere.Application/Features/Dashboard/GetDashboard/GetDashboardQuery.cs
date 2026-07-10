using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Dashboard.GetDashboard;

public sealed record GetDashboardQuery()
    : IRequest<Result<DashboardResponse>>;