using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TestWebApi.Controllers
{
    public class LinkedInData
    {
        public string GrantType { get; set; }
        public string Code { get; set; }
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LinkedInProxyController : ControllerBase
    {
        private readonly ILogger<LinkedInProxyController> log;

        public LinkedInProxyController(ILogger<LinkedInProxyController> log)
        {
            this.log = log;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]LinkedInData data)
        {
            var uri = new Uri("https://www.linkedin.com/oauth/v2/accessToken");
            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                {"grant_type", data.GrantType},
                {"code", data.Code},
                {"redirect_uri", data.RedirectUri},
                {"client_id", data.ClientId},
                {"client_secret", data.ClientSecret},
            });

            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage result = await client.PostAsync(uri, content))
                using (HttpContent response = result.Content)
                {
                    return Ok(JsonConvert.DeserializeObject<object>(await response.ReadAsStringAsync()));
                }

            } catch (Exception e)
            {
                log.LogDebug(e, "That's an error!");
                return BadRequest();
            }
        }
    }
}
