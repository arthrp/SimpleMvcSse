using System.Collections.Concurrent;
using System.Globalization;

namespace SimpleMvcSse;

public enum Result
{
    Ok,
    Error
}

public record InputResult(int Id, Result Result, bool IsFinal);

public class LongRunningService
{
    public async IAsyncEnumerable<InputResult> Schedule(List<int> inputs)
    {
        foreach (var input in inputs)
        {
            await Task.Delay(1 * 2000);
            yield return new InputResult(input, Result.Ok, false);
        }
    }
}