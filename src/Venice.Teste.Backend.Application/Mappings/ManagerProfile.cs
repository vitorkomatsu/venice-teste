using AutoMapper;
using Venice.Teste.Backend.Application.DTOs.Request;
using Venice.Teste.Backend.Application.DTOs.Response;

namespace Venice.Teste.Backend.Application.Mappings
{
    public class ManagerProfile : Profile
	{
		public ManagerProfile()
		{
			//Request
			CreateMap<ProductRequest, Domain.Entities.Product>().ReverseMap();

			//Response
			CreateMap<ProductResponse, Domain.Entities.Product>().ReverseMap();

            // Order mappings
            CreateMap<OrderResponse, Domain.Entities.Order>().ReverseMap();
            CreateMap<OrderItemResponse, Domain.Entities.OrderItem>().ReverseMap();
		}
	}
}