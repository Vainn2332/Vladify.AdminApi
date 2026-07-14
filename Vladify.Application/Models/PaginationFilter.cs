namespace Vladify.Application.Models;

public record PaginationFilter(int PageNumber = 1, int PageSize = 10)
{
    public int Offset => (PageNumber - 1) * PageSize;
}
