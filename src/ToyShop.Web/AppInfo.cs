using System.Reflection;

namespace ToyShop.Web;

public static class AppInfo
{
    /// <summary>Версия сборки. В CI/CD подставляется номер запуска пайплайна.</summary>
    public static string Version { get; } = ReadVersion();

    private static string ReadVersion()
    {
        var attribute = typeof(AppInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute is null)
        {
            return "dev";
        }

        // .NET добавляет к версии хеш коммита после «+», он нам не нужен.
        return attribute.InformationalVersion.Split('+')[0];
    }
}
