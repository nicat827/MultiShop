using AutoMapper;

namespace MultiShop.MappingProfiles
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductGetItemVM>();
            CreateMap<ProductCreateVM, Product>();
            CreateMap<ProductUpdateVM, Product>().ReverseMap();
            CreateMap<ProductGetVM, Product>().ReverseMap();
        }
    }
}
