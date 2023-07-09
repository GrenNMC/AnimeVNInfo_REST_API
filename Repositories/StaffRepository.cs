using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Azure.Core.HttpHeader;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IStaffRepository
    {
        public Task<ResponseWithPaginationView> GetAllStaff(StaffParamView staffParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllStaff(StaffQueryListView staffQueryListView, CancellationToken cancellationToken);
        public Task<StaffView> GetStaffById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateStaff(StaffView staffView, CancellationToken cancellationToken);
        public Task<Tuple<ResponseView, string?, string?>> UpdateStaff(StaffView staffView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteStaff(StaffView staffView, CancellationToken cancellationToken);
    }

    public class StaffRepository : IStaffRepository
    {
        private readonly ILogger<StaffRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StaffRepository(ApplicationDbContext context, ILogger<StaffRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateStaff(StaffView staffView, CancellationToken cancellationToken)
        {
            try
            {
                var language = await _context.Languages.Where(s => !s.IsDeleted && s.Id == staffView.LanguageId).FirstOrDefaultAsync(cancellationToken);
                if(language == null)
                {
                    return new(MessageConstant.NOT_FOUND_LANGUAGE, 2);
                }
                Staff newStaff = new();
                newStaff = _mapper.Map<Staff>(staffView);
                newStaff.CreatedAt = DateTime.UtcNow;
                newStaff.UpdatedAt = DateTime.UtcNow;
                if (newStaff.StaffJob != null && newStaff.StaffJob.Count > 0)
                {
                    var listJobId = newStaff.StaffJob.Select(x => x.JobId).Distinct().ToList();
                    var allJob = await _context.Jobs.Where(s => !s.IsDeleted && listJobId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listJobId.Count();
                    if(!allJob) {
                        return new(MessageConstant.NOT_FOUND_JOB, 3);
                    }
                    newStaff.StaffJob.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                await _context.Staffs.AddAsync(newStaff, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteStaff(StaffView staffView, CancellationToken cancellationToken)
        {
            try
            {
                var staff = await _context.Staffs.Where(s => (s.Id == staffView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (staff == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                staff.IsDeleted = true;
                staff.DeletedAt = DateTime.UtcNow;
                await _context.StaffJobs.Where(s => (s.StaffId == staffView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                        .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.CharacterStaffs.Where(s => (s.StaffId == staffView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                        .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeStaffs.Where(s => (s.StaffId == staffView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                        .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllStaff(StaffParamView staffParamView, CancellationToken cancellationToken)
        {
            try
            {
                var staffs = from s in _context.Staffs
                             orderby s.Id
                             where !s.IsDeleted
                             join r in (from rt in _context.Languages where !rt.IsDeleted select rt) on s.LanguageId equals r.Id into sr
                             from rr in sr.DefaultIfEmpty()
                             select new StaffView
                             {
                                 LatinName = s.LatinName,
                                 NativeName = s.NativeName,
                                 Id = s.Id,
                                 IdAnilist = s.IdAnilist,
                                 CreatedAt = s.CreatedAt,
                                 BloodType = s.BloodType,
                                 HomeTown = s.HomeTown,
                                 IsMale = s.IsMale,
                                 OtherInformation = s.OtherInformation,
                                 LanguageId = s.LanguageId,
                                 LanguageName = rr.LanguageName,
                                 DayOfBirth = s.DayOfBirth,
                                 DayOfDeath = s.DayOfDeath,
                                 MonthOfBirth = s.MonthOfBirth,
                                 MonthOfDeath = s.MonthOfDeath,
                                 OtherName = s.OtherName,
                                 YearOfBirth = s.YearOfBirth,
                                 YearOfDeath = s.YearOfDeath,
                                 CharacterCount = (from ss in _context.CharacterStaffs
                                                   where ss.StaffId == s.Id && !ss.IsDeleted
                                                   select ss).Count()
                             };
                if (!await staffs.AnyAsync(cancellationToken))
                {
                    return new StaffListView(new List<StaffView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(staffParamView.SearchString))
                {
                    staffs = staffs.Where(s => ((s.NativeName ?? "").ToLower().Contains(staffParamView.SearchString.ToLower()) ||
                                                        (s.LatinName ?? "").ToLower().Contains(staffParamView.SearchString.ToLower()) ||
                                                        (s.OtherName ?? "").ToLower().Contains(staffParamView.SearchString.ToLower()) ||
                                                        (s.HomeTown ?? "").ToLower().Contains(staffParamView.SearchString.ToLower()) ||
                                                        (s.OtherInformation ?? "").ToLower().Contains(staffParamView.SearchString.ToLower()) ||
                                                        (s.LanguageName ?? "").ToLower().Contains(staffParamView.SearchString.ToLower())));
                }
                if (!string.IsNullOrWhiteSpace(staffParamView.SearchName))
                {
                    staffs = staffs.Where(s => ((s.NativeName ?? "").ToLower().Contains(staffParamView.SearchName.ToLower()) ||
                                                        (s.LatinName ?? "").ToLower().Contains(staffParamView.SearchName.ToLower()) ||
                                                        (s.OtherName ?? "").ToLower().Contains(staffParamView.SearchName.ToLower())));
                }
                if (staffParamView.LanguageId != null)
                {
                    staffs = staffs.Where(s => s.LanguageId == staffParamView.LanguageId);
                }
                if (staffParamView.DayOfBirth != null && staffParamView.DayOfBirth > 0)
                {
                    staffs = staffs.Where(s => s.DayOfBirth == staffParamView.DayOfBirth);
                }
                if (staffParamView.MonthOfBirth != null && staffParamView.MonthOfBirth > 0)
                {
                    staffs = staffs.Where(s => s.MonthOfBirth == staffParamView.MonthOfBirth);
                }
                if (staffParamView.YearOfBirth != null && staffParamView.YearOfBirth > 0)
                {
                    staffs = staffs.Where(s => s.YearOfBirth == staffParamView.YearOfBirth);
                }
                if (staffParamView.DayOfDeath != null && staffParamView.DayOfDeath > 0)
                {
                    staffs = staffs.Where(s => s.DayOfDeath == staffParamView.DayOfDeath);
                }
                if (staffParamView.MonthOfDeath != null && staffParamView.MonthOfDeath > 0)
                {
                    staffs = staffs.Where(s => s.MonthOfDeath == staffParamView.MonthOfDeath);
                }
                if (staffParamView.YearOfDeath != null && staffParamView.YearOfDeath > 0)
                {
                    staffs = staffs.Where(s => s.YearOfDeath == staffParamView.YearOfDeath);
                }
                if (staffParamView.JobId != null && staffParamView.JobId.Count > 0)
                {
                    if (staffParamView.IsJobGroup == true)
                    {
                        var _sg = _context.StaffJobs.Where(s => (staffParamView.JobId.Contains(s.JobId ?? 0)) && !s.IsDeleted);
                        var _sgGroup = _sg.GroupBy(s => s.StaffId).Select(s => new
                        {
                            StaffId = s.Key,
                            Count = s.Count()
                        });
                        staffs = staffs.Join(_sgGroup.Where(s => s.Count == staffParamView.JobId.Count()), s => s.Id, r => r.StaffId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                    else
                    {
                        staffs = staffs.Join(_context.StaffJobs.Where(s => (staffParamView.JobId.Contains(s.JobId ?? 0)) &&
                                                                            !s.IsDeleted), s => s.Id, r => r.StaffId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                }
                if (staffParamView.Order == "asc")
                {
                    if (staffParamView.OrderBy == "latinName")
                    {
                        staffs = staffs.OrderBy(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (staffParamView.OrderBy == "nativeName")
                    {
                        staffs = staffs.OrderBy(s => s.NativeName).ThenBy(s => s.Id);
                    }
                    else if (staffParamView.OrderBy == "createdAt")
                    {
                        staffs = staffs.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (staffParamView.OrderBy == "characterCount")
                    {
                        staffs = staffs.OrderBy(s => s.CharacterCount).ThenBy(s => s.Id);
                    }
                }
                else if (staffParamView.Order == "desc")
                {
                    if (staffParamView.OrderBy == "latinName")
                    {
                        staffs = staffs.OrderByDescending(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (staffParamView.OrderBy == "nativeName")
                    {
                        staffs = staffs.OrderByDescending(s => s.NativeName).ThenBy(s => s.Id);
                    }
                    else if (staffParamView.OrderBy == "createdAt")
                    {
                        staffs = staffs.OrderByDescending(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (staffParamView.OrderBy == "characterCount")
                    {
                        staffs = staffs.OrderByDescending(s => s.CharacterCount).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await staffs.CountAsync(cancellationToken);
                if (staffParamView.PageIndex > 0)
                {
                    staffs = staffs.Skip(staffParamView.PageSize * (staffParamView.PageIndex - 1)).Take(staffParamView.PageSize);
                }
                staffs = staffs.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                var list = await staffs.ToListAsync(cancellationToken);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            item.StaffJob = await (from sv in _context.StaffJobs
                                                   where !sv.IsDeleted && sv.StaffId == item.Id
                                                   join j in (from jt in _context.Jobs where !jt.IsDeleted select jt) on sv.JobId equals j.Id
                                                   select new StaffJobView {
                                                       Id = sv.Id,
                                                       JobId = sv.JobId,
                                                       StaffId = sv.StaffId,
                                                       IsMain = sv.IsMain,
                                                       StaffJobName = j.JobName
                                                   }).ToListAsync(cancellationToken);
                            item.StaffImage = await (from si in _context.StaffImages
                                                     orderby si.UpdatedAt descending
                                                     where si.StaffId == item.Id && !si.IsDeleted
                                                     select new StaffImageView {
                                                         Id = si.Id,
                                                         StaffId = si.StaffId,
                                                         Image = si.Image,
                                                         IsAvatar = si.IsAvatar
                                                     }).ToListAsync(cancellationToken);
                        }
                    }
                }
                return new StaffListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new StaffListView(new List<StaffView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllStaff(StaffQueryListView staffQueryListView, CancellationToken cancellationToken)
        {
            try
            {
                if(staffQueryListView.Id == null)
                {
                    return new StaffListView(new(), 0, 2, MessageConstant.NO_DATA_REQUEST);
                }
                var staffs = from s in _context.Staffs
                             orderby s.Id
                             where !s.IsDeleted && staffQueryListView.Id.Contains(s.IdAnilist)
                             join r in (from rt in _context.Languages where !rt.IsDeleted select rt) on s.LanguageId equals r.Id into sr
                             from rr in sr.DefaultIfEmpty()
                             select new StaffView
                             {
                                 LatinName = s.LatinName,
                                 NativeName = s.NativeName,
                                 Id = s.Id,
                                 IdAnilist = s.IdAnilist,
                                 CreatedAt = s.CreatedAt,
                                 BloodType = s.BloodType,
                                 HomeTown = s.HomeTown,
                                 IsMale = s.IsMale,
                                 OtherInformation = s.OtherInformation,
                                 LanguageId = s.LanguageId,
                                 LanguageName = rr.LanguageName,
                                 DayOfBirth = s.DayOfBirth,
                                 DayOfDeath = s.DayOfDeath,
                                 MonthOfBirth = s.MonthOfBirth,
                                 MonthOfDeath = s.MonthOfDeath,
                                 OtherName = s.OtherName,
                                 YearOfBirth = s.YearOfBirth,
                                 YearOfDeath = s.YearOfDeath,
                                 CharacterCount = (from ss in _context.CharacterStaffs
                                                   where ss.StaffId == s.Id && !ss.IsDeleted
                                                   select ss).Count()
                             };
                var totalRecord = await staffs.CountAsync(cancellationToken);
                staffs = staffs.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                var list = await staffs.ToListAsync(cancellationToken);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {

                            item.StaffJob = await (from sv in _context.StaffJobs
                                                   where !sv.IsDeleted && sv.StaffId == item.Id
                                                   join j in (from jt in _context.Jobs where !jt.IsDeleted select jt) on sv.JobId equals j.Id
                                                   select new StaffJobView {
                                                       Id = sv.Id,
                                                       JobId = sv.JobId,
                                                       StaffId = sv.StaffId,
                                                       IsMain = sv.IsMain,
                                                       StaffJobName = j.JobName
                                                   }).ToListAsync(cancellationToken);
                            item.StaffImage = await (from si in _context.StaffImages
                                                     orderby si.UpdatedAt descending
                                                     where si.StaffId == item.Id && !si.IsDeleted
                                                     select new StaffImageView {
                                                         Id = si.Id,
                                                         StaffId = si.StaffId,
                                                         Image = si.Image,
                                                         IsAvatar = si.IsAvatar
                                                     }).ToListAsync(cancellationToken);
                        }
                    }
                }
                return new StaffListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new StaffListView(new List<StaffView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<StaffView> GetStaffById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var staff = from s in _context.Staffs
                            where s.Id == id && !s.IsDeleted
                            join r in (from rt in _context.Languages where !rt.IsDeleted select rt) on s.LanguageId equals r.Id into sr
                            from rr in sr.DefaultIfEmpty()
                            select new StaffView
                            {
                                LatinName = s.LatinName,
                                NativeName = s.NativeName,
                                Id = s.Id,
                                CreatedAt = s.CreatedAt,
                                BloodType = s.BloodType,
                                HomeTown = s.HomeTown,
                                IsMale = s.IsMale,
                                OtherInformation = s.OtherInformation,
                                LanguageId = s.LanguageId,
                                DayOfBirth = s.DayOfBirth,
                                DayOfDeath = s.DayOfDeath,
                                MonthOfBirth = s.MonthOfBirth,
                                MonthOfDeath = s.MonthOfDeath,
                                OtherName = s.OtherName,
                                YearOfBirth = s.YearOfBirth,
                                YearOfDeath = s.YearOfDeath,
                                LanguageName = rr.LanguageName
                            };
                return await staff.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<Tuple<ResponseView, string?, string?>> UpdateStaff(StaffView staffView, CancellationToken cancellationToken)
        {
            try
            {
                var languageId = staffView.LanguageId;
                var language = await _context.Languages.Where(s => !s.IsDeleted && s.Id == languageId).FirstOrDefaultAsync(cancellationToken);
                if (language == null)
                {
                    return new(new(MessageConstant.NOT_FOUND_LANGUAGE, 3), null, null);
                }
                var staff = await _context.Staffs.Where(s => (s.Id == staffView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (staff == null)
                {
                    return new(new(MessageConstant.NOT_FOUND, 2), null, null);
                }
                var listStaffJob = await _context.StaffJobs.Where(s => (s.StaffId == staffView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                if (staffView.StaffJob != null && staffView.StaffJob.Count > 0)
                {
                    var listJobId = staffView.StaffJob.Select(x => x.JobId).Distinct().ToList();
                    var allJob = await _context.Jobs.Where(s => !s.IsDeleted && listJobId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listJobId.Count();
                    if (!allJob) {
                        return new(new(MessageConstant.NOT_FOUND_JOB, 4), null, null);
                    }
                    foreach (var item in listStaffJob)
                    {
                        var existed = staffView.StaffJob.Where(s => (s.Id == item.Id && s.StaffId == item.StaffId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.JobId = existed.JobId;
                            item.IsMain = existed.IsMain;
                            item.UpdatedAt = DateTime.UtcNow;
                            staffView.StaffJob.Remove(existed);
                        }
                        else
                        {
                            item.IsDeleted = true;
                            item.DeletedAt = DateTime.UtcNow;
                        }
                    }
                }
                else
                {
                    foreach (var item in listStaffJob)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                var latinName = staff.LatinName;
                _mapper.Map(staffView, staff);
                staff.UpdatedAt = DateTime.UtcNow;
                if (staff.StaffJob != null && staff.StaffJob.Count > 0)
                {
                    staff.StaffJob.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    staff.StaffJob = new();
                }
                staff.StaffJob.AddRange(listStaffJob);
                await _context.SaveChangesAsync(cancellationToken);
                var nativeName = staff.NativeName;
                return new(new(MessageConstant.UPDATE_SUCCESS, 0), latinName, nativeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), null, null);
            }
        }
    }
}
