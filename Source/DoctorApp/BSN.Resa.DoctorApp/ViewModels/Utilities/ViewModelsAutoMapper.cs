using AutoMapper;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;

namespace BSN.Resa.DoctorApp.ViewModels.Utilities
{
    public class ViewModelsMappingProfile : Profile
    {
        public ViewModelsMappingProfile()
        {

            CreateMap<CallbackRequest, CallbackRequestBindableObject>(MemberList.Source);

        }
    }

    public class ViewModelsAutoMapper
    {
        public static IMapper Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ViewModelsMappingProfile>());
                configuration.AssertConfigurationIsValid();

                _instance = configuration.CreateMapper();
                return _instance;
            }
        }

        private static IMapper _instance;
    }
}
