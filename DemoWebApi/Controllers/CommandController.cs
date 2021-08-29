using DDMedi;
using DemoWebApi.Handlers;
using DemoWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DemoWebApi.Controllers
{
    public class CommandController : BaseController
    {
        public CommandController(IDDBroker ddBroker) : base(ddBroker) { }
        [HttpPost()]
        public Task<int> Post()
        {
            return _ddBroker.ProcessAsync<InsertModelCommand, int>();
        }
        [HttpPut()]
        public Task Put([FromBody] DemoModel model)
        {
            return _ddBroker.ProcessAsync(new UpdateModelCommand(model.Id, model.Count));
        }
        [HttpDelete("{id}")]
        public Task Delete(int id)
        {
            return _ddBroker.ProcessAsync(new DeleteModelCommand(id));
        }
        [HttpDelete("cache")]
        public void PurgeCachedModels()
        {
            _ddBroker.Process<PurgeCachedModelsCommand>();
        }
    }
}
