using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace AnimeVnInfoBackend.Repositories
{
    public interface ICharacterRepository
    {
        public Task<ResponseWithPaginationView> GetAllCharacter(CharacterParamView characterParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllCharacter(CharacterQueryListView characterQueryListView, CancellationToken cancellationToken);
        public Task<CharacterView> GetCharacterById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateCharacter(CharacterView characterView, CancellationToken cancellationToken);
        public Task<Tuple<ResponseView, string?, string?>> UpdateCharacter(CharacterView characterView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteCharacter(CharacterView characterView, CancellationToken cancellationToken);
    }

    public class CharacterRepository : ICharacterRepository
    {
        private readonly ILogger<CharacterRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CharacterRepository(ApplicationDbContext context, ILogger<CharacterRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateCharacter(CharacterView characterView, CancellationToken cancellationToken)
        {
            try
            {
                Character newCharacter = new();
                newCharacter = _mapper.Map<Character>(characterView);
                newCharacter.CreatedAt = DateTime.UtcNow;
                newCharacter.UpdatedAt = DateTime.UtcNow;
                if (newCharacter.CharacterStaff != null && newCharacter.CharacterStaff.Count > 0)
                {
                    var listStaffId = newCharacter.CharacterStaff.Select(x => x.StaffId).Distinct().ToList();
                    var allStaff = await _context.Staffs.Where(s => !s.IsDeleted && listStaffId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listStaffId.Count();
                    if (!allStaff) {
                        return new(MessageConstant.NOT_FOUND_STAFF, 3);
                    }
                    newCharacter.CharacterStaff.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                await _context.Characters.AddAsync(newCharacter, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteCharacter(CharacterView characterView, CancellationToken cancellationToken)
        {
            try
            {
                var character = await _context.Characters.Where(s => (s.Id == characterView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (character == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                character.IsDeleted = true;
                character.DeletedAt = DateTime.UtcNow;
                await _context.CharacterStaffs.Where(s => (s.CharacterId == characterView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                        .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeCharacters.Where(s => (s.CharacterId == characterView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                        .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllCharacter(CharacterParamView characterParamView, CancellationToken cancellationToken)
        {
            try
            {
                var characters = from s in _context.Characters
                                 orderby s.Id
                                 where !s.IsDeleted
                                 select new CharacterView
                                 {
                                     Id = s.Id,
                                     Age = s.Age,
                                     CreatedAt = s.CreatedAt,
                                     DayOfBirth = s.DayOfBirth,
                                     IsMale = s.IsMale,
                                     NativeName = s.NativeName,
                                     MonthOfBirth = s.MonthOfBirth,
                                     OtherInformation = s.OtherInformation,
                                     IdAnilist = s.IdAnilist,
                                     LatinName = s.LatinName,
                                     YearOfBirth = s.YearOfBirth,
                                     OtherName = s.OtherName,
                                     BloodType = s.BloodType,
                                     AnimeCount = (from sa in _context.AnimeCharacters
                                                   where sa.CharacterId == s.Id && !sa.IsDeleted
                                                   select sa).Count()
                                 };
                if (!await characters.AnyAsync(cancellationToken))
                {
                    return new CharacterListView(new List<CharacterView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(characterParamView.SearchString))
                {
                    characters = characters.Where(s => ((s.NativeName ?? "").ToLower().Contains(characterParamView.SearchString.ToLower()) ||
                                                        (s.LatinName ?? "").ToLower().Contains(characterParamView.SearchString.ToLower()) ||
                                                        (s.OtherName ?? "").ToLower().Contains(characterParamView.SearchString.ToLower()) ||
                                                        (s.OtherInformation ?? "").ToLower().Contains(characterParamView.SearchString.ToLower())))
                                            .Union(characters.Join(_context.CharacterStaffs
                                                                .Join(_context.Staffs.Where(s => (((s.NativeName ?? "").ToLower().Contains(characterParamView.SearchString) ||
                                                                                                    (s.LatinName ?? "").ToLower().Contains(characterParamView.SearchString) ||
                                                                                                    (s.OtherName ?? "").ToLower().Contains(characterParamView.SearchString)) &&
                                                                                                    !s.IsDeleted)),
                                                                                        s => s.StaffId, r => r.Id, (s, r) => s),
                                                                s => s.Id, r => r.CharacterId, (s, r) => s).Distinct()).OrderBy(s => s.Id);
                }
                if (!string.IsNullOrWhiteSpace(characterParamView.SearchStaff))
                {
                    characters = characters.Join(_context.CharacterStaffs
                                                        .Join(_context.Staffs.Where(s => (((s.NativeName ?? "").ToLower().Contains(characterParamView.SearchStaff) ||
                                                                                            (s.LatinName ?? "").ToLower().Contains(characterParamView.SearchStaff) ||
                                                                                            (s.OtherName ?? "").ToLower().Contains(characterParamView.SearchStaff)) &&
                                                                                            !s.IsDeleted)),
                                                                                s => s.StaffId, r => r.Id, (s, r) => s),
                                                        s => s.Id, r => r.CharacterId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                }
                if (!string.IsNullOrWhiteSpace(characterParamView.SearchName))
                {
                    characters = characters.Where(s => ((s.NativeName ?? "").ToLower().Contains(characterParamView.SearchName.ToLower()) ||
                                                        (s.LatinName ?? "").ToLower().Contains(characterParamView.SearchName.ToLower()) ||
                                                        (s.OtherName ?? "").ToLower().Contains(characterParamView.SearchName.ToLower())));
                }
                if (characterParamView.DayOfBirth != null && characterParamView.DayOfBirth > 0)
                {
                    characters = characters.Where(s => s.DayOfBirth == characterParamView.DayOfBirth);
                }
                if (characterParamView.MonthOfBirth != null && characterParamView.MonthOfBirth > 0)
                {
                    characters = characters.Where(s => s.MonthOfBirth == characterParamView.MonthOfBirth);
                }
                if (characterParamView.YearOfBirth != null && characterParamView.YearOfBirth > 0)
                {
                    characters = characters.Where(s => s.YearOfBirth == characterParamView.YearOfBirth);
                }
                if (characterParamView.Order == "asc")
                {
                    if (characterParamView.OrderBy == "latinName")
                    {
                        characters = characters.OrderBy(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (characterParamView.OrderBy == "nativeName")
                    {
                        characters = characters.OrderBy(s => s.NativeName).ThenBy(s => s.Id);
                    }
                    else if (characterParamView.OrderBy == "createdAt")
                    {
                        characters = characters.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (characterParamView.OrderBy == "animeCount")
                    {
                        characters = characters.OrderBy(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                else if (characterParamView.Order == "desc")
                {
                    if (characterParamView.OrderBy == "latinName")
                    {
                        characters = characters.OrderByDescending(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (characterParamView.OrderBy == "nativeName")
                    {
                        characters = characters.OrderByDescending(s => s.NativeName).ThenBy(s => s.Id);
                    }
                    else if (characterParamView.OrderBy == "createdAt")
                    {
                        characters = characters.OrderByDescending(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (characterParamView.OrderBy == "animeCount")
                    {
                        characters = characters.OrderByDescending(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await characters.CountAsync(cancellationToken);
                if (characterParamView.PageIndex > 0)
                {
                    characters = characters.Skip(characterParamView.PageSize * (characterParamView.PageIndex - 1)).Take(characterParamView.PageSize);
                }
                characters = characters.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                var list = await characters.ToListAsync(cancellationToken);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {

                            item.CharacterStaff = await (from sv in _context.CharacterStaffs
                                                         where !sv.IsDeleted && sv.CharacterId == item.Id
                                                         join a in (from at in _context.Staffs where !at.IsDeleted select at) on sv.StaffId equals a.Id
                                                         join l in (from lt in _context.Languages where !lt.IsDeleted select lt) on a.LanguageId equals l.Id into sl
                                                         from ll in sl.DefaultIfEmpty()
                                                         select new CharacterStaffView {
                                                             Id = sv.Id,
                                                             CharacterId = sv.CharacterId,
                                                             StaffId = sv.StaffId,
                                                             StaffIdAnilist = a.IdAnilist,
                                                             StaffNativeName = a.NativeName,
                                                             StaffLanguage = ll.LanguageName,
                                                             StaffLatinName = a.LatinName
                                                         }).ToListAsync(cancellationToken);
                            item.CharacterImage = await (from si in _context.CharacterImages
                                                         orderby si.UpdatedAt descending
                                                         where si.CharacterId == item.Id && !si.IsDeleted
                                                         select new CharacterImageView {
                                                             Id = si.Id,
                                                             CharacterId = si.CharacterId,
                                                             Image = si.Image,
                                                             IsAvatar = si.IsAvatar
                                                         }).ToListAsync(cancellationToken);
                        }
                    }
                }
                return new CharacterListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new CharacterListView(new List<CharacterView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllCharacter(CharacterQueryListView characterQueryListView, CancellationToken cancellationToken)
        {
            try
            {
                if (characterQueryListView.Id == null)
                {
                    return new CharacterListView(new(), 0, 2, MessageConstant.NO_DATA_REQUEST);
                }
                var characters = from s in _context.Characters
                                 orderby s.Id
                                 where !s.IsDeleted && characterQueryListView.Id.Contains(s.IdAnilist)
                                 select new CharacterView
                                 {
                                     Id = s.Id,
                                     Age = s.Age,
                                     CreatedAt = s.CreatedAt,
                                     DayOfBirth = s.DayOfBirth,
                                     IsMale = s.IsMale,
                                     NativeName = s.NativeName,
                                     MonthOfBirth = s.MonthOfBirth,
                                     OtherInformation = s.OtherInformation,
                                     IdAnilist = s.IdAnilist,
                                     LatinName = s.LatinName,
                                     YearOfBirth = s.YearOfBirth,
                                     OtherName = s.OtherName,
                                     BloodType = s.BloodType,
                                     AnimeCount = (from sa in _context.AnimeCharacters
                                                   where sa.CharacterId == s.Id && !sa.IsDeleted
                                                   select sa).Count(),
                                 };
                var totalRecord = await characters.CountAsync(cancellationToken);
                characters = characters.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                var list = await characters.ToListAsync(cancellationToken);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {

                            item.CharacterStaff = await (from sv in _context.CharacterStaffs
                                                         where !sv.IsDeleted && sv.CharacterId == item.Id
                                                         join a in (from at in _context.Staffs where !at.IsDeleted select at) on sv.StaffId equals a.Id
                                                         join l in (from lt in _context.Languages where !lt.IsDeleted select lt) on a.LanguageId equals l.Id into sl
                                                         from ll in sl.DefaultIfEmpty()
                                                         select new CharacterStaffView {
                                                             Id = sv.Id,
                                                             CharacterId = sv.CharacterId,
                                                             StaffId = sv.StaffId,
                                                             StaffIdAnilist = a.IdAnilist,
                                                             StaffNativeName = a.NativeName,
                                                             StaffLanguage = ll.LanguageName,
                                                             StaffLatinName = a.LatinName
                                                         }).ToListAsync(cancellationToken);
                            item.CharacterImage = await (from si in _context.CharacterImages
                                                         orderby si.UpdatedAt descending
                                                         where si.CharacterId == item.Id && !si.IsDeleted
                                                         select new CharacterImageView {
                                                             Id = si.Id,
                                                             CharacterId = si.CharacterId,
                                                             Image = si.Image,
                                                             IsAvatar = si.IsAvatar
                                                         }).ToListAsync(cancellationToken);
                        }
                    }
                }
                return new CharacterListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new CharacterListView(new List<CharacterView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<CharacterView> GetCharacterById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var character = from s in _context.Characters
                                where s.Id == id && !s.IsDeleted
                                select new CharacterView
                                {
                                    LatinName = s.LatinName,
                                    NativeName = s.NativeName,
                                    Id = s.Id,
                                    CreatedAt = s.CreatedAt,
                                    IsMale = s.IsMale,
                                    OtherInformation = s.OtherInformation,
                                    DayOfBirth = s.DayOfBirth,
                                    Age = s.Age,
                                    MonthOfBirth = s.MonthOfBirth,
                                    YearOfBirth = s.YearOfBirth,
                                    BloodType = s.BloodType,
                                    OtherName = s.OtherName
                                };
                return await character.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<Tuple<ResponseView, string?, string?>> UpdateCharacter(CharacterView characterView, CancellationToken cancellationToken)
        {
            try
            {
                var character = await _context.Characters.Where(s => (s.Id == characterView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (character == null)
                {
                    return new(new(MessageConstant.NOT_FOUND, 2), null, null);
                }
                var listCharacterStaff = await _context.CharacterStaffs.Where(s => (s.CharacterId == characterView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                if (characterView.CharacterStaff != null && characterView.CharacterStaff.Count > 0)
                {
                    var listStaffId = characterView.CharacterStaff.Select(x => x.StaffId).Distinct().ToList();
                    var allStaff = await _context.Staffs.Where(s => !s.IsDeleted && listStaffId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listStaffId.Count();
                    if (!allStaff) {
                        return new(new(MessageConstant.NOT_FOUND_STAFF, 3), null, null);
                    }
                    foreach (var item in listCharacterStaff)
                    {
                        var existed = characterView.CharacterStaff.Where(s => (s.Id == item.Id && s.CharacterId == item.CharacterId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.StaffId = existed.StaffId;
                            item.UpdatedAt = DateTime.UtcNow;
                            characterView.CharacterStaff.Remove(existed);
                        }
                        else
                        {
                            item.IsDeleted = true;
                            item.DeletedAt = DateTime.UtcNow;
                        }
                    }
                }
                else
                {
                    foreach (var item in listCharacterStaff)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                var latinName = character.LatinName;
                _mapper.Map(characterView, character);
                character.UpdatedAt = DateTime.UtcNow;
                if (character.CharacterStaff != null && character.CharacterStaff.Count > 0)
                {
                    character.CharacterStaff.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    character.CharacterStaff = new();
                }
                character.CharacterStaff.AddRange(listCharacterStaff);
                await _context.SaveChangesAsync(cancellationToken);
                var nativeName = character.NativeName;
                return new(new(MessageConstant.UPDATE_SUCCESS, 0), latinName, nativeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), null, null);
            }
        }
    }
}
