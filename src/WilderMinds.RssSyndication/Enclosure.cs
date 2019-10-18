using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace WilderMinds.RssSyndication
{
    public class Enclosure
    {
        public Enclosure()
        {
            Values = new NameValueCollection();
        }
        /// <summary>
        /// Absolute URL to where the enclosure is located
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Size in Bytes
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// standard MIME type
        /// </summary>
        /// <example>audio/mpeg</example>
        public string MimeType { get; set; }

        public NameValueCollection Values { get; set; }
    }
}