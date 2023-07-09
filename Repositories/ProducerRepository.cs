using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AnimeVnInfoBackend.Utilities.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.Repositories
{
    public interface IProducerRepository
    {
        public Task<ResponseWithPaginationView> GetAllProducer(ProducerParamView producerParamView, CancellationToken cancellationToken);
        public Task<ResponseWithPaginationView> GetAllProducer(ProducerQueryListView producerQueryListView, CancellationToken cancellationToken);
        public Task<ProducerView> GetProducerById(int id, CancellationToken cancellationToken);
        public Task<ResponseView> CreateProducer(ProducerView producerView, CancellationToken cancellationToken);
        public Task<Tuple<ResponseView, string?>> UpdateProducer(ProducerView producerView, CancellationToken cancellationToken);
        public Task<ResponseView> DeleteProducer(ProducerView producerView, CancellationToken cancellationToken);
    }

    public class ProducerRepository : IProducerRepository
    {
        private readonly ILogger<ProducerRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProducerRepository(ApplicationDbContext context, ILogger<ProducerRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseView> CreateProducer(ProducerView producerView, CancellationToken cancellationToken)
        {
            try
            {
                var checkExisted = await _context.Producers.Where(s => (s.ProducerName == producerView.ProducerName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(MessageConstant.EXISTED, 2);
                }
                Producer newProducer = new();
                newProducer = _mapper.Map<Producer>(producerView);
                newProducer.CreatedAt = DateTime.UtcNow;
                newProducer.UpdatedAt = DateTime.UtcNow;
                await _context.Producers.AddAsync(newProducer, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.CREATE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseView> DeleteProducer(ProducerView producerView, CancellationToken cancellationToken)
        {
            try
            {
                var producer = await _context.Producers.Where(s => (s.Id == producerView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (producer == null)
                {
                    return new(MessageConstant.NOT_FOUND, 2);
                }
                await _context.AnimeProducers.Where(s => (s.ProducerId == producerView.Id && !s.IsDeleted)).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true)
                                                                                                                                    .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
                producer.IsDeleted = true;
                producer.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(MessageConstant.DELETE_SUCCESS, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(MessageConstant.SYSTEM_ERROR, 1);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllProducer(ProducerParamView producerParamView, CancellationToken cancellationToken)
        {
            try
            {
                var producers = from s in _context.Producers
                                orderby s.Id
                                where !s.IsDeleted
                                select new ProducerView
                                {
                                    ProducerName = s.ProducerName,
                                    Description = s.Description,
                                    Id = s.Id,
                                    IdAnilist = s.IdAnilist,
                                    Established = s.Established,
                                    ProducerLink = s.ProducerLink,
                                    CreatedAt = s.CreatedAt,
                                    ProducerImage = (from si in _context.ProducerImages
                                                     orderby si.UpdatedAt descending
                                                     where si.ProducerId == s.Id && !si.IsDeleted
                                                     select new ProducerImageView
                                                     {
                                                         Id = si.Id,
                                                         ProducerId = si.ProducerId,
                                                         Image = si.Image,
                                                         IsAvatar = si.IsAvatar
                                                     }).ToList(),
                                    AnimeCount = (from ss in _context.AnimeProducers
                                                  where ss.ProducerId == s.Id && !ss.IsDeleted
                                                  select ss).Count()
                                };
                if (!await producers.AnyAsync(cancellationToken))
                {
                    return new ProducerListView(new List<ProducerView>(), 0, 2, MessageConstant.NOT_FOUND);
                }
                if (!string.IsNullOrWhiteSpace(producerParamView.SearchString))
                {
                    producers = producers.Where(s => ((s.ProducerName != null && s.ProducerName.ToLower().Contains(producerParamView.SearchString.ToLower())) ||
                                                (s.Description != null && s.Description.ToLower().Contains(producerParamView.SearchString.ToLower())) ||
                                                (s.ProducerLink != null && s.ProducerLink.ToLower().Contains(producerParamView.SearchString.ToLower())) ||
                                                (s.Established != null && (s.Established.ToString() ?? "").ToLower().Contains(producerParamView.SearchString.ToLower()))));
                }
                if (!string.IsNullOrWhiteSpace(producerParamView.SearchName))
                {
                    producers = producers.Where(s => (s.ProducerName ?? "").ToLower().Contains(producerParamView.SearchName.ToLower()));
                }
                if (producerParamView.Order == "asc")
                {
                    if (producerParamView.OrderBy == "producerName")
                    {
                        producers = producers.OrderBy(s => s.ProducerName).ThenBy(s => s.Id);
                    }
                    else if (producerParamView.OrderBy == "established")
                    {
                        producers = producers.OrderBy(s => s.Established).ThenBy(s => s.Id);
                    }
                    else if (producerParamView.OrderBy == "createdAt")
                    {
                        producers = producers.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (producerParamView.OrderBy == "animeCount")
                    {
                        producers = producers.OrderBy(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                else if (producerParamView.Order == "desc")
                {
                    if (producerParamView.OrderBy == "producerName")
                    {
                        producers = producers.OrderByDescending(s => s.ProducerName).ThenBy(s => s.Id);
                    }
                    else if (producerParamView.OrderBy == "established")
                    {
                        producers = producers.OrderByDescending(s => s.Established).ThenBy(s => s.Id);
                    }
                    else if (producerParamView.OrderBy == "createdAt")
                    {
                        producers = producers.OrderByDescending(s => s.CreatedAt).ThenBy(s => s.Id);
                    }
                    else if (producerParamView.OrderBy == "animeCount")
                    {
                        producers = producers.OrderByDescending(s => s.AnimeCount).ThenBy(s => s.Id);
                    }
                }
                var totalRecord = await producers.CountAsync(cancellationToken);
                if (producerParamView.PageIndex > 0)
                {
                    producers = producers.Skip(producerParamView.PageSize * (producerParamView.PageIndex - 1)).Take(producerParamView.PageSize);
                }
                producers = producers.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
				return new ProducerListView(await producers.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ProducerListView(new List<ProducerView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ResponseWithPaginationView> GetAllProducer(ProducerQueryListView producerQueryListView, CancellationToken cancellationToken)
        {
            try
            {
                if (producerQueryListView.Id == null)
                {
                    return new ProducerListView(new(), 0, 2, MessageConstant.NO_DATA_REQUEST);
                }
                var producers = from s in _context.Producers
                                orderby s.Id
                                where !s.IsDeleted && producerQueryListView.Id.Contains(s.IdAnilist)
                                select new ProducerView {
                                    ProducerName = s.ProducerName,
                                    Description = s.Description,
                                    Id = s.Id,
                                    IdAnilist = s.IdAnilist,
                                    Established = s.Established,
                                    ProducerLink = s.ProducerLink,
                                    CreatedAt = s.CreatedAt,
                                    ProducerImage = (from si in _context.ProducerImages
                                                     orderby si.UpdatedAt descending
                                                     where si.ProducerId == s.Id && !si.IsDeleted
                                                     select new ProducerImageView {
                                                         Id = si.Id,
                                                         ProducerId = si.ProducerId,
                                                         Image = si.Image,
                                                         IsAvatar = si.IsAvatar
                                                     }).ToList(),
                                    AnimeCount = (from ss in _context.AnimeProducers
                                                  where ss.ProducerId == s.Id && !ss.IsDeleted
                                                  select ss).Count()
                                };
                var totalRecord = await producers.CountAsync(cancellationToken);
                producers = producers.Take(EnvironmentConstant.MAX_ITEM_PER_PAGE);
				return new ProducerListView(await producers.ToListAsync(cancellationToken), totalRecord, 0, MessageConstant.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ProducerListView(new List<ProducerView>(), 0, 1, MessageConstant.SYSTEM_ERROR);
            }
        }

        public async Task<ProducerView> GetProducerById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var producer = from s in _context.Producers
                               where s.Id == id && !s.IsDeleted
                               select new ProducerView
                               {
                                   ProducerName = s.ProducerName,
                                   Description = s.Description,
                                   Id = s.Id,
                                   Established = s.Established,
                                   ProducerLink = s.ProducerLink,
                                   CreatedAt = s.CreatedAt
                               };
                return await producer.FirstOrDefaultAsync(cancellationToken) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }

        public async Task<Tuple<ResponseView, string?>> UpdateProducer(ProducerView producerView, CancellationToken cancellationToken)
        {
            try
            {
                var producer = await _context.Producers.Where(s => (s.Id == producerView.Id && !s.IsDeleted)).FirstOrDefaultAsync(cancellationToken);
                if (producer == null)
                {
                    return new(new(MessageConstant.NOT_FOUND, 2), null);
                }
                var checkExisted = await _context.Producers.Where(s => (s.ProducerName == producerView.ProducerName && s.ProducerName != producer.ProducerName && !s.IsDeleted)).AnyAsync(cancellationToken);
                if (checkExisted)
                {
                    return new(new(MessageConstant.EXISTED, 3), null);
                }
                var name = producer.ProducerName;
                _mapper.Map(producerView, producer);
                producer.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new(new(MessageConstant.UPDATE_SUCCESS, 0), name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(new(MessageConstant.SYSTEM_ERROR, 1), null);
            }
        }
    }
}
