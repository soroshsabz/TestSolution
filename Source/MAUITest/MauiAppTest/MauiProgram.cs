using Microsoft.Extensions.Logging;

namespace MauiAppTest
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            IIAM iam = new IAM();
            iam.DoSomethingElse();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}