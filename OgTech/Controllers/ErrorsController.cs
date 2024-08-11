using Microsoft.AspNetCore.Mvc;
using OgTech.Api.Errors;

namespace OgTech.Api.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            return code switch
            {
                400 => BadRequest(new ApiResponse(400)),
                401 => Unauthorized(new ApiResponse(401)),
                404 => NotFound(new ApiResponse(404)),
                _ => StatusCode(code)
            };
        }
    }
}
