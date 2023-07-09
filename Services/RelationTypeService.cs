using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;

namespace AnimeVnInfoBackend.Services
{
    public interface IRelationTypeService
    {
        public Task<ResponseWithPaginationView> GetAllRelationType(RelationTypeParamView relationTypeParamView, CancellationToken cancellationToken);
        public Task<RelationTypeView> GetRelationTypeById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangeNextOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> ChangePrevOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken);
    }

    public class RelationTypeService : IRelationTypeService
    {
        private readonly IRelationTypeRepository _repo;

        public RelationTypeService(IRelationTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResponseView> ChangeNextOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            return await _repo.ChangeNextOrder(relationTypeView, cancellationToken);
        }

        public async Task<ResponseView> ChangePrevOrder(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            return await _repo.ChangePrevOrder(relationTypeView, cancellationToken);
        }

        public async Task<ResponseView> CreateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            return await _repo.CreateRelationType(relationTypeView, cancellationToken);
        }

        public async Task<ResponseView> DeleteRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteRelationType(relationTypeView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllRelationType(RelationTypeParamView relationTypeParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllRelationType(relationTypeParamView, cancellationToken);
        }

        public async Task<RelationTypeView> GetRelationTypeById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetRelationTypeById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateRelationType(RelationTypeView relationTypeView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateRelationType(relationTypeView, cancellationToken);
        }
    }
}
