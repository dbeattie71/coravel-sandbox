using Coravel.Invocable;
using Coravel.Pro;
using Coravel.Scheduling.Schedule.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        _scheduler.ScheduleWithParams<SomeJob>(1)
            .EveryThirtySeconds();

        return Task.CompletedTask;
    }
}

public class SomeJob : IInvocable, IDoNotAutoRegister
{
    private readonly ILogger<SomeJob> _logger;
    private readonly int _groupNumber;

    public SomeJob(
        ILogger<SomeJob> logger,
        int groupNumber
        )
    {
        _logger = logger;
        _groupNumber = groupNumber;
    }

    public async Task Invoke()
    {
        _logger.LogInformation($"**** SomeJob *** {_groupNumber}");
    }
}