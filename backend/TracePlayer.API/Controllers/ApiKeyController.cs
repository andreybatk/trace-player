using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TracePlayer.BL.Services.Api;

namespace TracePlayer.API.Controllers
{
    [Route("api/api-key")]
    [ApiController]
    [Authorize("Admin")]
    public class ApiKeyController : ControllerBase
    {
        private readonly ApiKeyService _apiKeyService;

        public ApiKeyController(ApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddApiKey([FromQuery] string serverIp)
        {
            var apiKey = await _apiKeyService.GenerateAndSaveApiKey(serverIp);

            if(apiKey is null)
            {
                return BadRequest("Failed to adding key to ServerIp or ServerIp already exists.");
            }

            return Ok(apiKey);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var result = await _apiKeyService.GetAllServerIps();

            return Ok(result);
        }

        [HttpDelete("{serverIp}")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string serverIp)
        {
            var result = await _apiKeyService.DeleteApiKeyByServerIp(serverIp);

            if (!result)
            {
                return BadRequest("Failed to delete api key by ServerIp or ServerIp already delete.");
            }

            return Ok(result);
        }
    }
}