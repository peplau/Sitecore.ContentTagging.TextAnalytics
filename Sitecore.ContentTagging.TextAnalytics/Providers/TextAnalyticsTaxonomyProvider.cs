using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Sitecore.Buckets.Interfaces;
using Sitecore.Buckets.Util;
using Sitecore.ContentTagging.Core.Comparers;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.ContentTagging.TextAnalytics.Models;
using Sitecore.ContentTagging.TextAnalytics.Templates;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.ContentTagging.TextAnalytics.Providers
{
    public class TextAnalyticsTaxonomyProvider: ITaxonomyProvider
    {
        private string _tagRepositoryId;
        protected ITagRepository TagRepository;
        protected static Database Database => Context.ContentDatabase ?? Context.Database;

        public TextAnalyticsTaxonomyProvider()
        {
            TagRepository = DependencyResolver.Current.GetService<ITagRepository>();
            _tagRepositoryId = Sitecore.Configuration.Settings.GetSetting(
                Constants.ConfigKeys.TextAnalyticsTagRepository, "{154D56CC-0DE2-43C7-BBC0-A25BD7FFD901}");
        }

        public IEnumerable<Tag> CreateTags(IEnumerable<TagData> tagData)
        {
            var tagList = new List<Tag>();
            var tagDatas = tagData.ToList();
            if (!tagDatas.Any())
                return tagList;

            foreach (var tagDataEntry in tagDatas.Distinct(new TagNameComparer()))
            {
                var tag = GetTag(tagDataEntry);
                if (tag == null)
                    continue;
                tagList.Add(tag);
            }

            return tagList;
        }

        /// <summary>
        /// Return a tag by tagEntry
        /// </summary>
        /// <param name="tagEntry"></param>
        /// <returns></returns>
        public Tag GetTag(TagData tagEntry)
        {
            // Sentiment Tags
            if (tagEntry.GetType()==typeof(SentimentTagData))
            {
                var range = SentimentRange.GetSentimentRangeForValue(tagEntry.Relevance);
                return range == null ? null : new Tag
                    {
                        TaxonomyProviderId = ProviderId,
                        TagName = nameof(SentimentTagData),
                        ID = range.ID.ToString(),
                        Data = tagEntry
                    };
            }

            // Entities Tags
            if (tagEntry.GetType() == typeof(EntitiesTagData))
            {
                return new Tag
                {
                    TaxonomyProviderId = ProviderId,
                    TagName = nameof(EntitiesTagData),
                    Data = tagEntry
                };
            }

            // Normal Tags
            var tag = GetTag(tagEntry.TagName);
            if (tag != null)
            {
                tag.Data = tagEntry;
                return tag;
            }

            var tagId = CreateTag(tagEntry);
            if (!(tagId == (ID) null))
            {
                return new Tag
                {
                    TagName = tagEntry.TagName,
                    ID = tagId.ToString(),
                    TaxonomyProviderId = ProviderId,
                    Data = tagEntry
                };
            }
            return null;
        }

        /// <summary>Create tag in tag storage from given TagData</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual ID CreateTag(TagData data)
        {
            var template = new TemplateItem(Database.GetItem(BucketConfigurationSettings.TagRepositoryId));
            var repositoryItem = Context.ContentDatabase.GetItem(_tagRepositoryId);
            if (repositoryItem == null)
                return null;
            var name = ItemUtil.ProposeValidItemName(RemoveDiacritics(data.TagName), "tag");
            using (new SecurityDisabler())
            {
                var tagItem = repositoryItem.Add(name, template);
                if (name == data.TagName) 
                    return tagItem.ID;
                tagItem.Editing.BeginEdit();
                tagItem.Fields["__Display Name"].Value = data.TagName;
                tagItem.Editing.EndEdit();
                return tagItem.ID;
            }
        }

        /// <summary>Replaces forbiden by Sitecore characters</summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected virtual string RemoveDiacritics(string s)
        {
            return Encoding.ASCII.GetString(Encoding.GetEncoding(1251).GetBytes(s));
        }

        /// <summary>Return tag by tag id</summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Tag GetTag(string tagId)
        {
            var tag = TagRepository.GetTags(RemoveDiacritics(tagId)).FirstOrDefault();
            if (tag == null)
                return null;
            return new Tag
            {
                TaxonomyProviderId = ProviderId,
                TagName = tag.DisplayText,
                ID = tag.DisplayValue
            };
        }

        public Tag GetParent(string tagId)
        {
            return null;
        }

        public IEnumerable<Tag> GetChildren(string tagId)
        {
            return null;
        }

        public string ProviderId => nameof (TextAnalyticsTaxonomyProvider);
    }
}