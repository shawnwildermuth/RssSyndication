using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace WilderMinds.RssSyndication
{
    public static class XDocumentExtensions
    {
        public static string ToStringWithDeclaration(this XDocument what)
        {
            if (what == null)
                throw new ArgumentNullException(nameof(what));

            var builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
                what.Save(writer);
            return builder.ToString();
        }
    }
}