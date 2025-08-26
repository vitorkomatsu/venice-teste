using MediatR;
using Microsoft.AspNetCore.Mvc;
using Venice.Teste.Backend.Application.DTOs.Request;

namespace Venice.Teste.Backend.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/pedidos")]
    public class OrderController : BaseController<OrderController>
    {
        public OrderController(IMediator mediator, ILogger<OrderController> logger) : base(mediator, logger)
        { }

        [HttpPost("customer/{customerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> CreateAsync(Guid customerId, [FromBody] OrderRequest request)
        {
            var result = await Mediator.Send(new Application.UseCases.Order.Create.Command(customerId, request));
            return CreatedAtAction(nameof(GetByIdAsync), new { version = "1", orderId = result }, null);
        }

        [HttpGet("{orderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetByIdAsync(Guid orderId)
        {
            var result = await Mediator.Send(new Application.UseCases.Order.GetById.Command(orderId));
            return Ok(result);
        }

        [HttpGet("{orderId:guid}/items")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetItemsAsync(Guid orderId)
        {
            var result = await Mediator.Send(new Application.UseCases.Order.GetItems.Command(orderId));
            return Ok(result);
        }
    }
}