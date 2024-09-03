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

        [HttpGet]
        public IActionResult BufferOverflow(string x)
        {
            // This would be a buffer overflow if it weren't for 
            char[] s = new char[256];
            for(int i = 0; i < x.Length; i++)
            {
                s[i] = x[i];
            }
            return Ok();
        }

    }
}
