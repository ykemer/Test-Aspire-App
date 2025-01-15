namespace Contracts.Common;

public class PagedList<T>
{
  public PagedList()
  {
  }

  public PagedList(List<T> items, int count, int pageNumber, int pageSize)
  {
    TotalCount = count;
    PageSize = pageSize;
    CurrentPage = pageNumber;
    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    Items = items;
  }

  public int CurrentPage { get; set; }
  public int TotalPages { get; set; }
  public int PageSize { get; set; }
  public int TotalCount { get; set; }
  public List<T> Items { get; set; }

  public bool HasPrevious => CurrentPage > 1;
  public bool HasNext => CurrentPage < TotalPages;

  public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
  {
    int count = source.Count();
    List<T>? items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    return new PagedList<T>(items, count, pageNumber, pageSize);
  }

  public static PagedList<T> Create(IList<T> source, int pageNumber, int pageSize)
  {
    int count = source.Count();
    List<T>? items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    return new PagedList<T>(items, count, pageNumber, pageSize);
  }
}
