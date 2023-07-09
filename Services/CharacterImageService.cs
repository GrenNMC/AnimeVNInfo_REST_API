using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface ICharacterImageService
    {
        public Task<ResponseView> CreateCharacterImage(CharacterImageView characterImageView, IFormFile file, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken);
        public Task<CharacterView> GetCharacterById(int id, CancellationToken cancellationToken);
    }
    public class CharacterImageService : ICharacterImageService
    {
        private readonly ICharacterImageRepository _repo;
        private readonly ICharacterRepository _characterRepo;

        public CharacterImageService(ICharacterImageRepository repo, ICharacterRepository characterRepo)
        {
            _repo = repo;
            _characterRepo = characterRepo;
        }

        public async Task<ResponseView> CreateCharacterImage(CharacterImageView characterImageView, IFormFile file, CancellationToken cancellationToken)
        {
            var character = await _characterRepo.GetCharacterById(characterImageView.CharacterId ?? 0, cancellationToken);
            if (character.Id > 0)
            {
                var romanjiName = (character.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
                var folderName = Path.Combine("Image", "Character", $"{romanjiName}-{character.Id}");
                var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
                Directory.CreateDirectory(pathToSave);
                var _res = await _repo.CreateCharacterImage(characterImageView, cancellationToken);
                var res = _res.Item1;
                characterImageView.Id = _res.Item2;
                characterImageView.CharacterId = _res.Item3;
                if (res.ErrorCode != 0)
                {
                    return res;
                }
                var name = $"{romanjiName}-{character.Id}-{characterImageView.Id}.{file.FileName.Split(".").Last()}";
                var fullPath = Path.Combine(pathToSave, name);
                characterImageView.Image = Path.Combine(folderName, name);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var res2 = await _repo.UpdateCharacterImage(characterImageView, cancellationToken);
                if (res2.ErrorCode != 0)
                {
                    return res2;
                }
                return res;
            }
            return new(MessageConstant.NOT_FOUND, 2);
        }

        public async Task<ResponseView> DeleteCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            return await _repo.DeleteCharacterImage(characterImageView, cancellationToken);
        }

        public async Task<CharacterView> GetCharacterById(int id, CancellationToken cancellationToken)
        {
            return await _characterRepo.GetCharacterById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateCharacterImage(CharacterImageView characterImageView, CancellationToken cancellationToken)
        {
            return await _repo.UpdateCharacterImage(characterImageView, cancellationToken);
        }
    }
}
