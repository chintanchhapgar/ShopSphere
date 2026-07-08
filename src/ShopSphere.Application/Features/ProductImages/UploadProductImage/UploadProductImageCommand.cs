using MediatR;
using ShopSphere.Contracts.Common;

public sealed record UploadProductImageCommand(
    Guid ProductId,
    Stream FileStream,
    string FileName,
    long FileSize,
    bool IsPrimary)
    : IRequest<Result<Guid>>;