﻿namespace Unosquare.Labs.EmbedIO.Tests
{
    using NUnit.Framework;
    using Swan.Formatters;
    using TestObjects;
    using System.Threading.Tasks;
    using Modules;

    public class IWebServerTest
    {
        [Test]
        public void SetupInMemoryWebServer_ReturnsValidInstance()
        {
            Assert.IsNotNull(new WebServer());
        }

        [Test]
        public void RegisterWebModule_ReturnsValidInstance()
        {
            var webserver = new TestWebServer();
            webserver.RegisterModule(new FallbackModule((ctx, ct) => ctx.JsonResponse(nameof(TestWebServer))));

            Assert.AreEqual(1, webserver.Modules.Count);
        }

        [Test]
        public void UnregisterWebModule_ReturnsValidInstance()
        {
            var webserver = new TestWebServer();
            webserver.RegisterModule(new FallbackModule((ctx, ct) => ctx.JsonResponse(nameof(TestWebServer))));
            webserver.UnregisterModule(typeof(FallbackModule));

            Assert.AreEqual(0, webserver.Modules.Count);
        }

        [Test]
        public void RegisterSessionModule_ReturnsValidInstance()
        {
            var webserver = new TestWebServer();
            webserver.RegisterModule(new LocalSessionModule());

            Assert.NotNull(webserver.SessionModule);
        }

        [Test]
        public void UnregisterSessionModule_ReturnsValidInstance()
        {
            var webserver = new TestWebServer();
            webserver.RegisterModule(new LocalSessionModule());
            webserver.UnregisterModule(typeof(LocalSessionModule));

            Assert.IsNull(webserver.SessionModule);
        }

        [Test]
        public async Task RunsServerAndRequestData_ReturnsValidData()
        {
            var webserver = new TestWebServer();
            webserver.OnAny((ctx, ct) => ctx.JsonResponse(new Person { Name = nameof(Person) }));

            webserver.RunAsync();

            var client = webserver.GetClient();

            var data = await client.GetAsync("/");
            Assert.IsNotNull(data);

            var person = Json.Deserialize<Person>(data);
            Assert.IsNotNull(person);

            Assert.AreEqual(person.Name, nameof(Person));
        }
    }
}