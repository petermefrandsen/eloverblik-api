using Eloverblik_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eloverblik_API;

[Route("api/[controller]")]
[ApiController]
public class EloverblikController : ControllerBase
{
    private readonly ILogger<EloverblikController> _logger;
    private readonly IEloverblikService _eloverblikService;
    public EloverblikController(ILogger<EloverblikController> logger, IEloverblikService eloverblikService)
    {
        _logger = logger;
        _eloverblikService = eloverblikService;
    }

    [Route("GetAccessToken")]
    [HttpGet]
    public async Task<StringApiResponse> GetAccessToken()
    {
        return await _eloverblikService.TokenAsync();
    }

    [Route("GetAndSetAccessToken")]
    [HttpGet]
    public async Task<bool> GetAndSetAccessToken()
    {
        var accessToken = await _eloverblikService.TokenAsync();
        Environment.SetEnvironmentVariable("eloverblikApiAccessToken", accessToken.Result);
        return true;
    }

    [Route("GetMeteringPoints")]
    [HttpGet]
    public async Task<MeteringPointApiDtoListApiResponse> GetMeteringPoints(bool includeAll = true)
    {
        return await _eloverblikService.MeteringpointsAsync(includeAll);
    }
}