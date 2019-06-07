using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;

namespace Sitecore.ContentTagging.TextAnalytics.Extensions
{
    public static class ItemExtensions
    {
        public static bool IsDerived([NotNull] this Item item, [NotNull] ID templateId)
        {
            if (item == null)
                return false;
            var itemTemplate = TemplateManager.GetTemplate(item);
            return itemTemplate != null && (itemTemplate.ID == templateId || itemTemplate.DescendsFrom(templateId));
        }
    }
}