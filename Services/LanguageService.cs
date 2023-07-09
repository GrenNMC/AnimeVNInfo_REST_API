using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface ILanguageService
    {
        public Task<ResponseWithPaginationView> GetAllLanguage(LanguageParamView LanguageParamView, CancellationToken cancellationToken);
        public Task<LanguageView> GetLanguageById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateLanguage(LanguageView LanguageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateLanguage(LanguageView LanguageView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteLanguage(LanguageView LanguageView, CancellationToken cancellationToken);
    }

    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _repo;

        public LanguageService(ILanguageRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> CreateLanguage(LanguageView LanguageView, CancellationToken cancellationToken)
        {
            return await _repo.CreateLanguage(LanguageView, cancellationToken);
        }

        public async Task<ResponseView> DeleteLanguage(LanguageView LanguageView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteLanguage(LanguageView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllLanguage(LanguageParamView LanguageParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllLanguage(LanguageParamView, cancellationToken);
        }

        public async Task<LanguageView> GetLanguageById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetLanguageById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateLanguage(LanguageView LanguageView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateLanguage(LanguageView, cancellationToken);
        }
    }
}
