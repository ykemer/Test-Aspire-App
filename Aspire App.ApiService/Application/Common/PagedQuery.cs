namespace Aspire_App.ApiService.Application.Common
{
    public class PagedQuery
    {
        private int _pageSize = 10;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 50 ? 50 : value;
        }
    }
}
