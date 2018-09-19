using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OfflineExamSystem.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal sealed class LocalizedAuthorizeAttribute : AuthorizeAttribute
    {
        #region Protected Methods
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string language = filterContext.RouteData.Values["lang"] == null ? CultureHelper.GetCurrentCulture() : filterContext.RouteData.Values["lang"].ToString();
            filterContext.Result =
            new RedirectResult(string.Format("~/{0}/account/login?returnUrl={1}", language, HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.PathAndQuery)));
            //base.HandleUnauthorizedRequest(filterContext);
        }
        #endregion Protected Methods
    }
    public class CompressAttribute : ActionFilterAttribute
    {
        #region Public Methods
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool allowCompression = false;
            bool.TryParse(ConfigurationManager.AppSettings["Compression"], out allowCompression);

            if (allowCompression)
            {
                HttpRequestBase request = filterContext.HttpContext.Request;

                string acceptEncoding = request.Headers["Accept-Encoding"];

                if (string.IsNullOrEmpty(acceptEncoding)) return;

                acceptEncoding = acceptEncoding.ToUpperInvariant();

                HttpResponseBase response = filterContext.HttpContext.Response;

                if (acceptEncoding.Contains("GZIP"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("DEFLATE"))
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }
        }
        #endregion Public Methods
    }
    public class WhitespaceAttribute : ActionFilterAttribute
    {
        #region Public Methods
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            // If it's a sitemap, just return.
            if (filterContext.HttpContext.Request.RawUrl == "/sitemap.xml") return;

            if (response.ContentType != "text/html" || response.Filter == null) return;

            response.Filter = new HelperClass(response.Filter);
        }
        #endregion Public Methods

        #region Private Classes
        private class HelperClass : Stream
        {
            #region Private Fields
            private readonly Stream _base;
            private StringBuilder _s = new StringBuilder();
            #endregion Private Fields

            #region Public Constructors
            public HelperClass(Stream responseStream)
            {
                if (responseStream == null)
                    throw new ArgumentNullException("responseStream");
                _base = responseStream;
            }
            #endregion Public Constructors

            #region Public Properties
            public override bool CanRead { get { return false; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return true; } }
            public override long Length { get { throw new NotSupportedException(); } }
            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }
            #endregion Public Properties

            #region Public Methods
            public override void Write(byte[] buffer, int offset, int count)
            {
                var html = Encoding.UTF8.GetString(buffer, offset, count);
                var reg = new Regex(@"(?<=\s)\s+(?![^<>]*</pre>)");
                html = reg.Replace(html, string.Empty);

                buffer = Encoding.UTF8.GetBytes(html);
                _base.Write(buffer, 0, buffer.Length);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
            public override void Flush()
            {
                _base.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }
            #endregion Public Methods
        }
        #endregion Private Classes
    }
    public class ETagAttribute : ActionFilterAttribute
    {
        #region Public Methods
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Filter = new ETagFilter(filterContext.HttpContext.Response, filterContext.RequestContext.HttpContext.Request);
        }
        #endregion Public Methods
    }
    public class ETagFilter : MemoryStream
    {
        #region Private Fields
        private HttpResponseBase _response = null;
        private HttpRequestBase _request;
        private Stream _filter = null;
        #endregion Private Fields

        #region Public Constructors
        public ETagFilter(HttpResponseBase response, HttpRequestBase request)
        {
            _response = response;
            _request = request;
            _filter = response.Filter;
        }
        #endregion Public Constructors

        #region Private Methods
        private string GetToken(Stream stream)
        {
            var checksum = new byte[0];
            checksum = System.Security.Cryptography.MD5.Create().ComputeHash(stream);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }
        #endregion Private Methods

        #region Public Methods
        public override void Write(byte[] buffer, int offset, int count)
        {
            var data = new byte[count];

            Buffer.BlockCopy(buffer, offset, data, 0, count);

            var token = GetToken(new MemoryStream(data));
            var clientToken = _request.Headers["If-None-Match"];

            if (token != clientToken)
            {
                _response.AddHeader("ETag", token);
                _filter.Write(data, 0, count);
            }
            else
            {
                _response.SuppressContent = true;
                _response.StatusCode = 304;
                _response.StatusDescription = "Not Modified";
                _response.AddHeader("Content-Length", "0");
            }
        }
        #endregion Public Methods
    }
}