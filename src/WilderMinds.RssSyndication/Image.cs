using System;
using System.Collections.Generic;
using System.Text;

namespace WilderMinds.RssSyndication
{
    public class Image
    {
        public Image(Uri url, string title, Uri link)
        {
            Url = url;
            Title = title;
            Link = link;
        }

        public Uri Url { get; }
        public string Title { get; }
        public Uri Link { get; }
    }
}
