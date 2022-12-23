using AutoMapper;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.MedicalTestViewModels;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;

namespace BSN.Resa.DoctorApp.Domain.Utilities
{
	public class DefaultMappingProfile : Profile
	{
		public DefaultMappingProfile()
		{
			CreateMap<Contact, Contact>();

			CreateMap<DoctorPreviewViewModel, Doctor>()
				.ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.FirstName))
				.ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.LastName))
				.ForMember(dst => dst.Msisdn, opt => opt.MapFrom(src => src.MSISDN))
			    .ForMember(dst => dst.State, opt => opt.MapFrom(src => src.State))
                .ForAllOtherMembers(opt => opt.Ignore());

			CreateMap<Core.Commons.ViewModels.AppUpdateManifest, AppUpdate>()
				.ForMember(dst => dst.HasUrgentUpdateLocally, opt => opt.MapFrom(src => src.HasUrgentUpdate))
				.ForMember(dst => dst.HasNotifiableUpdateLocally, opt => opt.MapFrom(src => src.HasNotifiableUpdate))
				.ForMember(dst => dst.LatestDownloadableAppUpdateUrlLocally, opt => opt.MapFrom(src => src.LatestDownloadableAppUpdateInfo != null ? src.LatestDownloadableAppUpdateInfo.Url : null))
				.ForMember(dst => dst.LatestDownloadableAppUpdateVersionLocally, opt => opt.MapFrom(src => src.LatestDownloadableAppUpdateInfo != null ? src.LatestDownloadableAppUpdateInfo.Version : null))
				.ForAllOtherMembers(opt => opt.Ignore());

			CreateMap<OauthToken, OauthToken>();

		    CreateMap<CallbackRequestPreview, CallbackRequest>(MemberList.Source);

		    CreateMap<CallbackRequest, CallbackRequest>()
		        .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
		        .ForMember(dst => dst.CommunicationAttemptsCount, opt => opt.MapFrom(src => src.CommunicationAttemptsCount))
		        .ForMember(dst => dst.ConsentGivenAt, opt => opt.MapFrom(src => src.ConsentGivenAt))
		        .ForMember(dst => dst.CallerFullName, opt => opt.MapFrom(src => src.CallerFullName))
		        .ForMember(dst => dst.CallerSubscriberNumber, opt => opt.MapFrom(src => src.CallerSubscriberNumber))
		        .ForMember(dst => dst.ReceiverFullName, opt => opt.MapFrom(src => src.ReceiverFullName))
		        .ForMember(dst => dst.ReceiverSubscriberNumber, opt => opt.MapFrom(src => src.ReceiverSubscriberNumber))
		        .ForMember(dst => dst.IsExpired, opt => opt.MapFrom(src => src.IsExpired))
		        .ForMember(dst => dst.ReturnCallHasBeenEstablished, opt => opt.MapFrom(src => src.ReturnCallHasBeenEstablished))
                .ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dst => dst.Credit, opt => opt.MapFrom(src => src.Credit))
                .ForMember(dst => dst.IsCancelled, opt => opt.MapFrom(src => src.IsCancelled))
                .ForAllOtherMembers(opt => opt.Ignore());

			CreateMap<MedicalTestViewModel, MedicalTest>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status))
				.ForMember(dst => dst.Photos, opt => opt.MapFrom(src => src.Files))
				.ForMember(dst => dst.Price, opt => opt.MapFrom(src => src.Price))
				.ForMember(dst => dst.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
				.ForMember(dst => dst.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
				.ForMember(dst => dst.PatientId, opt => opt.MapFrom(src => src.User != null && src.User.Id != null ? src.User.Id : null))
				.ForMember(dst => dst.PatientPhone, opt => opt.MapFrom(src => src.User != null && src.User.Phone != null ? src.User.Phone : null))
				.ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MedicalTest, MedicalTest>();
		}
    }

    public class DoctorAppAutoMapper
    {
		public static IMapper Instance
		{
			get
			{
				if (_instance != null)
					return _instance;

				var configuration = new MapperConfiguration(cfg => cfg.AddProfile<DefaultMappingProfile>());
				configuration.AssertConfigurationIsValid();

				_instance = configuration.CreateMapper();
				return _instance;
			}
		}

		private static IMapper _instance;
	}
}
