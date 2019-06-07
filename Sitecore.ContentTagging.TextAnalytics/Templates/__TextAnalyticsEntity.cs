using System;
using System.Text;
using Sitecore.ContentTagging.TextAnalytics.Extensions;
using Sitecore.Data.Fields;

namespace Sitecore.ContentTagging.TextAnalytics.Templates
{
    public partial class __TextAnalyticsEntity
    {
        private readonly string _fieldsToProcess = Sitecore.Configuration.Settings.GetSetting(Constants.ConfigKeys.TextAnalyticsFieldsToProcess);

        public string RawContent
        {
            get
            {
                var stringBuilder = new StringBuilder();

                // Use all fields
                if (string.IsNullOrEmpty(_fieldsToProcess))
                {
                    foreach (Field field in InnerItem.Fields)
                    {
                        if (field.Name.StartsWith("__", StringComparison.InvariantCulture)) 
                            continue;
                        stringBuilder.Append(field.Value.ExtractText());
                        if (stringBuilder.Length > 0)
                            stringBuilder.Append(" ");
                    }
                }
                else
                {
                    // Use specific fields
                    foreach (var fieldName in _fieldsToProcess.Split('|'))
                    {
                        var field = InnerItem.Fields[fieldName];
                        if (field == null)
                            continue;
                        stringBuilder.AppendLine(field.Value.ExtractText());
                    }
                }

                return stringBuilder.ToString();
            }
        }
    }
}