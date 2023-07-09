using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.ModelViews;
using AutoMapper;

namespace AnimeVnInfoBackend.Utilities.Assemblies
{
    public class AnimeMapperProfile : Profile
    {
        public AnimeMapperProfile()
        {
            CreateMap<UserView, User>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDisabled, option => option.Ignore());
            CreateMap<UserInfoView, UserInfo>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<CharacterView, Character>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.CharacterImage, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<RoleView, Role>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore());
            CreateMap<AdminNavigationView, AdminNavigation>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.Order, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<SeasonView, Season>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<StudioView, Studio>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.StudioImage, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<StudioImageView, StudioImage>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<ProducerView, Producer>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.ProducerImage, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<ProducerImageView, ProducerImage>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeTypeView, AnimeType>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.Order, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeStatusView, AnimeStatus>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.Order, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeSourceView, AnimeSource>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.Order, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<RatingView, Rating>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.Order, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<LoggerView, Logger>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<ReviewScoreView, ReviewScore>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<GenreView, Genre>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<LanguageView, Language>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<StaffView, Staff>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.StaffImage, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<StaffImageView, StaffImage>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<CharacterStaffView, CharacterStaff>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeListStatusView, AnimeListStatus>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.Order, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<CharacterImageView, CharacterImage>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeView, Anime>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.AnimeImage, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeGenreView, AnimeGenre>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeProducerView, AnimeProducer>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeStudioView, AnimeStudio>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeCharacterView, AnimeCharacter>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeImageView, AnimeImage>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<JobView, Job>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<StaffJobView, StaffJob>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<RelationTypeView, RelationType>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.Order, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeRelationView, AnimeRelation>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeVideoView, AnimeVideo>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
            CreateMap<AnimeStaffView, AnimeStaff>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.CreatedAt, option => option.Ignore())
                .ForMember(destination => destination.UpdatedAt, option => option.Ignore())
                .ForMember(destination => destination.DeletedAt, option => option.Ignore())
                .ForMember(destination => destination.IsDeleted, option => option.Ignore());
        }
    }
}
