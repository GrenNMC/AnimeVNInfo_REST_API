using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IAdminNavRepository
    {
        public Task<ResponseWithPaginationView> GetAllAdminNav(AdminNavParamView AdminNavParamView, CancellationToken cancellationToken);
        public Task<AdminNavigationView> GetAdminNavById(int id, CancellationToken cancellationToken);
        public Task<AdminNavigationView> GetAdminNavByLink(string link, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken);
    }

    public class AdminNavigationRepository : IAdminNavRepository
    {
        private readonly ILogger<AdminNavigationRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AdminNavigationRepository(ApplicationDbContext context, ILogger<AdminNavigationRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseView> ChangeNextOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            try
            {
                var adminnav = await _context.AdminNavigations.Where(s => (s.Id == AdminNavView.Id && s.Id != 1 && s.Id != 2 && s.Id != 3 && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (adminnav == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AdminNavigations.Where(s => (s.ParentId == adminnav.ParentId && !s.IsDeleted)).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == adminnav.Id);
                if (index >= list.Count - 1)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                list[index].Order++;
                list[index + 1].Order--;
                list[index].UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> ChangePrevOrder(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            try
            {
                var adminnav = await _context.AdminNavigations.Where(s => (s.Id == AdminNavView.Id && s.Id != 1 && s.Id != 2 && s.Id != 3 && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (adminnav == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var list = await _context.AdminNavigations.Where(s => (s.ParentId == adminnav.ParentId && !s.IsDeleted)).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var index = list.FindIndex(s => s.Id == adminnav.Id);
                if (index <= 0)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                list[index].Order--;
                list[index - 1].Order++;
                list[index].UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> CreateAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            try
            {
                AdminNavigation newAdminNav = new();
                newAdminNav = _mapper.Map<AdminNavigation>(AdminNavView);
                newAdminNav.CreatedAt = DateTime.UtcNow;
                newAdminNav.UpdatedAt = DateTime.UtcNow;
                var list = await _context.AdminNavigations.Where(s => (s.ParentId == AdminNavView.ParentId && !s.IsDeleted)).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                var lastNav = list.OrderByDescending(s => s.Order).ThenBy(s => s.Id).FirstOrDefault();
                if (lastNav != null)
                {
                    newAdminNav.Order = lastNav.Order + 1;
                }
                else
                {
                    newAdminNav.Order = 1;
                }
                await _context.AdminNavigations.AddAsync(newAdminNav, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            try
            {
                var adminnav = await _context.AdminNavigations.Where(s => (s.Id == AdminNavView.Id && s.Id != 1 && s.Id != 2 && s.Id != 3 && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (adminnav == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var adminNavList = await _context.AdminNavigations.Where(s => (s.ParentId == AdminNavView.Id && s.ParentId != 1 && !s.IsDeleted)).ToListAsync(cancellationToken);
                adminnav.IsDeleted = true;
                adminnav.DeletedAt = DateTime.UtcNow;
                foreach (var nav in adminNavList)
                {
                    nav.IsDeleted = true;
                    nav.DeletedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync(cancellationToken);
                var list = await _context.AdminNavigations.Where(s => (s.ParentId == adminnav.ParentId && !s.IsDeleted)).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<AdminNavigationView> GetAdminNavById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var adminNav = from s in _context.AdminNavigations
                               where s.Id == id && !s.IsDeleted
                               select new AdminNavigationView
                               {
                                   Id = s.Id,
                                   IsDisabled = s.IsDisabled,
                                   Link = s.Link,
                                   Order = s.Order,
                                   ParentId = s.ParentId,
                                   Title = s.Title,
                                   AdminRole = (s.RoleId == 1 ? true : false),
                                   ModeratorRole = ((s.RoleId == 1 || s.RoleId == 2) ? true : false),
                                   UserRole = ((s.RoleId == 1 || s.RoleId == 2 || s.RoleId == 3) ? true : false),
                                   RoleId = s.RoleId
                               };
                return await adminNav.FirstOrDefaultAsync(cancellationToken) ?? new AdminNavigationView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AdminNavigationView();
            }
        }

        public async Task<AdminNavigationView> GetAdminNavByLink(string link, CancellationToken cancellationToken)
        {
            try
            {
                var adminNav = from s in _context.AdminNavigations
                               where s.Link == link &&
                                     !s.IsDisabled && !s.IsDeleted
                               select new AdminNavigationView
                               {
                                   Id = s.Id,
                                   IsDisabled = s.IsDisabled,
                                   Link = s.Link,
                                   Order = s.Order,
                                   ParentId = s.ParentId,
                                   Title = s.Title,
                                   AdminRole = (s.RoleId == 1 ? true : false),
                                   ModeratorRole = ((s.RoleId == 1 || s.RoleId == 2) ? true : false),
                                   UserRole = ((s.RoleId == 1 || s.RoleId == 2 || s.RoleId == 3) ? true : false),
                                   RoleId = s.RoleId,
                                   SubNavigation = (from ss in _context.AdminNavigations
                                                    where s.Id == ss.ParentId &&
                                                          !ss.IsDisabled && !ss.IsDeleted
                                                    orderby ss.Order
                                                    select new AdminNavigationView
                                                    {
                                                        Id = ss.Id,
                                                        IsDisabled = ss.IsDisabled,
                                                        Link = ss.Link,
                                                        Order = ss.Order,
                                                        ParentId = ss.ParentId,
                                                        Title = ss.Title,
                                                        AdminRole = (ss.RoleId == 1 ? true : false),
                                                        ModeratorRole = ((ss.RoleId == 1 || ss.RoleId == 2) ? true : false),
                                                        UserRole = ((ss.RoleId == 1 || ss.RoleId == 2 || ss.RoleId == 3) ? true : false),
                                                        RoleId = ss.RoleId
                                                    }).ToList()
                               };
                return await adminNav.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseWithPaginationView> GetAllAdminNav(AdminNavParamView AdminNavParamView, CancellationToken cancellationToken)
        {
            try
            {
                var adminnavs = from s in _context.AdminNavigations
                                where s.ParentId == 0 && !s.IsDeleted
                                orderby s.Order
                                select new AdminNavigationView
                                {
                                    Id = s.Id,
                                    IsDisabled = s.IsDisabled,
                                    Link = s.Link,
                                    Order = s.Order,
                                    ParentId = s.ParentId,
                                    Title = s.Title,
                                    AdminRole = (s.RoleId == 1 ? true : false),
                                    ModeratorRole = ((s.RoleId == 1 || s.RoleId == 2) ? true : false),
                                    UserRole = ((s.RoleId == 1 || s.RoleId == 2 || s.RoleId == 3) ? true : false),
                                    RoleId = s.RoleId,
                                    SubNavigation = (from ss in _context.AdminNavigations
                                                     where s.Id == ss.ParentId && !ss.IsDeleted
                                                     orderby ss.Order
                                                     select new AdminNavigationView
                                                     {
                                                         Id = ss.Id,
                                                         IsDisabled = ss.IsDisabled,
                                                         Link = ss.Link,
                                                         Order = ss.Order,
                                                         ParentId = ss.ParentId,
                                                         Title = ss.Title,
                                                         AdminRole = (ss.RoleId == 1 ? true : false),
                                                         ModeratorRole = ((ss.RoleId == 1 || ss.RoleId == 2) ? true : false),
                                                         UserRole = ((ss.RoleId == 1 || ss.RoleId == 2 || ss.RoleId == 3) ? true : false),
                                                         RoleId = ss.RoleId
                                                     }).ToList()
                                };
                if (!await adminnavs.AnyAsync(cancellationToken))
                {
                    return new AdminNavListView(new List<AdminNavigationView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                var list = await adminnavs.ToListAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(AdminNavParamView.SearchString))
                {
                    List<AdminNavigationView> tmp = new();
                    foreach (var nav in list)
                    {
                        if ((nav.Title != null && nav.Title.ToLower().Contains(AdminNavParamView.SearchString.ToLower())) ||
                            (nav.Link != null && nav.Link.ToLower().Contains(AdminNavParamView.SearchString.ToLower())))
                        {
                            tmp.Add(nav);
                        }
                        else if ((nav.Title != null && !nav.Title.ToLower().Contains(AdminNavParamView.SearchString.ToLower())) &&
                            (nav.Link != null && !nav.Link.ToLower().Contains(AdminNavParamView.SearchString.ToLower())))
                        {
                            List<AdminNavigationView> tmp2 = new();
                            if (nav.SubNavigation != null)
                            {
                                foreach (var nav2 in nav.SubNavigation)
                                {
                                    if ((nav2.Title != null && nav2.Title.ToLower().Contains(AdminNavParamView.SearchString.ToLower())) ||
                                        (nav2.Link != null && nav2.Link.ToLower().Contains(AdminNavParamView.SearchString.ToLower())))
                                    {
                                        tmp2.Add(nav2);
                                    }
                                }
                            }
                            if (tmp2.Count > 0)
                            {
                                nav.SubNavigation = tmp2;
                                tmp.Add(nav);
                            }
                        }
                    }
                    list = tmp;
                }
                if (AdminNavParamView.IsDisabled != null)
                {
                    List<AdminNavigationView> tmp = new();
                    foreach (var nav in list)
                    {
                        if (nav.IsDisabled == AdminNavParamView.IsDisabled)
                        {
                            tmp.Add(nav);
                        }
                        else
                        {
                            List<AdminNavigationView> tmp2 = new();
                            if (nav.SubNavigation != null)
                            {
                                foreach (var nav2 in nav.SubNavigation)
                                {
                                    if (nav2.IsDisabled == AdminNavParamView.IsDisabled)
                                    {
                                        tmp2.Add(nav2);
                                    }
                                }
                            }
                            if (tmp2.Count > 0)
                            {
                                nav.SubNavigation = tmp2;
                                tmp.Add(nav);
                            }
                        }
                    }
                    list = tmp;
                }
                var totalRecord = list.Count();
                if (AdminNavParamView.PageIndex > 0) {
                    list = list.Skip(AdminNavParamView.PageSize * (AdminNavParamView.PageIndex - 1)).Take(AdminNavParamView.PageSize).ToList();
                }
                return new AdminNavListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AdminNavListView(new List<AdminNavigationView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseView> UpdateAdminNav(AdminNavigationView AdminNavView, CancellationToken cancellationToken)
        {
            try
            {
                var adminnav = await _context.AdminNavigations.Where(s => (s.Id == AdminNavView.Id && s.Id != 1 && s.Id != 2 && s.Id != 3 && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (adminnav == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                _mapper.Map(AdminNavView, adminnav);
                adminnav.UpdatedAt = DateTime.UtcNow;
                var list = await _context.AdminNavigations.Where(s => (s.ParentId == adminnav.ParentId && !s.IsDeleted)).OrderBy(s => s.Order).ThenBy(s => s.Id).ToListAsync(cancellationToken);
                list = reOrder(list);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        private List<AdminNavigation> reOrder(List<AdminNavigation> list)
        {
            for (var i = 1; i <= list.Count; i++)
            {
                list[i - 1].Order = i;
            }
            return list;
        }
    }
}
