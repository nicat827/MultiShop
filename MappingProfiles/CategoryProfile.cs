using AutoMapper;
using MultiShop.Entities;

namespace MultiShop.MappingProfiles
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryGetItemVM>();
            
        }
    }
}
