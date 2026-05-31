using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/HandleErrorCode/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            Response.StatusCode = statusCode;

            if (statusCode == 404)
            {
                return View("NotFound");
            }

            return View("Error", statusCode);
        }

        public new IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }


    }


}
