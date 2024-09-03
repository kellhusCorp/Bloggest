namespace Posts.Application.Dtos;

public sealed class PostDto
{
    public Guid Id { get; init; }

    public string Header { get; init; }

    public string Link { get; init; }

    public string AuthorId { get; init; }

    public DateTimeOffset PublishedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }

    public TagDto[] Tags { get; init; }
}