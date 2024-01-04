using AutoMapper;

namespace MultiShop.MappingProfiles
{
    public class SlideProfile:Profile
    {
        public SlideProfile()
        {
            CreateMap<Slide, SlideGetItemVM>();
            CreateMap<SlideCreateVM, Slide>();
            CreateMap<SlideUpdateVM, Slide>().ReverseMap();

        }
    }
}
