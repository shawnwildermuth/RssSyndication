using System;
using System.Collections.Generic;
using System.Text;

namespace WilderMinds.RssSyndication
{
    public class Image
    {
        public Image(Uri url, string title, Uri link)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Link = link ?? throw new ArgumentNullException(nameof(link));
        }

        /// <summary>The URL of a GIF, JPEG or PNG image that represents the channel.</summary>
        public Uri Url { get; }

        /// <summary>
        /// Image description used in ALT attribute of the HTML image tag when the channel is rendered in HTML.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The URL of the site. When the channel is rendered, the image is a link to the site.
        /// </summary>
        public Uri Link { get; }
    }
}
