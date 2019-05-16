using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.iOS.StoreKit;


namespace SA.iOS
{
    public class ISN_StoreKitUI : ISN_ServiceSettingsUI
    {

        GUIContent ProductIdDLabel = new GUIContent("ProductId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
        GUIContent ProductTypeLabel = new GUIContent("Product Type[?]:", "Select the in-app purchase type you want to register");
        GUIContent DisplayNameLabel = new GUIContent("Display Name[?]:", "This is the name of the In-App Purchase that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
        GUIContent DescriptionLabel = new GUIContent("Description[?]:", "This is the description of the In-App Purchase that will be used by App Review during the review process. If indicated in your code, this description may also be seen by customers. For automatically renewable subscriptions, do not include a duration in the description. The description cannot be longer than 255 bytes.");
        GUIContent PriceTierLabel = new GUIContent("Price Tier[?]:", "The retail price for this In-App Purchase subscription.");



        public override void OnAwake() {
            base.OnAwake();
            AddFeatureUrl("iTunes Connect", "https://unionassets.com/ios-native-pro/itunesconnect-setup-627");
            AddFeatureUrl("Initialization", "https://unionassets.com/ios-native-pro/storekit-initialization-628");
            AddFeatureUrl("Purchase flow", "https://unionassets.com/ios-native-pro/purchase-flow-629");
            AddFeatureUrl("Receipt Validation", "https://unionassets.com/ios-native-pro/receipt-validation-630");
            AddFeatureUrl("Store Review", "https://unionassets.com/ios-native-pro/store-review-controller-631");
        }

        public override string Title {
            get {
                return "Store Kit";
            }
        }

        public override string Description {
            get {
                return "Support in-app purchases and interactions with the App Store.";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "StoreKit_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_StoreKitResolver>();
            }
        }


        protected override void OnServiceUI() {
            using (new SA_WindowBlockWithSpace(new GUIContent("In-App Products List"))) {

                if (ISN_Settings.Instance.InAppProducts.Count == 0) {
                    EditorGUILayout.HelpBox("Use this menu to specify in-app products available for your App.", MessageType.Info);
                }

                SA_EditorGUILayout.ReorderablList(ISN_Settings.Instance.InAppProducts, GetProductDisplayName, DrawProductContent, () => {
                    ISN_Settings.Instance.InAppProducts.Add(new ISN_SKProduct());
                });
            }
        }


        private string GetProductDisplayName(ISN_SKProduct product) {
            return product.LocalizedTitle + "           " + product.Price + "$";
        }

        private void DrawProductContent(ISN_SKProduct product) {

            product.ProductIdentifier = SA_EditorGUILayout.TextField(ProductIdDLabel, product.ProductIdentifier);
            product.LocalizedTitle = SA_EditorGUILayout.TextField(DisplayNameLabel, product.LocalizedTitle);
            product.Type = (ISN_SKProductType)SA_EditorGUILayout.EnumPopup(ProductTypeLabel, product.Type);
            product.PriceTier = (ISN_SKPriceTier)SA_EditorGUILayout.EnumPopup(PriceTierLabel, product.PriceTier);


            EditorGUILayout.LabelField(DescriptionLabel);
            using (new SA_GuiBeginHorizontal()) {
                product.LocalizedDescription = EditorGUILayout.TextArea(product.LocalizedDescription, GUILayout.Height(60), GUILayout.MinWidth(190));
                EditorGUILayout.Space();
                product.Icon = (Texture2D)EditorGUILayout.ObjectField("", product.Icon, typeof(Texture2D), false, GUILayout.Width(75));
            }
        }


    }
}