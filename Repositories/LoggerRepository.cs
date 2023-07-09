using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface ILoggerRepository
    {
        public Task<ResponseWithPaginationView> GetAllLog(LoggerParamView loggerParamView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateLog(LoggerView loggerView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateMultipleLog(List<LoggerView> loggerView, CancellationToken cancellationToken);
    }

    public class LoggerRepository : ILoggerRepository
    {
        private readonly ILogger<LoggerRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LoggerRepository(ApplicationDbContext context, ILogger<LoggerRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateLog(LoggerView loggerView, CancellationToken cancellationToken)
        {
            try
            {
                Logger newLog = new();
                newLog = _mapper.Map<Logger>(loggerView);
                newLog.CreatedAt = DateTime.UtcNow;
                newLog.UpdatedAt = DateTime.UtcNow;
                await _context.Loggers.AddAsync(newLog, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> CreateMultipleLog(List<LoggerView> loggerView, CancellationToken cancellationToken) {
            try {
                List<Logger> newLogs = new();
                foreach(var logger in loggerView) {
                    Logger newLog = new();
                    newLog = _mapper.Map<Logger>(logger);
                    newLog.CreatedAt = DateTime.UtcNow;
                    newLog.UpdatedAt = DateTime.UtcNow;
                    newLogs.Add(newLog);
                }
                await _context.Loggers.AddRangeAsync(newLogs, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllLog(LoggerParamView loggerParamView, CancellationToken cancellationToken)
        {
            try
            {
                var logs = from s in _context.Loggers
                           orderby s.CreatedAt descending
                           where !s.IsDeleted
                           select new LoggerView
                           {
                               CreatedAt = s.CreatedAt,
                               Id = s.Id,
                               Content = s.Content
                           };
                if (!await logs.AnyAsync(cancellationToken))
                {
                    return new LoggerListView(new List<LoggerView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(loggerParamView.SearchString))
                {
                    logs = logs.Where(s => (s.Content ?? "").ToLower().Contains(loggerParamView.SearchString.ToLower()));
                }
                var totalRecord = await logs.CountAsync(cancellationToken);
                if (loggerParamView.PageIndex > 0)
                {
                    logs = logs.Skip(loggerParamView.PageSize * (loggerParamView.PageIndex - 1)).Take(loggerParamView.PageSize);
                }
                logs = logs.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
				return new LoggerListView(await logs.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new LoggerListView(new List<LoggerView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }
    }
}
