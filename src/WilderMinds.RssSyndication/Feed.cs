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
        XNamespace nsAtom = "http://www.w3.org/2005/Atom";
        var doc = new XDocument(new XElement("rss"));
        doc.Root.Add(
                new XAttribute("version", "2.0"), 
                new XAttribute(XNamespace.Xmlns + "atom", "http://www.w3.org/2005/Atom"));

        var channel = new XElement("channel");
      	// ignore if Link is not specified to prevent a NullReferenceException
        if (Link != null)
            channel.Add(
                new XElement(nsAtom + "link",
                new XAttribute("rel", "self"),
                new XAttribute("type","application/rss+xml"),
                new XAttribute("href", Link.AbsoluteUri)));

            channel.Add(new XElement("title", Title));
            if (Link != null) channel.Add(new XElement("link", Link.AbsoluteUri));
            channel.Add(new XElement("description", Description));
            // copyright is not a requirement
            if (!string.IsNullOrEmpty(Copyright)) channel.Add(new XElement("copyright", Copyright));
        
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
            foreach (var key in enclosure.Values.AllKeys)
            {
              enclosureElement.Add(new XAttribute(key, enclosure.Values[key]));
            }
            itemElement.Add(enclosureElement);
          }

        }
        channel.Add(itemElement);
      }

      return doc.ToStringWithDeclaration(option);
    }
  }
}
