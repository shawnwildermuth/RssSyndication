using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WilderMinds.RssSyndication
{
  /// <summary>Feed object which maps to 'channel' property on Feed.Serialize()</summary>
  public class Feed
  {
    public string Description { get; set; }
    public Uri Link { get; set; }
    public string Title { get; set; }
    public string Copyright { get; set; }

    /// <summary>Optional element specifying an image that can be displayed with the channel.</summary>
    public Image Image { get; set; }

    /// <summary>
    /// ISO-639 language codes.
    /// </summary>
    /// <example>en</example>
    /// <remarks>https://www.loc.gov/standards/iso639-2/php/code_list.php</remarks>
    public string Language { get; set; } = "en";

    public ICollection<Item> Items { get; set; } = new List<Item>();

    /// <summary>Produces well-formatted rss-compatible xml string.</summary>
    public string Serialize()
    {

      var defaultOption = new SerializeOption()
      {
        Encoding = Encoding.Unicode
      };
      return Serialize(defaultOption);
    }

    /// <summary>Produces well-formatted rss-compatible xml string.</summary>
    public string Serialize(SerializeOption option)
    {
      var contentNamespaceUrl = "http://purl.org/rss/1.0/modules/content/";

      XNamespace nsAtom = "http://www.w3.org/2005/Atom";
      var doc = new XDocument(new XElement("rss"));

      doc.Root.Add(
              new XAttribute("version", "2.0"),
              new XAttribute(XNamespace.Xmlns + "atom", "http://www.w3.org/2005/Atom"));

      //namespace for Facebook's xmlns:content full article content area
      doc.Root.Add(new XAttribute(XNamespace.Xmlns + "content", contentNamespaceUrl));

      var channel = new XElement("channel");
      // ignore if Link is not specified to prevent a NullReferenceException
      if (Link != null)
        channel.Add(
            new XElement(nsAtom + "link",
            new XAttribute("rel", "self"),
            new XAttribute("type", "application/rss+xml"),
            new XAttribute("href", Link.AbsoluteUri)));

      channel.Add(new XElement("title", Title));
      if (Link != null) channel.Add(new XElement("link", Link.AbsoluteUri));
      channel.Add(new XElement("description", Description));
      // copyright is not a requirement
      if (!string.IsNullOrEmpty(Copyright)) channel.Add(new XElement("copyright", Copyright));

      channel.Add(new XElement("language", Language));

      if (Image != null)
      {
          AddImageElement(channel);
      }

      doc.Root.Add(channel);

      foreach (var item in Items)
      {
        var itemElement = new XElement("item");

        itemElement.Add(new XElement("title", item.Title));

        if (item.Link != null) itemElement.Add(new XElement("link", item.Link.AbsoluteUri));

        itemElement.Add(new XElement("description", item.Body));

        if (item.Author != null) itemElement.Add(new XElement("author", $"{item.Author.Email} ({item.Author.Name})"));

        foreach (var c in item.Categories) itemElement.Add(new XElement("category", c));

        if (item.Comments != null) itemElement.Add(new XElement("comments", item.Comments.AbsoluteUri));

        if (!string.IsNullOrWhiteSpace(item.Permalink)) itemElement.Add(new XElement("guid", item.Permalink));

        var dateFmt = item.PublishDate.ToString("r");
        if (item.PublishDate != DateTime.MinValue) itemElement.Add(new XElement("pubDate", dateFmt));

        if (item.Enclosures != null && item.Enclosures.Any())
        {
          foreach (var enclosure in item.Enclosures)
          {
            var enclosureElement = new XElement("enclosure");
            if (enclosure.Length > 0)
            {
              enclosureElement.Add(new XAttribute("length", enclosure.Length));
            }

            if (enclosure.Url != null)
            {
              enclosureElement.Add(new XAttribute("url", enclosure.Url.AbsoluteUri));
            }

            if (!string.IsNullOrWhiteSpace(enclosure.MimeType))
            {
              enclosureElement.Add(new XAttribute("type", enclosure.MimeType.Trim()));
            }

            foreach (var key in enclosure.Values.AllKeys)
            {
              enclosureElement.Add(new XAttribute(key, enclosure.Values[key]));
            }
            itemElement.Add(enclosureElement);
          }
        }

        if (!string.IsNullOrWhiteSpace(item.FullHtmlContent))
        {
          //add content:encoded element, CData escaped html
          var ns = XNamespace.Get(contentNamespaceUrl);
          var html = new XElement(ns + "encoded", new XCData(item.FullHtmlContent));
          itemElement.Add(html);
          html.ReplaceNodes(new XCData(item.FullHtmlContent));
        }

        channel.Add(itemElement);
      }

      return doc.ToStringWithDeclaration(option);
    }

    private void AddImageElement(XElement channel)
    {
        var imageElement = new XElement("image");
        imageElement.Add(new XElement("url", Image.Url.AbsoluteUri));
        imageElement.Add(new XElement("title", Image.Title));
        imageElement.Add(new XElement("link", Image.Link.AbsoluteUri));
        channel.Add(imageElement);
    }
  }
}