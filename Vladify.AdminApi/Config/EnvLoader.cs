namespace Vladify.AdminApi.Config;

public static class EnvLoader
{
    public static void LoadEnvVariables()
    {
        DotNetEnv.Env.TraversePath().Load();
    }
}
