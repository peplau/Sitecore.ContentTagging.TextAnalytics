using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models; 

namespace Sitecore.ContentTagging.TextAnalytics.Repositories
{
    public class CognitiveTextAnalyticsRepository : ICognitiveTextAnalyticsRepository
    {
        /// <summary>
        /// Detects sentiment in a given Content
        /// Sentiments are expressed from 0 (negative) to 1 (positive)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="client"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public float DetectSentimentInContent(string content, TextAnalyticsClient client, string language="en")
        {
            var ret = SentimentInContent(client,content,(float)0.5); 
            ret.Wait();
            return ret.Result;
        }

        public static async Task<float> SentimentInContent(TextAnalyticsClient client, string content,
            float defaultScore, string language="en")
        {
            var inputDocuments = new MultiLanguageBatchInput
            {
                Documents = new List<MultiLanguageInput>
                    {new MultiLanguageInput(id: "1", text: content, language: language)}
            };

            var result = await client.SentimentAsync(false, inputDocuments);
            if (!result.Documents.Any())
                return defaultScore;

            var document = result.Documents.First();
            var score = document.Score.HasValue ? (float) document.Score.Value : defaultScore;
            return score;
        }

        /// <summary>
        /// Detects entities from a given Content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="client"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public NameValueCollection DetectEntitiesInContent(string content, TextAnalyticsClient client, string language="en")
        {
            var ret = EntitiesInContent(client,content); 
            ret.Wait();
            return ret.Result;
        }

        public static async Task<NameValueCollection> EntitiesInContent(TextAnalyticsClient client, string content, string language="en")
        {
            var inputDocuments = new MultiLanguageBatchInput
            {
                Documents = new List<MultiLanguageInput>
                    {new MultiLanguageInput(id: "1", text: content, language: "en")}
            };

            var result = await client.EntitiesAsync(false, inputDocuments);
            if (!result.Documents.Any())
                return new NameValueCollection();
            var document = result.Documents.First();

            var entities = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (var entity in document.Entities)
                entities[entity.Type] = entity.Name;
            return entities;
        }

        /// <summary>
        /// Get a TextAnalytics client
        /// </summary>
        /// <param name="subscriptionKey"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public TextAnalyticsClient GetClient(string subscriptionKey, string endpoint)
        {
            var credentials = new Clients.ApiKeyServiceClientCredentials(subscriptionKey);
            var client = new TextAnalyticsClient(credentials)
            {
                Endpoint = endpoint
            };
            return client;
        }
    }
}