namespace Posts.Application.Services;

public class DefaultDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}