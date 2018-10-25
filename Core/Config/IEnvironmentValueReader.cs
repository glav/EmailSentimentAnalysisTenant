namespace Core.Config
{
    public interface IEnvironmentValueReader
    {
        string GetEnvironmentValueThatIsNotEmpty(string[] environmentVariables);
    }
}