using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Utilities.Constants;
using AnimeVnInfoBackend.Utilities.Helpers;

namespace AnimeVnInfoBackend.Services
{
    public interface ICharacterService
    {
        public Task<ResponseWithPaginationView> GetAllCharacter(CharacterParamView characterParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllCharacter(CharacterQueryListView characterQueryListView, CancellationToken cancellationToken);
        public Task<CharacterView> GetCharacterById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateCharacter(CharacterView characterView, CancellationToken cancellationToken);
        public Task<ResponseView> UpdateCharacter(CharacterView characterView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteCharacter(CharacterView characterView, CancellationToken cancellationToken);
    }

    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _repo;
        private readonly ICharacterImageRepository _siRepo;

        public CharacterService(ICharacterRepository repo, ICharacterImageRepository siRepo)
        {
            _repo = repo;
            _siRepo = siRepo;
        }

        public async Task<ResponseView> CreateCharacter(CharacterView characterView, CancellationToken cancellationToken)
        {
            return await _repo.CreateCharacter(characterView, cancellationToken);
        }

        public async Task<ResponseView> DeleteCharacter(CharacterView characterView, CancellationToken cancellationToken)
        {
            var res = await _siRepo.DeleteAllCharacterImage(characterView.Id, cancellationToken);
            if (res.ErrorCode != 0)
            {
                return res;
            }
            res = await _repo.DeleteCharacter(characterView, cancellationToken);
            return res;
        }

        public async Task<ResponseWithPaginationView> GetAllCharacter(CharacterParamView characterParamView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllCharacter(characterParamView, cancellationToken);
        }

        public async Task<ResponseWithPaginationView> GetAllCharacter(CharacterQueryListView characterQueryListView, CancellationToken cancellationToken)
        {
            return await _repo.GetAllCharacter(characterQueryListView, cancellationToken);
        }

        public async Task<CharacterView> GetCharacterById(int id, CancellationToken cancellationToken)
        {
            return await _repo.GetCharacterById(id, cancellationToken);
        }

        public async Task<ResponseView> UpdateCharacter(CharacterView characterView, CancellationToken cancellationToken)
        {
            var newName = (characterView.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var id = characterView.Id;
            var _res = await _repo.UpdateCharacter(characterView, cancellationToken);
            var res = _res.Item1;
            characterView.LatinName = _res.Item2;
            characterView.NativeName = _res.Item3;
            if (res.ErrorCode != 0)
            {
                return res;
            }
            var oldName = (characterView.LatinName ?? "").RemoveNonAlphanumeric(' ').RemoveLongSpace(true).Replace(" ", "-");
            var folderName = Path.Combine("Image", "Character");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, $"{oldName}-{id}");
            if (Directory.Exists(pathToSave) && newName != oldName)
            {
                var newPath = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName, $"{newName}-{id}");
                Directory.Move(pathToSave, newPath);
                var files = Directory.GetFiles(newPath);
                foreach (var file in files)
                {
                    var name = $"{newName}-{id}-{file.Split("-").Last()}";
                    File.Move(file, $"{file.Substring(0, file.LastIndexOf("\\"))}\\{name}", true);
                }
                res = await _siRepo.UpdateAllNameCharacterImage(id, newName, cancellationToken);
            }
            return res;
        }
    }
}
