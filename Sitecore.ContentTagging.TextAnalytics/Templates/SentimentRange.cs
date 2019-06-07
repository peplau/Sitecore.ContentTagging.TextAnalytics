using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;

namespace Sitecore.ContentTagging.TextAnalytics.Templates
{
    public partial class SentimentRange
    {
        private static readonly string TextAnalyticsSentimentRangesRepository =
            Sitecore.Configuration.Settings.GetSetting(Constants.ConfigKeys.TextAnalyticsSentimentRangesRepository,
                "/sitecore/system/Modules/TextAnalyticsTagging/Sentiment Ranges");

        protected static Database ContextDatabase => Context.ContentDatabase ?? Context.Database;

        private static List<SentimentRange> _ranges;
        public static List<SentimentRange> Ranges
        {
            get
            {
                if (_ranges != null)
                    return _ranges;

                var repositoryItem = ContextDatabase.GetItem(TextAnalyticsSentimentRangesRepository);
                _ranges = repositoryItem == null
                    ? new List<SentimentRange>()
                    : repositoryItem.Children.Select(p=>new SentimentRange(p)).ToList();
                return _ranges;
            }
        }

        public float LowerNumber
        {
            get
            {
                float.TryParse(Lower, out var res);
                return res;
            }
        }

        public float HigherNumber
        {
            get
            {
                float.TryParse(Higher, out var res);
                return res;
            }
        }

        /// <summary>
        /// Get SentimentRange item for a given rating value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SentimentRange GetSentimentRangeForValue(float value)
        {
            return Ranges.FirstOrDefault(p => value >= p.LowerNumber && value <= p.HigherNumber);
        }
    }
}