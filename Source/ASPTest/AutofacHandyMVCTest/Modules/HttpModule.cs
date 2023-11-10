using Autofac;

namespace AutofacHandyMVCTest.Modules
{
    public class HttpModule : Module
    {
        public bool IsEnable { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            if (IsEnable)
            {
                builder.Register(ctx =>
                {
                    var services = new ServiceCollection();
                    services.AddHttpClient();
                    var provider = services.BuildServiceProvider();
                    return provider.GetRequiredService<IHttpClientFactory>()
                                    .CreateClient("HomeSubscriberServiceHttpClient");
                }).As<HttpClient>();
            }
        }
    }
}
