# Installation

Download the package and install with Installation Wizard

# Configuration

After installing this package, make sure to go through the following steps:

1. Open /App_Config/Include/Sitecore.ContentTagging.TextAnalytics.config
2. Add your TextAnalytics Subscription Key to the setting Sitecore.ContentTagging.TextAnalytics.SubscriptionKey
3. Add your TextAnalytics Endpoint to the setting Sitecore.ContentTagging.TextAnalytics.EndPoint
4. Adjust the setting Sitecore.ContentTagging.TextAnalytics.FieldsToProcess to reflect your field names (or let it empty to use all fields)
5. To have your items processed and tagged, activate their templates by making them inherit from /sitecore/templates/Modules/TextAnalyticsTagging/_Text Analytics Entity

