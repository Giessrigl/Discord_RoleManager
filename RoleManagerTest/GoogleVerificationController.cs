using Microsoft.AspNetCore.Mvc;

namespace RoleManagerTest
{
    public class GoogleVerificationController : ControllerBase
    {
        [HttpGet("/google711ad4814e8a6cc0.html")]
        public async Task<ContentResult> Index()
        {
            var html = "";
            var dir = Directory.GetCurrentDirectory();
            using (var reader = new StreamReader(Path.Combine(dir, "google711ad4814e8a6cc0.html")))
            {
                html = await reader.ReadToEndAsync();
            }

            return new ContentResult
            {
                Content = html,
                ContentType = "text/html"
            };
        }
    }
}
