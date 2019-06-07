using Sitecore.ContentTagging.Core.Messaging;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.ContentTagging.TextAnalytics.Extensions;
using Sitecore.ContentTagging.TextAnalytics.Models;
using Sitecore.ContentTagging.TextAnalytics.Templates;
using Sitecore.Data.Items;

namespace Sitecore.ContentTagging.TextAnalytics.Providers
{
    public class TextAnalyticsContentProvider: MessageSource, IContentProvider<Item>
    {
        public TaggableContent GetContent(Item source)
        {
            if (!source.IsDerived(__TextAnalyticsEntity.TemplateID))
                return null;
            var entity = new __TextAnalyticsEntity(source);
            if (entity.TextAnalyticsMD5 == entity.RawContent.CalculateMd5Hash())
                return null;

            return new TextAnalyticsStringContent
            {
                Content = entity.RawContent, 
                CurrentMd5 = entity.TextAnalyticsMD5
            };
        }
    }
}