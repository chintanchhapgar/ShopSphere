namespace ShopSphere.Domain.Constants;

public static class AuditActions
{
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";

    public const string Register = "Register";
    public const string Login = "Login";
    public const string EmailVerified = "EmailVerified";
    public const string PasswordReset = "PasswordReset";

    public const string CreateOrder = "CreateOrder";
    public const string CancelOrder = "CancelOrder";

    public const string PaymentCreated = "PaymentCreated";
    public const string PaymentSucceeded = "PaymentSucceeded";
    public const string PaymentFailed = "PaymentFailed";
    public const string PaymentRefunded = "PaymentRefunded";

    public const string ShipmentCreated = "ShipmentCreated";
    public const string ShipmentShipped = "ShipmentShipped";
    public const string ShipmentDelivered = "ShipmentDelivered";
    public const string ShipmentProcessing = "ShipmentProcessing";
    public const string ShipmentReturned = "ShipmentReturned";
}