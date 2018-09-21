using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace VideoStreamer.Controllers
{
    public class VideoOndemandController : ApiController
    {
        public HttpResponseMessage Get(string filename = "AV_60123_20170209104535.stream.mp4")
        {
            string videoFormat = "mp4";
            string videoContentLocation = "C:\\Projects\\content";
            string videoFulllName = videoContentLocation + "\\"+ filename;



            if (!File.Exists(videoFulllName))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var stream = new FileStream(videoFulllName, FileMode.Open, FileAccess.Read, FileShare.Read);

            var mediaType = MediaTypeHeaderValue.Parse($"video/{videoFormat}");

            if (Request.Headers.Range != null)
            {
                try
                {
                    var partialResponse = Request.CreateResponse(HttpStatusCode.PartialContent);
                    partialResponse.Content = new ByteRangeStreamContent(stream, Request.Headers.Range, mediaType);

                    return partialResponse;
                }
                catch (InvalidByteRangeException invalidByteRangeException)
                {
                    return Request.CreateErrorResponse(invalidByteRangeException);
                }
            }
            else
            {
                // If it is not a range request we just send the whole thing as normal
                var fullResponse = Request.CreateResponse(HttpStatusCode.OK);

                fullResponse.Content = new StreamContent(stream);
                fullResponse.Content.Headers.ContentType = mediaType;

                return fullResponse;
            }

        }
    }
}
