using Microsoft.Extensions.Logging;
using WireMock.Server;
using Xunit.Abstractions;

namespace LocalAI.NET.GPT4All.Tests.Common
{
    public abstract class TestBase : IDisposable
    {
        protected readonly WireMockServer Server;
        protected readonly ILogger Logger;
        protected readonly string BaseUrl;

        protected TestBase(ITestOutputHelper output)
        {
            Server = WireMockServer.Start();
            BaseUrl = Server.Urls[0];
            Logger = LoggerFactory
                .Create(builder => builder.AddXUnit(output))
                .CreateLogger(GetType());
        }

        public virtual void Dispose()
        {
            Server.Dispose();
        }
    }
}