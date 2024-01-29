using System.IdentityModel.Tokens.Jwt;
using Eloverblik_API.Repositories;
using Eloverblik_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eloverblik_API;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly IEloverblikService _eloverblikService;
    private readonly IBlobRepository _blobRepository;

    public HealthController(ILogger<HealthController> logger, IEloverblikService eloverblikService, IBlobRepository blobRepository)
    {
        _logger = logger;
        _blobRepository = blobRepository;
        _eloverblikService = eloverblikService;
    }

    [Route("Health")]
    [HttpGet]
    public async Task<Status> GetHealth()
    {
        var accessToken = Environment.GetEnvironmentVariable("eloverblikApiAccessToken");
        var elOverblikAccessTokenValid = "not set";
        if(!string.IsNullOrEmpty(accessToken))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(accessToken);
            elOverblikAccessTokenValid = jwtSecurityToken.ValidTo > DateTime.UtcNow ? "valid" : "invalid";
        }

        return new Status
        {
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            BlobConnectionIsAlive = await _blobRepository.TestBlobStorageConnection(),
            ElOverblikIsAlive = await _eloverblikService.IsaliveAsync(),
            ElOverblikAccessTokenValid = elOverblikAccessTokenValid
        };
    }
}