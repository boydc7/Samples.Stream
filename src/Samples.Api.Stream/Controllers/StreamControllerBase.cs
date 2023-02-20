using Microsoft.AspNetCore.Mvc;

namespace Samples.Api.Stream.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "stream")]
public abstract class StreamControllerBase : ControllerBase { }
