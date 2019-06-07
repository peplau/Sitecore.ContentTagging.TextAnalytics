using Sitecore.Abstractions;
using Sitecore.ContentTagging.Pipelines.GetTaggingConfiguration;

namespace Sitecore.ContentTagging.TextAnalytics.Pipelines
{
    public class GetTextAnalyticsConfiguration : GetDefaultConfigurationName
    {
        public GetTextAnalyticsConfiguration(BaseSettings settings) : base(settings) { }

        public new void Process(GetTaggingConfigurationArgs args)
        {
            var setting = Settings.GetSetting(Constants.ConfigKeys.TextAnalyticsDefaultConfigurationName, "TextAnalytics");
            args.ConfigurationName = setting;
        }
    }
}