using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers
{
    [ApiController]
    [Route("Bank")]
    public class BankController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Create(Bank.Domain.Bank bank)
        {
            return Ok();

        }

        [HttpPut]
        public IActionResult Update(Bank.Domain.Bank bank)
        {
            return Ok();

        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            return Ok();

        }

    }
}
