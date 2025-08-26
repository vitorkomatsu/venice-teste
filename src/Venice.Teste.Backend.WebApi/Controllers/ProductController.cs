using MediatR;
using Microsoft.AspNetCore.Mvc;
using Venice.Teste.Backend.Application.DTOs.Request;


namespace Venice.Teste.Backend.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    public class ProductController : BaseController<ProductController>
    {
        public ProductController(IMediator mediator, ILogger<ProductController> logger) : base(mediator, logger)
        { }

        [HttpPost("customer/{customerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> CreateAsync(Guid customerId, [FromBody] ProductRequest request)
        {
            var result = await Mediator.Send(new Application.UseCases.Product.Create.Command(customerId, request));
            return Created(string.Empty, result);
        }

        [HttpPut("{id}/customer/{customerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> UpdateAsync(Guid customerId, Guid id, [FromBody] ProductRequest request)
        {
            var result = await Mediator.Send(new Application.UseCases.Product.Update.Command(customerId, id, request));
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetAllAsync(Guid customerId, [FromQuery] PageOptions request)
        {
            var result = await Mediator.Send(new Application.UseCases.Product.GetAll.Command(customerId, PageOptions: request));
            return Ok(result);
        }

        [HttpGet("{id}/customer/{customerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Find))]
        public async Task<IActionResult> GetByIdAsync(Guid customerId, Guid id)
        {
            var result = await Mediator.Send(new Application.UseCases.Product.GetById.Command(customerId, id));
            return Ok(result);
        }

        [HttpDelete("{id}/customer/{customerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> DeleteAsync(Guid customerId, Guid id)
        {
            var result = await Mediator.Send(new Application.UseCases.Product.Delete.Command(customerId, id));
            return Ok(result);
        }
    }
}