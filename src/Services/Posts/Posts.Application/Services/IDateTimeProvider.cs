namespace Posts.Application.Services;

public interface IDateTimeProvider
{
    DateTimeOffset Now { get; }
}