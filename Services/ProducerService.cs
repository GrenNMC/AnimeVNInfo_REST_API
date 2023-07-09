using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IProducerService
    {
        public Task<ResponseWithPaginationView> GetAllProducer(ProducerParamView producerParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllProducer(ProducerQueryListView producerQueryListView, CancellationToken cancellationToken);
        public Task<ProducerView> GetProducerById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateProducer(ProducerView producerView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateProducer(ProducerView producerView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteProducer(ProducerView producerView, CancellationToken cancellationToken);
    }

    public class ProducerService : IProducerService
    {
        private readonly IProducerRepository _repo;
        private readonly IProducerImageRepository _piRepo;

        public ProducerService(IProducerRepository repo, IProducerImageRepository piRepo)
        {
            _repo = repo;
            _piRepo = piRepo;
        }

        public async Task<ResponseView> CreateProducer(ProducerView producerView, CancellationToken cancellationToken)
        {
            return await _repo.CreateProducer(producerView, cancellationToken);
        }

        public async Task<ResponseView> DeleteProducer(ProducerView producerView, CancellationToken cancellationToken)
        {
            var res = await _piRepo.DeleteAllProducerImage(producerView.Id, cancellationToken);
            if (res.ErrorCode != 0)
            {
                return res;
            }
            res = await _repo.DeleteProducer(producerView, cancellationToken);
            return res;
        }

        public async Task<ResponseWithPaginationView> GetAllProducer(ProducerParamView producerParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllProducer(producerParamView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllProducer(ProducerQueryListView producerQueryListView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllProducer(producerQueryListView, cancellationToken);
        }

        public async Task<ProducerView> GetProducerById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetProducerById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateProducer(ProducerView producerView, CancellationToken cancellationToken)
        {
            var newName = (producerView.ProducerName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var _res = await _repo.UpdateProducer(producerView, cancellationToken);
            var res = _res.Item1;
            producerView.ProducerName = _res.Item2;
            if (res.ErrorCode != 0)
            {
                return res;
            }
            var oldName = (producerView.ProducerName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var folderName = Path.Combine("Image", "Producer");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, oldName);
            if (Directory.Exists(pathToSave) && newName != oldName)
            {
                var newPath = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, newName);
                Directory.Move(pathToSave, newPath);
                var files = Directory.GetFiles(newPath);
                foreach (var file in files)
                {
                    var name = $"{newName}-{file.Split("-").Last()}";
                    File.Move(file, $"{file.Substring(0, file.LastIndexOf("\\"))}\\{name}", true);
                }
                res = await _piRepo.UpdateAllNameProducerImage(producerView.Id, newName, cancellationToken);
            }
            return res;
        }
    }
}
