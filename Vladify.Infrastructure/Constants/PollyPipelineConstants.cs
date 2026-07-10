namespace Vladify.Infrastructure.Constants;

public static class PollyPipelineConstants
{
    public const string DbRetryPipelineName = "DbRetryPipeline";

    public const int MaxAmountOfRetries = 3;

    public static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);
}
