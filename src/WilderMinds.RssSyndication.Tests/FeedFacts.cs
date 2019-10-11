using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WilderMinds.RssSyndication;
using Xunit;

namespace RssSyndication.Tests
{
    public class FeedFacts
    {

        Feed CreateTestFeed()
        {
            var feed = new Feed
            {
                Title = "Shawn Wildermuth's Blog",
                Description = "My Favorite Rants and Raves",
                Link = new Uri("http://wildermuth.com/feed"),
                Copyright = "(c) 2016"
            };

            var item1 = new Item
            {
                Title = "Foo Bar",
                Body = "<p>Foo bar</p>",
                Link = new Uri("http://foobar.com/item#1"),
                Permalink = "http://foobar.com/item#1",
                PublishDate = DateTime.UtcNow,
                Author = new Author { Name = "Shawn Wildermuth", Email = "shawn@wildermuth.com" }
            };

            item1.Categories.Add("aspnet");
            item1.Categories.Add("foobar");

            item1.Comments = new Uri("http://foobar.com/item1#comments");

            feed.Items.Add(item1);

            var item2 = new Item
            {
                Title = "Quux",
                Body = "<p>Quux</p>",
                Link = new Uri("http://quux.com/item#1"),
                Permalink = "http://quux.com/item#1",
                PublishDate = DateTime.UtcNow,
                Author = new Author { Name = "Shawn Wildermuth", Email = "shawn@wildermuth.com" }
            };

            item1.Categories.Add("aspnet");
            item1.Categories.Add("quux");

            feed.Items.Add(item2);

            return feed;
        }

        [Fact]
        public void CreatesValidRss()
        {
            var feed = CreateTestFeed();

            var rss = feed.Serialize();
            Debug.Write(rss);
            var doc = XDocument.Parse(rss);

            Assert.NotNull(doc);
            var item = doc.Descendants("item").FirstOrDefault();
            Assert.NotNull(item);
            Assert.True(item.Element("title").Value == "Foo Bar", "First Item was correct");
        }

        [Fact]
        public void DatesAreProperlyFormatted()
        {
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            var feed = CreateTestFeed();
            var rss = feed.Serialize();
            var doc = XDocument.Parse(rss);
            var pubDate = doc.Descendants("pubDate").First();

            var rfc822FormattedDate = feed.Items.First().PublishDate.ToString("r", CultureInfo.InvariantCulture);
            Assert.Equal(rfc822FormattedDate, pubDate.Value);
        }

        [Fact]
        public void FeedAddsItems()
        {
            var feed = CreateTestFeed();

            Assert.NotNull(feed.Items.First());
            Assert.True(feed.Items.First().Title == "Foo Bar");
            Assert.True(feed.Items.ElementAt(1).Title == "Quux");
            Assert.True(feed.Items.First().Author.Name == "Shawn Wildermuth");
        }

        [Fact]
        public void FeedIsCreated()
        {
            var feed = new Feed
            {
                Title = "Shawn Wildermuth's Blog",
                Description = "My Favorite Rants and Raves",
                Link = new Uri("http://wildermuth.com/feed"),
                Copyright = "(c) 2016"
            };

            Assert.NotNull(feed);
            Assert.True(feed.Title == "Shawn Wildermuth's Blog");
            Assert.True(feed.Description == "My Favorite Rants and Raves");
            Assert.True(feed.Link == new Uri("http://wildermuth.com/feed"));
            Assert.True(feed.Copyright == "(c) 2016");
        }

        [Fact]
        public void GeneratedXmlContainsDeclaration()
        {
            var feed = CreateTestFeed();
            var rss = feed.Serialize();
            Assert.StartsWith("<?xml version", rss);
        }

        [Fact]
        public void SerializedXmlHasContentNamespace()
        {
            var feed = CreateTestFeed();

            feed.Items.Clear();
            feed.Items.Add(new Item()
            {
                Title = "fake",
                FullHtmlContent = "<header><h1>article title</h1></header><main><p>body with &lt; some html characters and some neat@no.com symbols.</p></main><footer>&copy; 2019</footer>",

                Body = "<p>Foo bar</p>",
                Link = new Uri("http://foobar.com/item#1"),
                Permalink = "http://foobar.com/item#1",
                PublishDate = DateTime.UtcNow,
                Author = new Author { Name = "Dirk Watkins", Email = "ya@right.dev" }
            });

            var rss = feed.Serialize();

            Assert.Contains("xmlns:content=\"http://purl.org/rss/1.0/modules/content/\"", rss, StringComparison.OrdinalIgnoreCase);
        }


        [Fact]
        public void HtmlContentIsEnclosedInCData()
        {
            var feed = CreateTestFeed();

            feed.Items.Clear();
            feed.Items.Add(new Item()
            {
                Title = "fake",
                FullHtmlContent = "<header><h1>article title</h1></header><main><p>body with &lt; some html characters and some neat@no.com symbols.</p></main><footer>&copy; 2019</footer>",

                Body = "<p>Foo bar</p>",
                Link = new Uri("http://foobar.com/item#1"),
                Permalink = "http://foobar.com/item#1",
                PublishDate = DateTime.UtcNow,
                Author = new Author { Name = "Dirk Watkins", Email = "ya@right.dev" }
            });

            var rss = feed.Serialize();

            var doc = XDocument.Parse(rss);



            var content = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "encoded");

            Assert.NotNull(content);

            Assert.True(content.FirstNode.NodeType == System.Xml.XmlNodeType.CDATA);

            Assert.True(content.FirstNode.ToString().StartsWith("<![CDATA["), "HTML content needs to start with <![CDATA[");
        }


        [Fact]
        public void HtmlContentIsEnclosedInCData_Check2()
        {
            var feed = CreateTestFeed();

            feed.Items.Clear();
            feed.Items.Add(new Item()
            {
                Title = "fake",
                FullHtmlContent = "<section></section>",

                Body = "<p>Foo bar</p>",
                Link = new Uri("http://foobar.com/item#1"),
                Permalink = "http://foobar.com/item#1",
                PublishDate = DateTime.UtcNow,
                Author = new Author { Name = "Dirk Watkins", Email = "ya@right.dev" }
            });

            var rss = feed.Serialize();

            var doc = XDocument.Parse(rss);


            var content = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "encoded");

            Assert.Equal("<![CDATA[<section></section>]]>", content.FirstNode.ToString(), ignoreCase: true, ignoreLineEndingDifferences: true);

            Assert.True(content.FirstNode.ToString().StartsWith("<![CDATA["), "HTML content needs to start with <![CDATA[");


            Assert.True(content.FirstNode.ToString().EndsWith("]]>"), "HTML content needs to end with ]]>");
        }

        [Fact]
        public void HtmlContentIsNotEscaped()
        {
            var feed = CreateTestFeed();

            feed.Items.Clear();
            feed.Items.Add(new Item()
            {
                Title = "fake",
                FullHtmlContent = "&copy;",

                Body = "<p>Foo bar</p>",
                Link = new Uri("http://foobar.com/item#1"),
                Permalink = "http://foobar.com/item#1",
                PublishDate = DateTime.UtcNow,
                Author = new Author { Name = "Dirk Watkins", Email = "ya@right.dev" }
            });

            var rss = feed.Serialize();

            Assert.Contains("xmlns:content=\"http://purl.org/rss/1.0/modules/content/\"", rss, StringComparison.OrdinalIgnoreCase);

            var doc = XDocument.Parse(rss);


            var content = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "encoded");

            Assert.NotNull(content);

            Assert.DoesNotContain("&amp;", content.Value, StringComparison.OrdinalIgnoreCase);
        }
    }

}