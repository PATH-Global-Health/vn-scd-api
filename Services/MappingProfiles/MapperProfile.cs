using AutoMapper;
using Data.Entities;
using Data.Models;

namespace Services.MappingProfiles
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            CreateMap<DoctorCreateModel, Doctor>();
            //.ForMember(m => m.Id, opt => opt.Ignore());
            CreateMap<DoctorUpdateModel, Doctor>()
                .ForMember(m => m.Id, map => map.MapFrom(um => um.Id));
            CreateMap<Doctor, DoctorModel>();

            CreateMap<UnitTypeCreateModel, UnitType>();
            //.ForMember(m => m.Id, opt => opt.Ignore());
            CreateMap<UnitTypeUpdateModel, UnitType>()
                .ForMember(m => m.Id, map => map.MapFrom(um => um.Id));
            CreateMap<UnitType, UnitTypeModel>();

            CreateMap<UnitDoctorCreateModel, UnitDoctor>();
            CreateMap<UnitDoctorUpdateModel, UnitDoctor>()
                .ForMember(m => m.Id, map => map.MapFrom(um => um.Id));
            CreateMap<UnitDoctor, UnitDoctorModel>();

            CreateMap<Doctor, DoctorViewModel>();

            CreateMap<ServiceUnitCreateModel, ServiceUnit>();
            CreateMap<ServiceUnitUpdateModel, ServiceUnit>()
                .ForMember(m => m.Id, map => map.MapFrom(um => um.Id));
            CreateMap<ServiceUnit, ServiceUnitModel>();

            CreateMap<WorkingCalendar, WorkingCalendarViewModel>()
                .ForMember(m => m.Doctor, map => map.Ignore())
                .ForMember(m => m.Services, map => map.MapFrom(_m => _m.ServiceCalendars))
                .ForMember(m => m.Room, map => map.Ignore())
                .ForMember(m => m.FromTo, map => map.Ignore())
                .PreserveReferences()
                .ReverseMap();

            //CreateMap<WorkingCalendar, WorkingCalendarGetViewModel>().ForMember(w => w.Schedules, map => map.Ignore()).PreserveReferences();

            CreateMap<WorkingCalendar, WorkingCalendarCreateReturnModel>().PreserveReferences();

            //CreateMap<DoctorWorkingCalendar, DoctorWorkingCalendarModel>().PreserveReferences();


            CreateMap<InjectionObject, InjectionObjectAddModel>();
            CreateMap<InjectionObjectAddModel, InjectionObject>();

            CreateMap<InjectionObject, InjectionObjectViewModel>();
            CreateMap<InjectionObjectViewModel, InjectionObject>();

            CreateMap<Interval, IntervalViewModel>().ReverseMap();
            CreateMap<UserInformationViewModel, UserInformation>().ReverseMap();
            CreateMap<UserInformationModel, UserInformation>().ForMember(m => m.Username, map => map.Ignore()).ReverseMap();

            CreateMap<Data.Entities.Profile, ProfileAddModel>().ReverseMap();
            CreateMap<Data.Entities.Profile, ProfileUpdateModel>().ReverseMap();
            CreateMap<Data.Entities.Profile, ProfileViewModel>().ReverseMap();
            CreateMap<Data.Entities.Profile, CustomerIdentification>().ReverseMap();


            CreateMap<ServiceType, ServiceTypeModel>().ReverseMap();

            CreateMap<Day, DayViewModel>();

            CreateMap<Unit, UnitModel>().ForMember(f => f.Logo, m => m.Ignore()).ReverseMap();
            CreateMap<CreateOrganizationModel, Unit>();
            CreateMap<LayTestCreateModel, LayTest>().ReverseMap();
            CreateMap<Patient, PatientCreateModel>()
                .ForMember(f => f.CustomerInfo, m => m.Ignore())
                .ReverseMap();


            // Ticket
            CreateMap<ReferTicketCreateModel, ReferTickets>().ReverseMap();
            CreateMap<TicketEmployeeModel, ReferTickets>().ReverseMap();
            CreateMap<ReferTickets, ReferTicketViewModel>();
            CreateMap<ReferTickets, TicketViewModel>();

            //ProfileLinks
            CreateMap<ProfileLinks, ProfileLinkViewModel>();


            //Item
            CreateMap<ItemCreateModel, Item>();
            CreateMap<Item, ItemViewModel>();
            CreateMap<ItemUpdateModel, Item>()
                .ForAllMembers(opt => opt.Condition((src, des, srcMember) => srcMember != null));


            //ItemSrc
            CreateMap<ItemSourceCreateModel, ItemSource>();
            CreateMap<ItemSource, ItemSourceViewModel>();
            CreateMap<ItemSourceUpdateModel, ItemSource>()
                .ForAllMembers(opt => opt.Condition((src, des, srcMember) => srcMember != null));

        }
    }
}
