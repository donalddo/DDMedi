using DDMedi;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IDDBroker _ddBroker;
        public BaseController(IDDBroker ddBroker)
        {
            _ddBroker = ddBroker;
        }

    }
}
