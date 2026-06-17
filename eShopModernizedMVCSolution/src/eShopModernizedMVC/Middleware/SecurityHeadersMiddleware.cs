using System.Threading.Tasks;
using Microsoft.Owin;

namespace eShopModernizedMVC.Middleware
{
    public class SecurityHeadersMiddleware : OwinMiddleware
    {
        public SecurityHeadersMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var headers = context.Response.Headers;

            headers.Set("X-Content-Type-Options", "nosniff");
            headers.Set("X-Frame-Options", "DENY");
            headers.Set("Content-Security-Policy", "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'; img-src 'self' data:");
            headers.Set("Referrer-Policy", "strict-origin-when-cross-origin");
            headers.Set("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

            if (context.Request.IsSecure)
            {
                headers.Set("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            }

            await Next.Invoke(context);
        }
    }
}
