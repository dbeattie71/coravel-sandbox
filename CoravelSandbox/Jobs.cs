using Coravel.Invocable;
using Coravel.Pro;
using Coravel.Scheduling.Schedule.Interfaces;

namespace CoravelSandbox;

public class DynamicScheduler : IInvocable
{
    private readonly ILogger<DynamicScheduler> _logger;
    private readonly IScheduler _scheduler;

    public DynamicScheduler(
        ILogger<DynamicScheduler> logger,
        IScheduler scheduler
    )
    {
        _logger = logger;
        _scheduler = scheduler;
    }

    public Task Invoke()
    {
        _logger.LogInformation("{name} Invoke", nameof(DynamicScheduler));

       

        return Task.CompletedTask;
    }
}

public class SomeJob : IInvocable, IDoNotAutoRegister
{
    private readonly ILogger<SomeJob> _logger;

    public SomeJob(
        ILogger<SomeJob> logger
        )
    {
        _logger = logger;
    }

    public async Task Invoke()
    {
        _logger.LogInformation("**** SomeJob ***");
    }
}