using Microsoft.AspNetCore.Mvc;

namespace SimpleMvcSse.Controllers;

[ApiController]
[Route("[controller]")]
public class ValueController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LongRunningService _longRunningService;
    private readonly ILogger<ValueController> _logger;

    public ValueController(IHttpContextAccessor httpContextAccessor, LongRunningService longRunningService, ILogger<ValueController> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _longRunningService = longRunningService;
        _logger = logger;
    }

    [HttpGet("infinitestream")]
    public async Task GetInfiniteStream()
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        response!.Headers.Add("Content-Type", "text/event-stream");
        
        while(true)
        {
            await response.WriteAsync($"data: Hello at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\r");

            await response.Body.FlushAsync();
            await Task.Delay(1 * 2000);
        }
    }

    [HttpGet("feedback")]
    public async Task GetFeedback(string idsStr)
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        response!.Headers.Add("Content-Type", "text/event-stream");

        var newIds = idsStr.Split(',').Select(e => int.Parse(e)).ToList();
        await foreach (var result in _longRunningService.Schedule(newIds))
        {
            await response.WriteAsync($"data: Processed {result.Id}: {result.Result.ToString()} \r\r");
            await response.Body.FlushAsync();
        }

        await response.WriteAsync("data: Completed\r\r");
        await response.Body.FlushAsync();
    }
}