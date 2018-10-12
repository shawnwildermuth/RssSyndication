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

    public NameValueCollection Values { get; set; }
  }
}