using DDMedi;
using DemoWebApi.Handlers;
using DemoWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DemoWebApi.Controllers
{
    public class QueryController : BaseController
    {
        public QueryController(IDDBroker ddBroker) : base(ddBroker) { }
        [HttpGet("get-async/{id}")]
        public Task<DemoModel> GetAsync(int id)
        {
            return _ddBroker.ProcessAsync<GetModelQuery, DemoModel>(new GetModelQuery(id));
        }
        [HttpGet("{id}")]
        public DemoModel Get(int id)
        {
            return _ddBroker.Process<GetModelQuery, DemoModel>(new GetModelQuery(id));
        }
    }
}
