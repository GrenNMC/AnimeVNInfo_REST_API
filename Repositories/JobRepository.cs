using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IJobRepository
    {
        public Task<ResponseWithPaginationView> GetAllJob(JobParamView JobParamView, CancellationToken cancellationToken);
        public Task<JobView> GetJobById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateJob(JobView JobView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateJob(JobView JobView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteJob(JobView JobView, CancellationToken cancellationToken);
    }

    public class JobRepository : IJobRepository
    {
        private readonly ILogger<JobRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public JobRepository(ILogger<JobRepository> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateJob(JobView JobView, CancellationToken cancellationToken)
        {
            try
            {
                Job newJob = new();
                newJob = _mapper.Map<Job>(JobView);
                newJob.CreatedAt = DateTime.UtcNow;
                newJob.UpdatedAt = DateTime.UtcNow;
                var Job = await _context.Jobs.Where(s => (s.JobName == JobView.JobName && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (Job != null)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                await _context.Jobs.AddAsync(newJob, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteJob(JobView JobView, CancellationToken cancellationToken)
        {
            try
            {
                var Job = await _context.Jobs.Where(s => (s.Id == JobView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (Job == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                await _context.StaffJobs.Where(s => (s.JobId == Job.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                Job.IsDeleted = true;
                Job.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllJob(JobParamView JobParamView, CancellationToken cancellationToken)
        {
            try
            {
                var Jobs = from s in _context.Jobs
                           orderby s.Id
                           where !s.IsDeleted
                           select new JobView
                           {
                               Id = s.Id,
                               JobName = s.JobName,
                               Description = s.Description,
                               StaffCount = (from ss in _context.StaffJobs
                                             where ss.JobId == s.Id && !ss.IsDeleted
                                             select ss).Count()
                           };
                if (!await Jobs.AnyAsync(cancellationToken))
                {
                    return new JobListView(new List<JobView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(JobParamView.SearchString))
                {
                    Jobs = Jobs.Where(s => ((s.JobName ?? "").ToLower().Contains(JobParamView.SearchString.ToLower()) ||
                                                (s.Description ?? "").ToLower().Contains(JobParamView.SearchString.ToLower())));
                }
                if (JobParamView.Order == "asc")
                {
                    if (JobParamView.OrderBy == "jobName")
                    {
                        Jobs = Jobs.OrderBy(s => s.JobName).ThenBy(s => s.Id);
                    }
                    else if (JobParamView.OrderBy == "staffCount")
                    {
                        Jobs = Jobs.OrderBy(s => s.StaffCount).ThenBy(s => s.Id);
                    }
                }
                else if (JobParamView.Order == "desc")
                {
                    if (JobParamView.OrderBy == "jobName")
                    {
                        Jobs = Jobs.OrderByDescending(s => s.JobName).ThenBy(s => s.Id);
                    }
                    else if (JobParamView.OrderBy == "staffCount")
                    {
                        Jobs = Jobs.OrderByDescending(s => s.StaffCount).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await Jobs.CountAsync(cancellationToken);
                if (JobParamView.PageIndex > 0)
                {
                    Jobs = Jobs.Skip(JobParamView.PageSize * (JobParamView.PageIndex - 1)).Take(JobParamView.PageSize);
                }
                return new JobListView(await Jobs.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new JobListView(new List<JobView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<JobView> GetJobById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var Job = from s in _context.Jobs
                          where s.Id == id && !s.IsDeleted
                          select new JobView
                          {
                              Id = s.Id,
                              Description = s.Description,
                              JobName = s.JobName
                          };
                return await Job.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<ResponseView> UpdateJob(JobView JobView, CancellationToken cancellationToken)
        {
            try
            {
                var job = await _context.Jobs.Where(s => (s.Id == JobView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (job == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                var checkExisted = await _context.Jobs.Where(s => (s.JobName == JobView.JobName && s.JobName != job.JobName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 3);
                }
                _mapper.Map(JobView, job);
                job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.UPDATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }
    }
}
