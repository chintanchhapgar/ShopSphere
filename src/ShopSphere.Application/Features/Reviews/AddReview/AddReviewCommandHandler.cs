using MediatR;
using ShopSphere.Application.Features.Products;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Reviews.AddReview;

public sealed class AddReviewCommandHandler
    : IRequestHandler<AddReviewCommand, Result<Guid>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddReviewCommandHandler(
        IReviewRepository reviewRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        AddReviewCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result<Guid>.Failure(
                UserErrors.Unauthorized);
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result<Guid>.Failure(
                ProductErrors.NotFound);
        }

        var exists =
            await _reviewRepository.ExistsByCustomerAsync(
                request.ProductId,
                customerId,
                cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(
                ReviewErrors.AlreadyExists);
        }

        var review = Review.Create(
            request.ProductId,
            customerId,
            request.Rating,
            request.Comment);

        await _reviewRepository.AddAsync(
            review,
            cancellationToken);

        await _reviewRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            review.Id,
            "Review submitted successfully.");
    }
}