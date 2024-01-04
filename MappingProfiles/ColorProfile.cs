using AutoMapper;

namespace MultiShop.MappingProfiles
{
    public class ColorProfile:Profile
    {
        public ColorProfile()
        {
            CreateMap<Color, ColorGetItemVM>();
            
        }
    }
}
