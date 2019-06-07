using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.ContentTagging.TextAnalytics.Extensions;
using Sitecore.ContentTagging.TextAnalytics.Models;
using Sitecore.ContentTagging.TextAnalytics.Templates;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.ContentTagging.TextAnalytics.Providers
{
    public class TextAnalyticsTagger : ITagger<Item>
    {
        public void TagContent(Item item, IEnumerable<Tag> tags)
        {
            if (tags == null)
                return;
            var tagList = tags.ToList();
            if (!tagList.Any() || !item.IsDerived(__TextAnalyticsEntity.TemplateID))
                return;

            var entity = new __TextAnalyticsEntity(item);

            item.Editing.BeginEdit();
            var field = (MultilistField) item.Fields["{A14F1B0C-4384-49EC-8790-28A440F3670C}"];
            foreach (var tag in tagList.Where(p=>p?.Data != null))
            {
                var tagData = tag.Data;
                if (tagData.GetType() == typeof(SentimentTagData))
                {
                    // Sentiment
                    item.Fields[__TextAnalyticsEntity.FieldIds.Sentiment].Value = tag.ID;
                    item.Fields[__TextAnalyticsEntity.FieldIds.SentimentRating].Value = tagData.Relevance.ToString(CultureInfo.InvariantCulture);
                }
                else if (tagData.GetType() == typeof(EntitiesTagData))
                {
                    // Entities
                    item.Fields[__TextAnalyticsEntity.FieldIds.Entities].Value = ((EntitiesTagData)tagData).Entities.ToString();
                }
                else
                {
                    // Normal Tagging
                    if (ID.TryParse(tag.ID, out var result) && !field.TargetIDs.Contains(result))
                        field.Add(tag.ID);
                }
            }
            item.Fields[__TextAnalyticsEntity.FieldIds.TextAnalyticsMD5].Value = entity.RawContent.CalculateMd5Hash();
            item.Editing.EndEdit();
        }
    }
}