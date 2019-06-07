using System.Collections.Specialized;
using Sitecore.ContentTagging.Core.Models;

namespace Sitecore.ContentTagging.TextAnalytics.Models
{
    public class EntitiesTagData : TagData
    {
        public NameValueCollection Entities { get; set; }
    }
}