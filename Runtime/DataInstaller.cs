namespace MK.Data
{
    using MK.DependencyInjection;

    public static class DataInstaller
    {
        public static void InstallData(this IBuilder builder)
        {
            builder.Register<CsvSeparator>(Lifetime.Singleton);
            builder.Register<ConverterResolver>(Lifetime.Singleton);
        }
    }
}