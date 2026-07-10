using ShopSphere.Domain.Enums;

namespace ShopSphere.Domain.Entities;

public sealed class Review : AuditableEntity
{
    private Review()
    {
    }

    private Review(
        Guid productId,
        Guid customerId,
        int rating,
        string comment)
    {
        ProductId = productId;
        CustomerId = customerId;
        Rating = rating;
        Comment = comment;
        IsApproved = false;
    }

    public Guid ProductId { get; private set; }

    public Guid CustomerId { get; private set; }

    public int Rating { get; private set; }

    public string Comment { get; private set; } = string.Empty;

    public bool IsApproved { get; private set; }
    public Product Product { get; private set; } = null!;
    public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;

    public static Review Create(
        Guid productId,
        Guid customerId,
        int rating,
        string comment)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rating),
                "Rating must be between 1 and 5.");
        }

        return new Review(
            productId,
            customerId,
            rating,
            comment.Trim());
    }

    public void Update(
        int rating,
        string comment)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rating));
        }

        Rating = rating;
        Comment = comment.Trim();
    }

    public void Approve()
    {
        IsApproved = true;
        Status = ReviewStatus.Approved;
    }

    public void Reject()
    {
        IsApproved = false;
        Status = ReviewStatus.Rejected;
    }
}