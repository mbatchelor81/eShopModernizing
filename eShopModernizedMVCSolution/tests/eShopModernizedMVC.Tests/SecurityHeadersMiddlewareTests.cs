using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using Xunit;

namespace eShopModernizedMVC.Tests
{
    public class SecurityHeadersMiddlewareTests
    {
        private static async Task<IOwinResponse> InvokeMiddleware(string scheme = "http")
        {
            var environment = new Dictionary<string, object>
            {
                ["owin.RequestScheme"] = scheme,
                ["owin.RequestMethod"] = "GET",
                ["owin.RequestPath"] = "/",
                ["owin.RequestPathBase"] = "",
                ["owin.RequestQueryString"] = "",
                ["owin.RequestProtocol"] = "HTTP/1.1",
                ["owin.RequestHeaders"] = new Dictionary<string, string[]>
                {
                    ["Host"] = new[] { "localhost" }
                },
                ["owin.ResponseHeaders"] = new Dictionary<string, string[]>(),
                ["owin.ResponseStatusCode"] = 200,
                ["owin.ResponseBody"] = new System.IO.MemoryStream(),
                ["owin.CallCancelled"] = new System.Threading.CancellationToken()
            };

            var context = new OwinContext(environment);

            var noopNext = new NoopMiddleware();
            var middleware = new Middleware.SecurityHeadersMiddleware(noopNext);
            await middleware.Invoke(context);

            return context.Response;
        }

        [Fact]
        public async Task ApiResponse_ContainsXContentTypeOptions_OWASP_A05()
        {
            var response = await InvokeMiddleware();
            Assert.Equal("nosniff", response.Headers["X-Content-Type-Options"]);
        }

        [Fact]
        public async Task ApiResponse_ContainsXFrameOptions_OWASP_A05()
        {
            var response = await InvokeMiddleware();
            Assert.Equal("DENY", response.Headers["X-Frame-Options"]);
        }

        [Fact]
        public async Task ApiResponse_ContainsContentSecurityPolicy_OWASP_A05()
        {
            var response = await InvokeMiddleware();
            var csp = response.Headers["Content-Security-Policy"];
            Assert.Contains("default-src 'self'", csp);
        }

        [Fact]
        public async Task ApiResponse_ContainsReferrerPolicy_OWASP_A05()
        {
            var response = await InvokeMiddleware();
            Assert.Equal("strict-origin-when-cross-origin", response.Headers["Referrer-Policy"]);
        }

        [Fact]
        public async Task ApiResponse_ContainsPermissionsPolicy_OWASP_A05()
        {
            var response = await InvokeMiddleware();
            Assert.Equal("camera=(), microphone=(), geolocation=()", response.Headers["Permissions-Policy"]);
        }

        [Fact]
        public async Task ApiResponse_ContainsStrictTransportSecurity_WhenHttps_OWASP_A05()
        {
            var response = await InvokeMiddleware(scheme: "https");
            Assert.Equal("max-age=31536000; includeSubDomains", response.Headers["Strict-Transport-Security"]);
        }

        [Fact]
        public async Task ApiResponse_OmitsHsts_WhenHttp_OWASP_A05()
        {
            var response = await InvokeMiddleware(scheme: "http");
            Assert.Null(response.Headers["Strict-Transport-Security"]);
        }

        [Fact]
        public async Task ApiResponse_ContainsAllSixSecurityHeaders_OWASP_A05()
        {
            var response = await InvokeMiddleware(scheme: "https");

            Assert.NotNull(response.Headers["X-Content-Type-Options"]);
            Assert.NotNull(response.Headers["X-Frame-Options"]);
            Assert.NotNull(response.Headers["Strict-Transport-Security"]);
            Assert.NotNull(response.Headers["Content-Security-Policy"]);
            Assert.NotNull(response.Headers["Referrer-Policy"]);
            Assert.NotNull(response.Headers["Permissions-Policy"]);
        }

        private class NoopMiddleware : OwinMiddleware
        {
            public NoopMiddleware() : base(null) { }

            public override Task Invoke(IOwinContext context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
