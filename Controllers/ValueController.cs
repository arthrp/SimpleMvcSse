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
    public async Task GetFeedback()
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        response!.Headers.Add("Content-Type", "text/event-stream");

        var ids = new List<int>() {10, 20, 666};
        await foreach (var result in _longRunningService.Schedule(ids))
        {
            await response.WriteAsync($"data: {result.Id}: {result.Result.ToString()} \r\r");

            await response.Body.FlushAsync();
        }

        await response.WriteAsync("data: Completed");
        await response.Body.FlushAsync();
        await Task.Delay(100);
    }
}