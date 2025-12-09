using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers
{
    [ApiController]
    [Route("Bank")]
    public class BankController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult GetById(Guid id)
        {
            return Ok();
        }


        [HttpPost]
        public IActionResult Create(Domain.Bank bank)
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult Update(Domain.Bank bank)
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
