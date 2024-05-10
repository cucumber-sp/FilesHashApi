using FilesHashApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilesHashApi.Controllers;

[ApiController]
[Route("api/hashes")]
public class HashesController(IFileHashProviderService hashProviderService) : ControllerBase
{
    [HttpGet("md5")]
    public async Task<IActionResult> GetMd5Hash([FromQuery] string file)
    {
        string hash = await hashProviderService.GetMd5HashAsync(file);
        return Ok(hash);
    }
}