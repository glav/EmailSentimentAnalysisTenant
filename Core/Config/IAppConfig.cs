namespace Core.Config
{
    public interface IAppConfig
    {
        bool IsHostedInAzure { get; }
    }
}