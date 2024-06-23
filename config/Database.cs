using DotNetEnv;

public static class Config
{
   public static string ConnectionString { get; } = $"Host={Env.GetString("DATABASE_HOST")};Database={Env.GetString("DATABASE_NAME")};Username={Env.GetString("DATABASE_USER")};Password={Env.GetString("DATABASE_PASSWORD")}";
}