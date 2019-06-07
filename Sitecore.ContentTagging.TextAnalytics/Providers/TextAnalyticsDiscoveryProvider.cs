using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Sitecore.ContentTagging.Core.Messaging;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.ContentTagging.TextAnalytics.Models;
using Sitecore.ContentTagging.TextAnalytics.Repositories;

namespace Sitecore.ContentTagging.TextAnalytics.Providers
{
    public class TextAnalyticsDiscoveryProvider : MessageSource, IDiscoveryProvider
    {
        protected TextAnalyticsClient TextAnalyticsClient;
        protected ICognitiveTextAnalyticsRepository CognitiveTextAnalyticsRepository;
        protected string TagNamePattern { get; set; }
        protected string SubscriptionKey { get; set; }
        protected bool SentimentAnalysisEnabled { get; set; }
        protected bool EntityAnalysisEnabled { get; set; }

        public TextAnalyticsDiscoveryProvider()
        {
            SubscriptionKey =
                Sitecore.Configuration.Settings.GetSetting(Constants.ConfigKeys.TextAnalyticsSubscriptionKey);
            var endPoint =
                Sitecore.Configuration.Settings.GetSetting(Constants.ConfigKeys.TextAnalyticsEndPoint);
            SentimentAnalysisEnabled =
                Sitecore.Configuration.Settings.GetBoolSetting(
                    Constants.ConfigKeys.TextAnalyticsSentimentAnalysisEnabled, false);
            EntityAnalysisEnabled =
                Sitecore.Configuration.Settings.GetBoolSetting(
                    Constants.ConfigKeys.TextAnalyticsEntityAnalysisEnabled, false);

            TagNamePattern = Sitecore.Configuration.Settings.GetSetting(Constants.ConfigKeys.TextAnalyticsTagNamePattern,"{key} - {value}");
            CognitiveTextAnalyticsRepository = DependencyResolver.Current.GetService<ICognitiveTextAnalyticsRepository>();
            TextAnalyticsClient = CognitiveTextAnalyticsRepository.GetClient(SubscriptionKey, endPoint);
        }

        /// <summary>Status of the provider</summary>
        /// <returns>Returns true if provider is configured</returns>
        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(SubscriptionKey);
        }

        /// <summary>GetTags</summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public IEnumerable<TagData> GetTags(IEnumerable<TaggableContent> content)
        {
            var taggableContents = content.Where(p => p != null).ToList();
            if (!taggableContents.Any())
                return new List<TagData>();

            var tagDataList = new List<TagData>();
            foreach (var taggableContent in taggableContents)
            {
                var stringContent = (TextAnalyticsStringContent)taggableContent;
                if (string.IsNullOrEmpty(stringContent?.Content))
                    continue;

                // Call the API passing the Content to be indexed

                // Sentiment
                var rate = 
                    CognitiveTextAnalyticsRepository.DetectSentimentInContent(stringContent.Content, TextAnalyticsClient);
                tagDataList.Add(new SentimentTagData {
                    Relevance = rate,
                    TagName = nameof(SentimentTagData)
                });

                // Entities
                var entities =
                    CognitiveTextAnalyticsRepository.DetectEntitiesInContent(stringContent.Content, TextAnalyticsClient);
                tagDataList.Add(new EntitiesTagData {
                    TagName = nameof(EntitiesTagData),
                    Entities = entities
                });

                // Tags
                foreach (var key in entities.AllKeys)
                {
                    tagDataList.Add(new TagData {
                        TagName = TagNamePattern.Replace("{key}",key).Replace("{value}",entities[key])
                    });
                }
            }
            return tagDataList;
        }
    }
}