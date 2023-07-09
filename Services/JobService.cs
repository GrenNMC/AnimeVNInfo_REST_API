using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IJobService
    {
        public Task<ResponseWithPaginationView> GetAllJob(JobParamView JobParamView, CancellationToken cancellationToken);
        public Task<JobView> GetJobById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateJob(JobView JobView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateJob(JobView JobView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteJob(JobView JobView, CancellationToken cancellationToken);
    }

    public class JobService : IJobService
    {
        private readonly IJobRepository _repo;

        public JobService(IJobRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> CreateJob(JobView JobView, CancellationToken cancellationToken)
        {
            return await _repo.CreateJob(JobView, cancellationToken);
        }

        public async Task<ResponseView> DeleteJob(JobView JobView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteJob(JobView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllJob(JobParamView JobParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllJob(JobParamView, cancellationToken);
        }

        public async Task<JobView> GetJobById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetJobById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateJob(JobView JobView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateJob(JobView, cancellationToken);
        }
    }
}
