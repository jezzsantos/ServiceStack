using System;
using System.Web;
using ServiceStack.Text;
using ServiceStack.Web;

namespace ServiceStack.Host
{
    public class RequestPreferences : IRequestPreferences
    {
        private readonly HttpContextBase httpContext;

        public RequestPreferences(IRequest httpRequest)
        {
            this.acceptEncoding = httpRequest.Headers[HttpHeaders.AcceptEncoding];
            if (this.acceptEncoding.IsNullOrEmpty())
            {
                this.acceptEncoding = "none";
                return;
            }
            this.acceptEncoding = this.acceptEncoding.ToLower();
        }

        public RequestPreferences(HttpContextBase httpContext)
        {
            this.httpContext = httpContext;
            this.acceptEncoding = httpContext.Request.Headers[HttpHeaders.AcceptEncoding];
            if (this.acceptEncoding.IsNullOrEmpty())
            {
                this.acceptEncoding = "none";
                return;
            }
            this.acceptEncoding = this.acceptEncoding.ToLower();
        }

        public static HttpWorkerRequest GetWorker(HttpContextBase context)
        {
            var provider = (IServiceProvider)context;
            var worker = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
            return worker;
        }

        private HttpWorkerRequest httpWorkerRequest;
        private HttpWorkerRequest HttpWorkerRequest => 
            this.httpWorkerRequest ?? (this.httpWorkerRequest = GetWorker(this.httpContext));

        private string acceptEncoding;
        public string AcceptEncoding
        {
            get
            {
                if (acceptEncoding != null)
                    return acceptEncoding;
                if (Env.IsMono)
                    return acceptEncoding;

                acceptEncoding = HttpWorkerRequest.GetKnownRequestHeader(HttpWorkerRequest.HeaderAcceptEncoding)?.ToLower();
                return acceptEncoding;
            }
        }

        public bool AcceptsGzip => AcceptEncoding != null && AcceptEncoding.Contains("gzip");

        public bool AcceptsDeflate => AcceptEncoding != null && AcceptEncoding.Contains("deflate");
    }
}