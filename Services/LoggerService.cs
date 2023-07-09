using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface ILoggerService
    {
        public Task<ResponseWithPaginationView> GetAllLog(LoggerParamView loggerParamView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateLog(LoggerView loggerView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateLog(string content, CancellationToken cancellationToken);
        public Task<ResponseView> CreateMultipleLog(List<LoggerView> loggerView, CancellationToken cancellationToken);
        public Task<ResponseView> CreateMultipleLog(List<string> content, CancellationToken cancellationToken);
    }

    public class LoggerService : ILoggerService
    {
        private readonly ILoggerRepository _repo;

        public LoggerService(ILoggerRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseWithPaginationView> GetAllLog(LoggerParamView loggerParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllLog(loggerParamView, cancellationToken);
        }

        public async Task<ResponseView> CreateLog(LoggerView loggerView, CancellationToken cancellationToken)
        {
            return await _repo.CreateLog(loggerView, cancellationToken);
        }

        public async Task<ResponseView> CreateLog(string content, CancellationToken cancellationToken)
        {
            LoggerView loggerView = new()
            {
                Content = content
            };
            return await CreateLog(loggerView, cancellationToken);
        }

        public async Task<ResponseView> CreateMultipleLog(List<string> content, CancellationToken cancellationToken) {
            List<LoggerView> loggerView = new();
            foreach(var item in content) {
                loggerView.Add(new() {
                    Content = item
                });
            }
            return await CreateMultipleLog(loggerView, cancellationToken);
        }

        public async Task<ResponseView> CreateMultipleLog(List<LoggerView> loggerView, CancellationToken cancellationToken) {
            return await _repo.CreateMultipleLog(loggerView, cancellationToken);
        }
    }
}