namespace Bloggest.Common;

public sealed record PagedResult<T>(T[] Result, int Count);