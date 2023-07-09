using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface IProducerImageService
    {
        public Task<ResponseView> CreateProducerImage(ProducerImageView producerImageView, IFormFile file, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken);
        public Task<ProducerView> GetProducerById(int id, CancellationToken cancellationToken);
    }
    public class ProducerImageService : IProducerImageService
    {
        private readonly IProducerImageRepository _repo;
        private readonly IProducerRepository _producerRepo;

        public ProducerImageService(IProducerImageRepository repo, IProducerRepository producerRepo)
        {
            _repo = repo;
            _producerRepo = producerRepo;
        }

        public async Task<ResponseView> CreateProducerImage(ProducerImageView producerImageView, IFormFile file, CancellationToken cancellationToken)
        {
            var producer = await _producerRepo.GetProducerById(producerImageView.ProducerId ?? 0, cancellationToken);
            if (producer.Id > 0)
            {
                var producerName = (producer.ProducerName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
                var folderName = Path.Combine("Image", "Producer", producerName);
                var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
                Directory.CreateDirectory(pathToSave);
                var _res = await _repo.CreateProducerImage(producerImageView, cancellationToken);
                var res = _res.Item1;
                producerImageView.Id = _res.Item2;
                producerImageView.ProducerId = _res.Item3;
                if (res.ErrorCode != 0)
                {
                    return res;
                }
                var name = $"{producerName}-{producerImageView.Id}.{file.FileName.Split(".").Last()}";
                var fullPath = Path.Combine(pathToSave, name);
                producerImageView.Image = Path.Combine(folderName, name);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var res2 = await _repo.UpdateProducerImage(producerImageView, cancellationToken);
                if (res2.ErrorCode != 0)
                {
                    return res2;
                }
                return res;
            }
            return new(MessageConstant.NOT_FOUND, 2);
        }

        public async Task<ResponseView> DeleteProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteProducerImage(producerImageView, cancellationToken);
        }

        public async Task<ProducerView> GetProducerById(int id, CancellationToken cancellationToken)
        {
            return await _producerRepo.GetProducerById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateProducerImage(ProducerImageView producerImageView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateProducerImage(producerImageView, cancellationToken);
        }
    }
}
