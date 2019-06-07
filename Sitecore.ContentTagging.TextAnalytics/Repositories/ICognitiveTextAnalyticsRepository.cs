using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;

namespace Sitecore.ContentTagging.TextAnalytics.Repositories
{
    public interface ICognitiveTextAnalyticsRepository
    {
        float DetectSentimentInContent(string content, TextAnalyticsClient client, string language="en");
        NameValueCollection DetectEntitiesInContent(string content, TextAnalyticsClient client, string language="en");
        TextAnalyticsClient GetClient(string subscriptionKey, string endpoint);
    }
}