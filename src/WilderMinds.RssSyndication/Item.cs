using System;
using System.Collections.Generic;

namespace WilderMinds.RssSyndication
{
  public class Item
  {
    public Author Author { get; set; }

    public string Body { get; set; }

    public ICollection<string> Categories { get; set; } = new List<string>();

    public Uri Comments { get; set; }

    public Uri Link { get; set; }

    /// <summary>Maps to 'guid' property on Feed.Serialize()</summary>
    public string Permalink { get; set; }

    /// <summary>Maps to 'pubDate' property on Feed.Serialize()</summary>
    public DateTime PublishDate { get; set; }

    public string Title { get; set; }

    /// <summary>
    /// The full content of your article, in HTML form.
    /// Mainly for Facebook feeds.  Insert the entire HTML content here.  It will be escaped in a CDATA section when serialized.
    /// </summary>
    public string FullHtmlContent { get; set; }

    /// <summary>
    /// A string that provides a unique identifier for this article in your feed.
    /// </summary>
    public string Guid { get; set; }

    /// <summary>
    /// use this tag to add a media element that will be used in layout view to illustrate your article. 
    /// It can be an image or a video. For videos, mobile-friendly mp4 format is strongly preferred. 
    /// For images, prefer a high-resolution image; the smallest dimension should not be under 500px.
    /// </summary>
    public ICollection<Enclosure> Enclosures { get; set; } = new List<Enclosure>();
  }
}