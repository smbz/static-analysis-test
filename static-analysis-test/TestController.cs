using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace static_analysis_test
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet]
        public IActionResult Index(string id)
        {
            // Should get error about possible out-of-bounds string access
            if (id[0] == 'A')
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult XsrfVulnerability(string x)
        {
            // Textbook XSRF/CSRF vulnerability
            var authCookie = Request.Cookies["auth"];
            if (authCookie != "some_auth_value")
            {
                return Forbid();
            }
            ProtectedAction(x);
            return Ok();
        }

        private void ProtectedAction(string arg)
        {
            // Something with a side-effect
            System.IO.File.WriteAllBytes("C:/test.xyz", Encoding.UTF8.GetBytes(arg));
        }

    }
}
