using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Comakerships_Api.Security
{
    /// <summary>
    /// Whitelist of allowed files for deliverable fileupload.
    /// </summary>
    public class RestrictiveMultipartMemoryStreamProvider : MultipartMemoryStreamProvider
    {
        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            // currently only allowing the following extensions
            var extensions = new[] { "pdf", "doc", "docx", "zip", "rar", "png", "jpg", "jpeg", "odt", "xls", "xlsx", "ppt", "pptx", "txt"};
            var filename = headers.ContentDisposition.FileName.Replace("\"", string.Empty);

            if (filename.IndexOf('.') < 0)
                return Stream.Null;

            var extension = filename.Split('.').Last();

            return extensions.Any(i => i.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                       ? base.GetStream(parent, headers)
                       : Stream.Null;
        }
    }
}
