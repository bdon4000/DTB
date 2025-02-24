namespace DTB.Pages.App.User
{
    public class UserPage
    {
        public List<AppUser> UserDatas { get; set; }

        public string? Role { get; set; }

        //public string? Plan { get; set; }

        public string? Status { get; set; }

        public string? Search { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int PageCount => (int)Math.Ceiling(CurrentCount / (double)PageSize);

        public int CurrentCount => GetFilterDatas().Count();

        public UserPage(List<AppUser> datas)
        {
            UserDatas = new List<AppUser>();
            UserDatas.AddRange(datas);
        }

        private IEnumerable<AppUser> GetFilterDatas()
        {
            IEnumerable<AppUser> datas = UserDatas;

            if (!string.IsNullOrEmpty(Search))
            {
                datas = datas.Where(d => d.UserName.Contains(Search, StringComparison.OrdinalIgnoreCase) || d.JobNumber?.Contains(Search, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (Role is not null)
            {
                datas = datas.Where(d => d.Role == Role);
            }

            if (Status is not null)
            {
                datas = datas.Where(d => d.Status == Status);
            }

            if (datas.Count() < (PageIndex - 1) * PageSize) PageIndex = 1;

            return datas;
        }

        public List<AppUser> GetPageDatas()
        {
            return GetFilterDatas().Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }
    }

}
