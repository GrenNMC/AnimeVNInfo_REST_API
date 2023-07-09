using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IAnimeRepository
    {
        public Task<ResponseWithPaginationView> GetAllAnime(AnimeParamView animeParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAll(AnimeParamView animeParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllAnime(AnimeQueryListView animeQueryListView, CancellationToken cancellationToken);
        public Task<AnimeView> GetAnimeById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateAnime(AnimeView animeView, CancellationToken cancellationToken);
        public Task<Tuple<ResponseView, string?, string?>> UpdateAnime(AnimeView animeView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteAnime(AnimeView animeView, CancellationToken cancellationToken);
    }

    public class AnimeRepository : IAnimeRepository
    {
        private readonly ILogger<AnimeRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AnimeRepository(ApplicationDbContext context, ILogger<AnimeRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateAnime(AnimeView animeView, CancellationToken cancellationToken)
        {
            try
            {
                Anime newAnime = new();
                newAnime = _mapper.Map<Anime>(animeView);
                newAnime.CreatedAt = DateTime.UtcNow;
                newAnime.UpdatedAt = DateTime.UtcNow;
                if (newAnime.AnimeCharacter != null && newAnime.AnimeCharacter.Count > 0)
                {
                    var listCharacterId = newAnime.AnimeCharacter.Select(x => x.CharacterId).Distinct().ToList();
                    var allCharacter = await _context.Characters.Where(s => !s.IsDeleted && listCharacterId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listCharacterId.Count();
                    if (!allCharacter)
                    {
                        return new(MessageConstant.NOT_FOUND_CHARACTER, 3);
                    }
                    newAnime.AnimeCharacter.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                if (newAnime.AnimeGenre != null && newAnime.AnimeGenre.Count > 0)
                {
                    var listGenreId = newAnime.AnimeGenre.Select(x => x.GenreId).Distinct().ToList();
                    var allGenre = await _context.Genres.Where(s => !s.IsDeleted && listGenreId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listGenreId.Count();
                    if (!allGenre)
                    {
                        return new(MessageConstant.NOT_FOUND_GENRE, 3);
                    }
                    newAnime.AnimeGenre.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                if (newAnime.AnimeProducer != null && newAnime.AnimeProducer.Count > 0)
                {
                    var listProducerId = newAnime.AnimeProducer.Select(x => x.ProducerId).Distinct().ToList();
                    var allProducer = await _context.Producers.Where(s => !s.IsDeleted && listProducerId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listProducerId.Count();
                    if (!allProducer)
                    {
                        return new(MessageConstant.NOT_FOUND_PRODUCER, 3);
                    }
                    newAnime.AnimeProducer.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                if (newAnime.AnimeStudio != null && newAnime.AnimeStudio.Count > 0)
                {
                    var listStudioId = newAnime.AnimeStudio.Select(x => x.StudioId).Distinct().ToList();
                    var allStudio = await _context.Studios.Where(s => !s.IsDeleted && listStudioId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listStudioId.Count();
                    if (!allStudio)
                    {
                        return new(MessageConstant.NOT_FOUND_STUDIO, 3);
                    }
                    newAnime.AnimeStudio.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                if (newAnime.ChildAnimeRelation != null && newAnime.ChildAnimeRelation.Count > 0)
                {
                    var listParentId = newAnime.ChildAnimeRelation.Select(x => x.ParentId).Distinct().ToList();
                    var allParent = await _context.Animes.Where(s => !s.IsDeleted && listParentId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listParentId.Count();
                    if (!allParent)
                    {
                        return new(MessageConstant.NOT_FOUND_ANIME, 3);
                    }
                    newAnime.ChildAnimeRelation.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                if (newAnime.ParentAnimeRelation != null && newAnime.ParentAnimeRelation.Count > 0)
                {
                    var listChildId = newAnime.ParentAnimeRelation.Select(x => x.ChildId).Distinct().ToList();
                    var allChild = await _context.Animes.Where(s => !s.IsDeleted && listChildId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listChildId.Count();
                    if (!allChild)
                    {
                        return new(MessageConstant.NOT_FOUND_ANIME, 3);
                    }
                    newAnime.ParentAnimeRelation.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                if (newAnime.AnimeStaff != null && newAnime.AnimeStaff.Count > 0)
                {
                    var listStaffId = newAnime.AnimeStaff.Select(x => x.StaffId).Distinct().ToList();
                    var allStaff = await _context.Staffs.Where(s => !s.IsDeleted && listStaffId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listStaffId.Count();
                    if (!allStaff)
                    {
                        return new(MessageConstant.NOT_FOUND_STAFF, 3);
                    }
                    newAnime.AnimeStaff.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                if (newAnime.AnimeVideo != null && newAnime.AnimeVideo.Count > 0)
                {
                    newAnime.AnimeVideo.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                await _context.Animes.AddAsync(newAnime, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteAnime(AnimeView animeView, CancellationToken cancellationToken)
        {
            try
            {
                var anime = await _context.Animes.Where(s => (s.Id == animeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (anime == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                anime.IsDeleted = true;
                anime.DeletedAt = DateTime.UtcNow;
                await _context.AnimeCharacters.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeProducers.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeStudios.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeGenres.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                            .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeRelations.Where(s => ((s.ParentId == animeView.Id || s.ChildId == animeView.Id) && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeStaffs.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                            .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                await _context.AnimeVideos.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
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

        public async Task<ResponseWithPaginationView> GetAll(AnimeParamView animeParamView, CancellationToken cancellationToken)
        {
            try
            {
                var animes = from s in _context.Animes
                             orderby s.Id
                             where !s.IsDeleted
							 join r in (from rr in _context.AnimeTypes where !rr.IsDeleted select rr) on s.AnimeTypeId equals r.Id into rt
							 from st in rt.DefaultIfEmpty()
							 join r in (from rr in _context.Seasons where !rr.IsDeleted select rr) on s.SeasonId equals r.Id into rse
                             from sse in rse.DefaultIfEmpty()
                             select new HomeAnimeView
                             {
                                 Id = s.Id,
                                 NativeName = s.NativeName,
                                 LatinName = s.LatinName,
                                 Summary = s.Summary,
                                 TypeName = st.TypeName,
                                 AnimeTypeId = s.AnimeTypeId,
                                 SeasonId = s.SeasonId,
                                 DayOfStartTime = s.DayOfStartTime,
                                 MonthOfStartTime = s.MonthOfStartTime,
                                 YearOfStartTime = s.YearOfStartTime,
                                 HoursDuration = s.HoursDuration,
                                 MinutesDuration = s.MinutesDuration,
                                 SecondsDuration = s.SecondsDuration,
                                 Score = s.Score,
                                 Episodes = s.Episodes,
                                 Year = sse.Year,
                                 IsAdult = s.IsAdult,
                                 Quarter = sse.Quarter,
                             };
                if (!await animes.AnyAsync(cancellationToken))
                {
                    return new HomeAnimeListView(new List<HomeAnimeView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(animeParamView.SearchName))
                {
                    animes = animes.Where(s => ((s.NativeName ?? "").ToLower().Contains(animeParamView.SearchName.ToLower()) ||
                                                (s.LatinName ?? "").ToLower().Contains(animeParamView.SearchName.ToLower())));
                }
                if (animeParamView.AnimeTypeId != null)
                {
                    animes = animes.Where(s => s.AnimeTypeId == animeParamView.AnimeTypeId);
                }

                if (animeParamView.GenreId != null && animeParamView.GenreId.Count > 0)
                {
                    animes = animes.Join(_context.AnimeGenres.Where(s => (animeParamView.GenreId.Contains(s.GenreId ?? 0)) &&
                                                                        !s.IsDeleted), s => s.Id, r => r.AnimeId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                }
                if (animeParamView.Quarter != null)
                {
					animes = animes.Where(s => s.Quarter == animeParamView.Quarter);
				}
				if (animeParamView.Year != null)
				{
					animes = animes.Where(s => s.Year == animeParamView.Year);
				}

				if (animeParamView.Order == "asc")
                {
                    if (animeParamView.OrderBy == "latinName")
                    {
                        animes = animes.OrderBy(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "nativeName")
                    {
                        animes = animes.OrderBy(s => s.NativeName).ThenBy(s => s.Id);
                    }
                }
                else if (animeParamView.Order == "desc")
                {
                    if (animeParamView.OrderBy == "latinName")
                    {
                        animes = animes.OrderByDescending(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "nativeName")
                    {
                        animes = animes.OrderByDescending(s => s.NativeName).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await animes.CountAsync(cancellationToken);
                if (animeParamView.PageIndex > 0)
                {
                    animes = animes.Skip(animeParamView.PageSize * (animeParamView.PageIndex - 1)).Take(animeParamView.PageSize);
                }
                animes = animes.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                var list = await animes.ToListAsync(cancellationToken);
                if(list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            item.AnimeGenre = await (from sg in _context.AnimeGenres
                                                     where !sg.IsDeleted && sg.AnimeId == item.Id
                                                     join r in (from rr in _context.Genres where !rr.IsDeleted select rr) on sg.GenreId equals r.Id
                                                     select new AnimeGenreView
                                                     {
                                                         Id = sg.Id,
                                                         AnimeId = sg.AnimeId,
                                                         GenreId = sg.GenreId,
                                                         GenreName = r.GenreName,
                                                     }).ToListAsync(cancellationToken);

                            item.AnimeImage = await (from si in _context.AnimeImages
                                                     orderby si.UpdatedAt descending
                                                     where si.AnimeId == item.Id && !si.IsDeleted
                                                     select new AnimeImageView
                                                     {
                                                         Id = si.Id,
                                                         AnimeId = si.AnimeId,
                                                         Image = si.Image,
                                                         IsAvatar = si.IsAvatar
                                                     }).ToListAsync(cancellationToken);
                            item.AnimeStudio = await (from st in _context.AnimeStudios
													  where !st.IsDeleted && st.AnimeId == item.Id
													  join r in (from rr in _context.Studios where !rr.IsDeleted select rr) on st.StudioId equals r.Id
													  select new AnimeStudioView
                                                      {
                                                          Id = st.Id,
                                                          StudioName = r.StudioName
                                                      }).ToListAsync(cancellationToken);
						}
                    }
                }
				return new HomeAnimeListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return new HomeAnimeListView(new List<HomeAnimeView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllAnime(AnimeParamView animeParamView, CancellationToken cancellationToken)
        {
            try
            {
                var animes = from s in _context.Animes
                             orderby s.Id
                             where !s.IsDeleted
                             join r in (from rr in _context.AnimeTypes where !rr.IsDeleted select rr) on s.AnimeTypeId equals r.Id into rt
                             from st in rt.DefaultIfEmpty()
                             join r in (from rr in _context.AnimeStatuss where !rr.IsDeleted select rr) on s.AnimeStatusId equals r.Id into rss
                             from sss in rss.DefaultIfEmpty()
                             join r in (from rr in _context.AnimeSources where !rr.IsDeleted select rr) on s.AnimeSourceId equals r.Id into rsr
                             from ssr in rsr.DefaultIfEmpty()
                             join r in (from rr in _context.Ratings where !rr.IsDeleted select rr) on s.RatingId equals r.Id into rr
                             from sr in rr.DefaultIfEmpty()
                             join r in (from rr in _context.Seasons where !rr.IsDeleted select rr) on s.SeasonId equals r.Id into rse
                             from sse in rse.DefaultIfEmpty()
                             select new AnimeView
                             {
                                 Id = s.Id,
                                 CreatedAt = s.CreatedAt,
                                 NativeName = s.NativeName,
                                 LatinName = s.LatinName,
                                 OtherName = s.OtherName,
                                 AnimeSourceId = s.AnimeSourceId,
                                 AnimeTypeId = s.AnimeTypeId,
                                 AnimeStatusId = s.AnimeStatusId,
                                 RatingId = s.RatingId,
                                 SeasonId = s.SeasonId,
                                 LanguageId = s.LanguageId,
                                 DayOfEndTime = s.DayOfEndTime,
                                 DayOfStartTime = s.DayOfStartTime,
                                 MonthOfEndTime = s.MonthOfEndTime,
                                 MonthOfStartTime = s.MonthOfStartTime,
                                 YearOfEndTime = s.YearOfEndTime,
                                 YearOfStartTime = s.YearOfStartTime,
                                 HoursDuration = s.HoursDuration,
                                 MinutesDuration = s.MinutesDuration,
                                 SecondsDuration = s.SecondsDuration,
                                 Score = s.Score,
                                 Summary = s.Summary,
                                 Episodes = s.Episodes,
                                 TypeName = st.TypeName,
                                 StatusName = sss.StatusName,
                                 SourceName = ssr.SourceName,
                                 RatingName = sr.RatingName,
                                 Year = sse.Year,
                                 EndingSong = s.EndingSong,
                                 OpeningSong = s.OpeningSong,
                                 IsAdult = s.IsAdult,
                                 IdAnilist = s.IdAnilist,
                                 Quarter = sse.Quarter,
                             };
                if (!await animes.AnyAsync(cancellationToken))
                {
                    return new AnimeListView(new List<AnimeView>(), 0, 2, MessageConstant.NOT_FOUND);
                }

                if (!string.IsNullOrWhiteSpace(animeParamView.SearchString))
                {
                    animes = animes.Where(s => ((s.NativeName ?? "").ToLower().Contains(animeParamView.SearchString.ToLower()) ||
                                                (s.LatinName ?? "").ToLower().Contains(animeParamView.SearchString.ToLower()) ||
                                                (s.OtherName ?? "").ToLower().Contains(animeParamView.SearchString.ToLower()) ||
                                                (s.Summary ?? "").ToLower().Contains(animeParamView.SearchString.ToLower())))
                                    .Union(animes.Join(_context.AnimeCharacters
                                                                .Join(_context.Characters.Where(s => (((s.NativeName ?? "").ToLower().Contains(animeParamView.SearchString) ||
                                                                                                        (s.LatinName ?? "").ToLower().Contains(animeParamView.SearchString) ||
                                                                                                        (s.OtherName ?? "").ToLower().Contains(animeParamView.SearchString)) &&
                                                                                                        !s.IsDeleted)),
                                                                                        s => s.CharacterId, r => r.Id, (s, r) => s),
                                                                s => s.Id, r => r.AnimeId, (s, r) => s).Distinct())
                                    .Union(animes.Join(_context.AnimeStaffs
                                                                .Join(_context.Staffs.Where(s => (((s.NativeName ?? "").ToLower().Contains(animeParamView.SearchString) ||
                                                                                                    (s.LatinName ?? "").ToLower().Contains(animeParamView.SearchString) ||
                                                                                                    (s.OtherName ?? "").ToLower().Contains(animeParamView.SearchString)) &&
                                                                                                    !s.IsDeleted)),
                                                                                        s => s.StaffId, r => r.Id, (s, r) => s),
                                                                s => s.Id, r => r.AnimeId, (s, r) => s).Distinct());
                }
                if (!string.IsNullOrWhiteSpace(animeParamView.SearchStaff))
                {
                    animes = animes.Join(_context.AnimeStaffs
                                                .Join(_context.Staffs.Where(s => (((s.NativeName ?? "").ToLower().Contains(animeParamView.SearchStaff) ||
                                                                                    (s.LatinName ?? "").ToLower().Contains(animeParamView.SearchStaff) ||
                                                                                    (s.OtherName ?? "").ToLower().Contains(animeParamView.SearchStaff)) &&
                                                                                    !s.IsDeleted)),
                                                                        s => s.StaffId, r => r.Id, (s, r) => s),
                                                s => s.Id, r => r.AnimeId, (s, r) => s).Distinct();
                }
                if (!string.IsNullOrWhiteSpace(animeParamView.SearchCharacter))
                {
                    animes = animes.Join(_context.AnimeCharacters
                                                .Join(_context.Characters.Where(s => (((s.NativeName ?? "").ToLower().Contains(animeParamView.SearchCharacter) ||
                                                                                        (s.LatinName ?? "").ToLower().Contains(animeParamView.SearchCharacter) ||
                                                                                        (s.OtherName ?? "").ToLower().Contains(animeParamView.SearchCharacter)) &&
                                                                                        !s.IsDeleted)),
                                                                        s => s.CharacterId, r => r.Id, (s, r) => s),
                                                s => s.Id, r => r.AnimeId, (s, r) => s).Distinct();
                }
                if (!string.IsNullOrWhiteSpace(animeParamView.SearchName))
                {
                    animes = animes.Where(s => ((s.NativeName ?? "").ToLower().Contains(animeParamView.SearchName.ToLower()) ||
                                                (s.LatinName ?? "").ToLower().Contains(animeParamView.SearchName.ToLower()) ||
                                                (s.OtherName ?? "").ToLower().Contains(animeParamView.SearchName.ToLower())));
                }
                if (animeParamView.AnimeTypeId != null)
                {
                    animes = animes.Where(s => s.AnimeTypeId == animeParamView.AnimeTypeId);
                }
                if (animeParamView.AnimeSourceId != null)
                {
                    animes = animes.Where(s => s.AnimeSourceId == animeParamView.AnimeSourceId);
                }
                if (animeParamView.AnimeStatusId != null)
                {
                    animes = animes.Where(s => s.AnimeStatusId == animeParamView.AnimeStatusId);
                }
                if (animeParamView.RatingId != null)
                {
                    animes = animes.Where(s => s.RatingId == animeParamView.RatingId);
                }
                if(animeParamView.StudioId != null && animeParamView.StudioId.Count > 0)
                {
                    if (animeParamView.IsStudioGroup == true)
                    {
                        var _ag = _context.AnimeStudios.Where(s => (animeParamView.StudioId.Contains(s.StudioId ?? 0)) && !s.IsDeleted);
                        var _agGroup = _ag.GroupBy(s => s.AnimeId).Select(s => new
                        {
                            AnimeId = s.Key,
                            Count = s.Count()
                        });
                        animes = animes.Join(_agGroup.Where(s => s.Count == animeParamView.StudioId.Count()), s => s.Id, r => r.AnimeId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                    else
                    {
                        animes = animes.Join(_context.AnimeStudios.Where(s => (animeParamView.StudioId.Contains(s.StudioId ?? 0)) &&
                                                                            !s.IsDeleted), s => s.Id, r => r.AnimeId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                }
                if (animeParamView.ProducerId != null && animeParamView.ProducerId.Count > 0)
                {
                    if (animeParamView.IsProducerGroup == true)
                    {
                        var _ag = _context.AnimeProducers.Where(s => (animeParamView.ProducerId.Contains(s.ProducerId ?? 0)) && !s.IsDeleted);
                        var _agGroup = _ag.GroupBy(s => s.AnimeId).Select(s => new
                        {
                            AnimeId = s.Key,
                            Count = s.Count()
                        });
                        animes = animes.Join(_agGroup.Where(s => s.Count == animeParamView.ProducerId.Count()), s => s.Id, r => r.AnimeId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                    else
                    {
                        animes = animes.Join(_context.AnimeProducers.Where(s => (animeParamView.ProducerId.Contains(s.ProducerId ?? 0)) &&
                                                                            !s.IsDeleted), s => s.Id, r => r.AnimeId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                }
                if (animeParamView.GenreId != null && animeParamView.GenreId.Count > 0)
                {
                    if(animeParamView.IsGenreGroup == true)
                    {
                        var _ag = _context.AnimeGenres.Where(s => (animeParamView.GenreId.Contains(s.GenreId ?? 0)) && !s.IsDeleted);
                        var _agGroup = _ag.GroupBy(s => s.AnimeId).Select(s => new
                        {
                            AnimeId = s.Key,
                            Count = s.Count()
                        });
                        animes = animes.Join(_agGroup.Where(s => s.Count == animeParamView.GenreId.Count()), s => s.Id, r => r.AnimeId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                    else
                    {
                        animes = animes.Join(_context.AnimeGenres.Where(s => (animeParamView.GenreId.Contains(s.GenreId ?? 0)) &&
                                                                            !s.IsDeleted), s => s.Id, r => r.AnimeId, (s, r) => s).Distinct().OrderBy(s => s.Id);
                    }
                }
                if (animeParamView.SeasonId != null)
                {
                    if(animeParamView.SeasonId == -1)
                    {
                        animes = animes.Where(s => s.SeasonId == null);
                    }
                    else
                    {
                        animes = animes.Where(s => s.SeasonId == animeParamView.SeasonId);
                    }
                }
                if (animeParamView.Order == "asc")
                {
                    if (animeParamView.OrderBy == "latinName")
                    {
                        animes = animes.OrderBy(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "nativeName")
                    {
                        animes = animes.OrderBy(s => s.NativeName).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "createdAt")
                    {
                        animes = animes.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "seasonId")
                    {
                        animes = animes.OrderBy(s => s.Year).ThenBy(s => s.Quarter).ThenBy(s => s.Id);
                    }
                }
                else if (animeParamView.Order == "desc")
                {
                    if (animeParamView.OrderBy == "latinName")
                    {
                        animes = animes.OrderByDescending(s => s.LatinName).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "nativeName")
                    {
                        animes = animes.OrderByDescending(s => s.NativeName).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "createdAt")
                    {
                        animes = animes.OrderByDescending(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (animeParamView.OrderBy == "seasonId")
                    {
                        animes = animes.OrderByDescending(s => s.Year).ThenByDescending(s => s.Quarter).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await animes.CountAsync(cancellationToken);
                if (animeParamView.PageIndex > 0)
                {
                    animes = animes.Skip(animeParamView.PageSize * (animeParamView.PageIndex - 1)).Take(animeParamView.PageSize);
                }
                animes = animes.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                var list = await animes.ToListAsync(cancellationToken);
                if(list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            item.AnimeCharacter = await (from sc in _context.AnimeCharacters
                                                         where !sc.IsDeleted && sc.AnimeId == item.Id
                                                         join a in (from at in _context.Characters where !at.IsDeleted select at) on sc.CharacterId equals a.Id
                                                         select new AnimeCharacterView
                                                         {
                                                             Id = sc.Id,
                                                             AnimeId = sc.AnimeId,
                                                             CharacterId = sc.CharacterId,
                                                             IsMain = sc.IsMain,
                                                             CharacterNativeName = a.NativeName,
                                                             CharacterLatinName = a.LatinName
                                                         }).ToListAsync(cancellationToken);
                            item.AnimeGenre = await (from sg in _context.AnimeGenres
                                                     where !sg.IsDeleted && sg.AnimeId == item.Id
                                                     join a in (from at in _context.Genres where !at.IsDeleted select at) on sg.GenreId equals a.Id
                                                     select new AnimeGenreView
                                                     {
                                                         Id = sg.Id,
                                                         AnimeId = sg.AnimeId,
                                                         GenreId = sg.GenreId,
                                                         GenreName = a.GenreName
                                                     }).ToListAsync(cancellationToken);
                            item.AnimeProducer = await (from sg in _context.AnimeProducers
                                                        where !sg.IsDeleted && sg.AnimeId == item.Id
                                                        join a in (from at in _context.Producers where !at.IsDeleted select at) on sg.ProducerId equals a.Id
                                                        select new AnimeProducerView
                                                        {
                                                            Id = sg.Id,
                                                            AnimeId = sg.AnimeId,
                                                            ProducerId = sg.ProducerId,
                                                            ProducerName = a.ProducerName
                                                        }).ToListAsync(cancellationToken);
                            item.AnimeStudio = await (from sg in _context.AnimeStudios
                                                      where !sg.IsDeleted && sg.AnimeId == item.Id
                                                      join a in (from at in _context.Studios where !at.IsDeleted select at) on sg.StudioId equals a.Id
                                                      select new AnimeStudioView
                                                      {
                                                          Id = sg.Id,
                                                          AnimeId = sg.AnimeId,
                                                          StudioId = sg.StudioId,
                                                          StudioName = a.StudioName
                                                      }).ToListAsync(cancellationToken);
                            item.AnimeStaff = await (from sg in _context.AnimeStaffs
                                                     where !sg.IsDeleted && sg.AnimeId == item.Id
                                                     join a in (from at in _context.Staffs where !at.IsDeleted select at) on sg.StaffId equals a.Id
                                                     join l in (from lt in _context.Languages where !lt.IsDeleted select lt) on a.LanguageId equals l.Id into sl
                                                     from ll in sl.DefaultIfEmpty()
                                                     select new AnimeStaffView
                                                     {
                                                         Id = sg.Id,
                                                         AnimeId = sg.AnimeId,
                                                         StaffId = sg.StaffId,
                                                         Role = sg.Role,
                                                         StaffNativeName = a.NativeName,
                                                         StaffLanguage = ll.LanguageName,
                                                         StaffLatinName = a.LatinName
                                                     }).ToListAsync(cancellationToken);
                            item.ParentAnimeRelation = await (from sg in _context.AnimeRelations
                                                              where !sg.IsDeleted && sg.ParentId == item.Id
                                                              join a in (from at in _context.Animes where !at.IsDeleted select at) on sg.ChildId equals a.Id
                                                              select new AnimeRelationView
                                                              {
                                                                  Id = sg.Id,
                                                                  ChildId = sg.ChildId,
                                                                  ParentId = sg.ParentId,
                                                                  RelationTypeId = sg.RelationTypeId,
                                                                  ChildLatinName = a.LatinName,
                                                                  ChildNativeName = a.NativeName
                                                              }).ToListAsync(cancellationToken);
                            item.ChildAnimeRelation = await (from sg in _context.AnimeRelations
                                                             where !sg.IsDeleted && sg.ChildId == item.Id
                                                             join a in (from at in _context.Animes where !at.IsDeleted select at) on sg.ParentId equals a.Id
                                                             select new AnimeRelationView
                                                             {
                                                                 Id = sg.Id,
                                                                 ChildId = sg.ChildId,
                                                                 ParentId = sg.ParentId,
                                                                 RelationTypeId = sg.RelationTypeId,
                                                                 ParentLatinName = a.LatinName,
                                                                 ParentNativeName = a.NativeName
                                                             }).ToListAsync(cancellationToken);
                            item.AnimeImage = await (from si in _context.AnimeImages
                                                     orderby si.UpdatedAt descending
                                                     where si.AnimeId == item.Id && !si.IsDeleted
                                                     select new AnimeImageView
                                                     {
                                                         Id = si.Id,
                                                         AnimeId = si.AnimeId,
                                                         Image = si.Image,
                                                         IsAvatar = si.IsAvatar
                                                     }).ToListAsync(cancellationToken);
                            item.AnimeVideo = await (from si in _context.AnimeVideos
                                                     orderby si.UpdatedAt descending
                                                     where si.AnimeId == item.Id && !si.IsDeleted
                                                     select new AnimeVideoView
                                                     {
                                                         Id = si.Id,
                                                         AnimeId = si.AnimeId,
                                                         Link = si.Link,
                                                     }).ToListAsync(cancellationToken);
                        }
                    }
                }
				return new AnimeListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AnimeListView(new List<AnimeView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllAnime(AnimeQueryListView animeQueryListView, CancellationToken cancellationToken)
        {
            try
            {
                if (animeQueryListView.Id == null)
                {
                    return new CharacterListView(new(), 0, 2, MessageConstant.NO_DATA_REQUEST);
                }
                var animes = from s in _context.Animes
                             orderby s.Id
                             where !s.IsDeleted && animeQueryListView.Id.Contains(s.IdAnilist)
                             join r in (from rr in _context.AnimeTypes where !rr.IsDeleted select rr) on s.AnimeTypeId equals r.Id into rt
                             from st in rt.DefaultIfEmpty()
                             join r in (from rr in _context.AnimeStatuss where !rr.IsDeleted select rr) on s.AnimeStatusId equals r.Id into rss
                             from sss in rss.DefaultIfEmpty()
                             join r in (from rr in _context.AnimeSources where !rr.IsDeleted select rr) on s.AnimeSourceId equals r.Id into rsr
                             from ssr in rsr.DefaultIfEmpty()
                             join r in (from rr in _context.Ratings where !rr.IsDeleted select rr) on s.RatingId equals r.Id into rr
                             from sr in rr.DefaultIfEmpty()
                             join r in (from rr in _context.Seasons where !rr.IsDeleted select rr) on s.SeasonId equals r.Id into rse
                             from sse in rse.DefaultIfEmpty()
                             select new AnimeView
                             {
                                 Id = s.Id,
                                 CreatedAt = s.CreatedAt,
                                 NativeName = s.NativeName,
                                 LatinName = s.LatinName,
                                 OtherName = s.OtherName,
                                 AnimeSourceId = s.AnimeSourceId,
                                 AnimeTypeId = s.AnimeTypeId,
                                 AnimeStatusId = s.AnimeStatusId,
                                 RatingId = s.RatingId,
                                 SeasonId = s.SeasonId,
                                 LanguageId = s.LanguageId,
                                 DayOfEndTime = s.DayOfEndTime,
                                 DayOfStartTime = s.DayOfStartTime,
                                 MonthOfEndTime = s.MonthOfEndTime,
                                 MonthOfStartTime = s.MonthOfStartTime,
                                 YearOfEndTime = s.YearOfEndTime,
                                 YearOfStartTime = s.YearOfStartTime,
                                 HoursDuration = s.HoursDuration,
                                 MinutesDuration = s.MinutesDuration,
                                 SecondsDuration = s.SecondsDuration,
                                 Score = s.Score,
                                 Summary = s.Summary,
                                 Episodes = s.Episodes,
                                 TypeName = st.TypeName,
                                 StatusName = sss.StatusName,
                                 SourceName = ssr.SourceName,
                                 RatingName = sr.RatingName,
                                 Year = sse.Year,
                                 EndingSong = s.EndingSong,
                                 OpeningSong = s.OpeningSong,
                                 IsAdult = s.IsAdult,
                                 IdAnilist = s.IdAnilist,
                                 Quarter = sse.Quarter
                             };
                var totalRecord = await animes.CountAsync(cancellationToken);
                animes = animes.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
                var list = await animes.ToListAsync(cancellationToken);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            item.AnimeCharacter = await (from sc in _context.AnimeCharacters
                                                         where !sc.IsDeleted && sc.AnimeId == item.Id
                                                         join a in (from at in _context.Characters where !at.IsDeleted select at) on sc.CharacterId equals a.Id
                                                         select new AnimeCharacterView
                                                         {
                                                             Id = sc.Id,
                                                             AnimeId = sc.AnimeId,
                                                             CharacterId = sc.CharacterId,
                                                             IsMain = sc.IsMain,
                                                             CharacterNativeName = a.NativeName,
                                                             CharacterLatinName = a.LatinName
                                                         }).ToListAsync(cancellationToken);
                            item.AnimeGenre = await (from sg in _context.AnimeGenres
                                                     where !sg.IsDeleted && sg.AnimeId == item.Id
                                                     join a in (from at in _context.Genres where !at.IsDeleted select at) on sg.GenreId equals a.Id
                                                     select new AnimeGenreView
                                                     {
                                                         Id = sg.Id,
                                                         AnimeId = sg.AnimeId,
                                                         GenreId = sg.GenreId,
                                                         GenreName = a.GenreName
                                                     }).ToListAsync(cancellationToken);
                            item.AnimeProducer = await (from sg in _context.AnimeProducers
                                                        where !sg.IsDeleted && sg.AnimeId == item.Id
                                                        join a in (from at in _context.Producers where !at.IsDeleted select at) on sg.ProducerId equals a.Id
                                                        select new AnimeProducerView
                                                        {
                                                            Id = sg.Id,
                                                            AnimeId = sg.AnimeId,
                                                            ProducerId = sg.ProducerId,
                                                            ProducerName = a.ProducerName
                                                        }).ToListAsync(cancellationToken);
                            item.AnimeStudio = await (from sg in _context.AnimeStudios
                                                      where !sg.IsDeleted && sg.AnimeId == item.Id
                                                      join a in (from at in _context.Studios where !at.IsDeleted select at) on sg.StudioId equals a.Id
                                                      select new AnimeStudioView
                                                      {
                                                          Id = sg.Id,
                                                          AnimeId = sg.AnimeId,
                                                          StudioId = sg.StudioId,
                                                          StudioName = a.StudioName
                                                      }).ToListAsync(cancellationToken);
                            item.AnimeStaff = await (from sg in _context.AnimeStaffs
                                                     where !sg.IsDeleted && sg.AnimeId == item.Id
                                                     join a in (from at in _context.Staffs where !at.IsDeleted select at) on sg.StaffId equals a.Id
                                                     join l in (from lt in _context.Languages where !lt.IsDeleted select lt) on a.LanguageId equals l.Id into sl
                                                     from ll in sl.DefaultIfEmpty()
                                                     select new AnimeStaffView
                                                     {
                                                         Id = sg.Id,
                                                         AnimeId = sg.AnimeId,
                                                         StaffId = sg.StaffId,
                                                         Role = sg.Role,
                                                         StaffNativeName = a.NativeName,
                                                         StaffLanguage = ll.LanguageName,
                                                         StaffLatinName = a.LatinName
                                                     }).ToListAsync(cancellationToken);
                            item.ParentAnimeRelation = await (from sg in _context.AnimeRelations
                                                              where !sg.IsDeleted && sg.ParentId == item.Id
                                                              join a in (from at in _context.Animes where !at.IsDeleted select at) on sg.ChildId equals a.Id
                                                              select new AnimeRelationView
                                                              {
                                                                  Id = sg.Id,
                                                                  ChildId = sg.ChildId,
                                                                  ParentId = sg.ParentId,
                                                                  RelationTypeId = sg.RelationTypeId,
                                                                  ChildLatinName = a.LatinName,
                                                                  ChildNativeName = a.NativeName
                                                              }).ToListAsync(cancellationToken);
                            item.ChildAnimeRelation = await (from sg in _context.AnimeRelations
                                                             where !sg.IsDeleted && sg.ChildId == item.Id
                                                             join a in (from at in _context.Animes where !at.IsDeleted select at) on sg.ParentId equals a.Id
                                                             select new AnimeRelationView
                                                             {
                                                                 Id = sg.Id,
                                                                 ChildId = sg.ChildId,
                                                                 ParentId = sg.ParentId,
                                                                 RelationTypeId = sg.RelationTypeId,
                                                                 ParentLatinName = a.LatinName,
                                                                 ParentNativeName = a.NativeName
                                                             }).ToListAsync(cancellationToken);
                            item.AnimeImage = await (from si in _context.AnimeImages
                                                     orderby si.UpdatedAt descending
                                                     where si.AnimeId == item.Id && !si.IsDeleted
                                                     select new AnimeImageView
                                                     {
                                                         Id = si.Id,
                                                         AnimeId = si.AnimeId,
                                                         Image = si.Image,
                                                         IsAvatar = si.IsAvatar
                                                     }).ToListAsync(cancellationToken);
                            item.AnimeVideo = await (from si in _context.AnimeVideos
                                                     orderby si.UpdatedAt descending
                                                     where si.AnimeId == item.Id && !si.IsDeleted
                                                     select new AnimeVideoView
                                                     {
                                                         Id = si.Id,
                                                         AnimeId = si.AnimeId,
                                                         Link = si.Link,
                                                     }).ToListAsync(cancellationToken);
                        }
                    }
                }
                return new AnimeListView(list, totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new AnimeListView(new List<AnimeView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<AnimeView> GetAnimeById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var anime = from s in _context.Animes
                            where s.Id == id && !s.IsDeleted
                            select new AnimeView
                            {
                                Id = s.Id,
                                CreatedAt = s.CreatedAt,
                                NativeName = s.NativeName,
                                LatinName = s.LatinName,
                                OtherName = s.OtherName,
                                AnimeSourceId = s.AnimeSourceId,
                                AnimeTypeId = s.AnimeTypeId,
                                AnimeStatusId = s.AnimeStatusId,
                                RatingId = s.RatingId,
                                SeasonId = s.SeasonId,
                                LanguageId = s.LanguageId,
                                DayOfEndTime = s.DayOfEndTime,
                                DayOfStartTime = s.DayOfStartTime,
                                MonthOfEndTime = s.MonthOfEndTime,
                                MonthOfStartTime = s.MonthOfStartTime,
                                YearOfEndTime = s.YearOfEndTime,
                                YearOfStartTime = s.YearOfStartTime,
                                HoursDuration = s.HoursDuration,
                                MinutesDuration = s.MinutesDuration,
                                SecondsDuration = s.SecondsDuration,
                                Score = s.Score,
                                Summary = s.Summary,
                                Episodes = s.Episodes,
                                EndingSong = s.EndingSong,
                                OpeningSong = s.OpeningSong,
                                IsAdult = s.IsAdult,
                            };
                return await anime.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<Tuple<ResponseView, string?, string?>> UpdateAnime(AnimeView animeView, CancellationToken cancellationToken)
        {
            try
            {
                var anime = await _context.Animes.Where(s => (s.Id == animeView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (anime == null)
                {
                    return new(new(MessageConstant.NOT_FOUND, 2), null, null);
                }
                var listAnimeCharacter = await _context.AnimeCharacters.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                var listAnimeGenre = await _context.AnimeGenres.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                var listAnimeProducer = await _context.AnimeProducers.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                var listAnimeStudio = await _context.AnimeStudios.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                var listAnimeStaff = await _context.AnimeStaffs.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                var listAnimeVideo = await _context.AnimeVideos.Where(s => (s.AnimeId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                var listAnimeParent = await _context.AnimeRelations.Where(s => (s.ParentId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);
                var listAnimeChild = await _context.AnimeRelations.Where(s => (s.ChildId == animeView.Id && !s.IsDeleted)).ToListAsync(cancellationToken);

                if (animeView.AnimeCharacter != null && animeView.AnimeCharacter.Count > 0)
                {
                    var listCharacterId = animeView.AnimeCharacter.Select(x => x.CharacterId).Distinct().ToList();
                    var allCharacter = await _context.Characters.Where(s => !s.IsDeleted && listCharacterId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listCharacterId.Count();
                    if (!allCharacter)
                    {
                        return new(new(MessageConstant.NOT_FOUND_CHARACTER, 3), null, null);
                    }
                    foreach (var item in listAnimeCharacter)
                    {
                        var existed = animeView.AnimeCharacter.Where(s => (s.Id == item.Id && s.AnimeId == item.AnimeId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.CharacterId = existed.CharacterId;
                            item.IsMain = existed.IsMain;
                            item.UpdatedAt = DateTime.UtcNow;
                            animeView.AnimeCharacter.Remove(existed);
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
                    foreach (var item in listAnimeCharacter)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                if (animeView.AnimeGenre != null && animeView.AnimeGenre.Count > 0)
                {
                    var listGenreId = animeView.AnimeGenre.Select(x => x.GenreId).Distinct().ToList();
                    var allGenre = await _context.Genres.Where(s => !s.IsDeleted && listGenreId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listGenreId.Count();
                    if (!allGenre)
                    {
                        return new(new(MessageConstant.NOT_FOUND_GENRE, 3), null, null);
                    }
                    foreach (var item in listAnimeGenre)
                    {
                        var existed = animeView.AnimeGenre.Where(s => (s.Id == item.Id && s.AnimeId == item.AnimeId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.GenreId = existed.GenreId;
                            item.UpdatedAt = DateTime.UtcNow;
                            animeView.AnimeGenre.Remove(existed);
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
                    foreach (var item in listAnimeGenre)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                if (animeView.AnimeProducer != null && animeView.AnimeProducer.Count > 0)
                {
                    var listProducerId = animeView.AnimeProducer.Select(x => x.ProducerId).Distinct().ToList();
                    var allProducer = await _context.Producers.Where(s => !s.IsDeleted && listProducerId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listProducerId.Count();
                    if (!allProducer)
                    {
                        return new(new(MessageConstant.NOT_FOUND_PRODUCER, 3), null, null);
                    }
                    foreach (var item in listAnimeProducer)
                    {
                        var existed = animeView.AnimeProducer.Where(s => (s.Id == item.Id && s.AnimeId == item.AnimeId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.ProducerId = existed.ProducerId;
                            item.UpdatedAt = DateTime.UtcNow;
                            animeView.AnimeProducer.Remove(existed);
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
                    foreach (var item in listAnimeProducer)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                if (animeView.AnimeStudio != null && animeView.AnimeStudio.Count > 0)
                {
                    var listStudioId = animeView.AnimeStudio.Select(x => x.StudioId).Distinct().ToList();
                    var allStudio = await _context.Studios.Where(s => !s.IsDeleted && listStudioId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listStudioId.Count();
                    if (!allStudio)
                    {
                        return new(new(MessageConstant.NOT_FOUND_STUDIO, 3), null, null);
                    }
                    foreach (var item in listAnimeStudio)
                    {
                        var existed = animeView.AnimeStudio.Where(s => (s.Id == item.Id && s.AnimeId == item.AnimeId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.StudioId = existed.StudioId;
                            item.UpdatedAt = DateTime.UtcNow;
                            animeView.AnimeStudio.Remove(existed);
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
                    foreach (var item in listAnimeStudio)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                if (animeView.AnimeStaff != null && animeView.AnimeStaff.Count > 0)
                {
                    var listStaffId = animeView.AnimeStaff.Select(x => x.StaffId).Distinct().ToList();
                    var allStaff = await _context.Staffs.Where(s => !s.IsDeleted && listStaffId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listStaffId.Count();
                    if (!allStaff)
                    {
                        return new(new(MessageConstant.NOT_FOUND_STAFF, 3), null, null);
                    }
                    foreach (var item in listAnimeStaff)
                    {
                        var existed = animeView.AnimeStaff.Where(s => (s.Id == item.Id && s.AnimeId == item.AnimeId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.StaffId = existed.StaffId;
                            item.UpdatedAt = DateTime.UtcNow;
                            item.Role = existed.Role;
                            animeView.AnimeStaff.Remove(existed);
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
                    foreach (var item in listAnimeStaff)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                if (animeView.AnimeVideo != null && animeView.AnimeVideo.Count > 0)
                {
                    foreach (var item in listAnimeVideo)
                    {
                        var existed = animeView.AnimeVideo.Where(s => (s.Id == item.Id && s.AnimeId == item.AnimeId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.Link = existed.Link;
                            item.UpdatedAt = DateTime.UtcNow;
                            animeView.AnimeVideo.Remove(existed);
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
                    foreach (var item in listAnimeVideo)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                if (animeView.ParentAnimeRelation != null && animeView.ParentAnimeRelation.Count > 0)
                {
                    var listChildId = animeView.ParentAnimeRelation.Select(x => x.ChildId).Distinct().ToList();
                    var allChild = await _context.Animes.Where(s => !s.IsDeleted && listChildId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listChildId.Count();
                    if (!allChild)
                    {
                        return new(new(MessageConstant.NOT_FOUND_ANIME, 3), null, null);
                    }
                    foreach (var item in listAnimeParent)
                    {
                        var existed = animeView.ParentAnimeRelation.Where(s => (s.Id == item.Id && s.ParentId == item.ParentId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.ChildId = existed.ChildId;
                            item.RelationTypeId = existed.RelationTypeId;
                            item.UpdatedAt = DateTime.UtcNow;
                            animeView.ParentAnimeRelation.Remove(existed);
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
                    foreach (var item in listAnimeParent)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }
                if (animeView.ChildAnimeRelation != null && animeView.ChildAnimeRelation.Count > 0)
                {
                    var listParentId = animeView.ChildAnimeRelation.Select(x => x.ParentId).Distinct().ToList();
                    var allParent = await _context.Animes.Where(s => !s.IsDeleted && listParentId.Contains(s.Id)).Distinct().CountAsync(cancellationToken) == listParentId.Count();
                    if (!allParent)
                    {
                        return new(new(MessageConstant.NOT_FOUND_ANIME, 3), null, null);
                    }
                    foreach (var item in listAnimeChild)
                    {
                        var existed = animeView.ChildAnimeRelation.Where(s => (s.Id == item.Id && s.ChildId == item.ChildId)).FirstOrDefault();
                        if (existed != null)
                        {
                            item.ParentId = existed.ParentId;
                            item.RelationTypeId = existed.RelationTypeId;
                            item.UpdatedAt = DateTime.UtcNow;
                            animeView.ChildAnimeRelation.Remove(existed);
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
                    foreach (var item in listAnimeChild)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }

                var latinName = anime.LatinName;
                _mapper.Map(animeView, anime);
                anime.UpdatedAt = DateTime.UtcNow;

                if (anime.AnimeCharacter != null && anime.AnimeCharacter.Count > 0)
                {
                    anime.AnimeCharacter.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.AnimeCharacter = new();
                }
                if (anime.AnimeGenre != null && anime.AnimeGenre.Count > 0)
                {
                    anime.AnimeGenre.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.AnimeGenre = new();
                }
                if (anime.AnimeProducer != null && anime.AnimeProducer.Count > 0)
                {
                    anime.AnimeProducer.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.AnimeProducer = new();
                }
                if (anime.AnimeStudio != null && anime.AnimeStudio.Count > 0)
                {
                    anime.AnimeStudio.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.AnimeStudio = new();
                }
                if (anime.AnimeStaff != null && anime.AnimeStaff.Count > 0)
                {
                    anime.AnimeStaff.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.AnimeStaff = new();
                }
                if (anime.AnimeVideo != null && anime.AnimeVideo.Count > 0)
                {
                    anime.AnimeVideo.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.AnimeVideo = new();
                }
                if (anime.ParentAnimeRelation != null && anime.ParentAnimeRelation.Count > 0)
                {
                    anime.ParentAnimeRelation.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.ParentAnimeRelation = new();
                }
                if (anime.ChildAnimeRelation != null && anime.ChildAnimeRelation.Count > 0)
                {
                    anime.ChildAnimeRelation.ForEach(va =>
                    {
                        va.CreatedAt = DateTime.UtcNow;
                        va.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    anime.ChildAnimeRelation = new();
                }

                anime.AnimeCharacter.AddRange(listAnimeCharacter);
                anime.AnimeGenre.AddRange(listAnimeGenre);
                anime.AnimeProducer.AddRange(listAnimeProducer);
                anime.AnimeStudio.AddRange(listAnimeStudio);
                anime.AnimeStaff.AddRange(listAnimeStaff);
                anime.AnimeVideo.AddRange(listAnimeVideo);
                anime.ParentAnimeRelation.AddRange(listAnimeParent);
                anime.ChildAnimeRelation.AddRange(listAnimeChild);

                await _context.SaveChangesAsync(cancellationToken);
                var nativeName = anime.NativeName;
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
