using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace CompoundUI.TestEndpoint.Controllers
{
    public class TextPlainFormatter : BufferedMediaTypeFormatter
    {
        public TextPlainFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var writer = new StreamWriter(writeStream);
            writer.Write(value);
            writer.Flush();
        }
    }
}