namespace ShopSphere.Application.Notifications;

public sealed record WelcomeEmailModel(
    string FullName,
    string Email);