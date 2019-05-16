////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

namespace SA.iOS.StoreKit
{

    public enum ISN_SKProductType
    {

        //A product that is used once, after which it becomes depleted and must be purchased again.
        //Example: Fish food for a fishing app.
        Consumable,

        //A product that is purchased once and does not expire or decrease with use.
        //Example: Race track for a game app.
        NonConsumable,

        //A product that allows users to purchase a service with a limited duration. 
        //The content of this in-app purchase can be static. This type of subscription does not renew automatically.
        //Example: One-year subscription to a catalog of archived articles.
        NonRenewingSubscription,

        //A product that allows users to purchase dynamic content for a set period. 
        //This type of subscription renews automatically unless cancelled by the user.
        //Example: Monthly subscription for an app offering a streaming service.
        AutoRenewingSubscription

    }
}
