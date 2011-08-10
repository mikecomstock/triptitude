using System.Configuration;
using Amazon.PAAPI;

namespace Triptitude.Biz.Services
{
    public class AmazonService
    {
        private static string AWSAccessKeyId { get { return ConfigurationManager.AppSettings["AWSAccessKeyId"]; } }
        private static string AmazonAssociateTag { get { return ConfigurationManager.AppSettings["AmazonAssociateTag"]; } }

        public AmazonItem Find(string ASIN)
        {
            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient();

            ItemLookup itemLookup = new ItemLookup
            {
                AssociateTag = AmazonAssociateTag,
                AWSAccessKeyId = AWSAccessKeyId,
                Shared = new ItemLookupRequest
                {
                    ItemId = new[] { ASIN },
                    ResponseGroup = new[] { "ItemAttributes", "Images" }
                }
            };

            ItemLookupResponse itemLookupResponse = amazonClient.ItemLookup(itemLookup);
            Item product = itemLookupResponse.Items[0].Item[0];
            return new AmazonItem
            {
                DetailPageURL = product.DetailPageURL,
                SmallImageURL = product.SmallImage.URL,
                SmallImageHeight = product.SmallImage.Height.Value,
                SmallImageWidth = product.SmallImage.Width.Value
            };
        }
    }

    public class AmazonItem
    {
        public string DetailPageURL { get; set; }
        public string SmallImageURL { get; set; }
        public decimal SmallImageHeight { get; set; }
        public decimal SmallImageWidth { get; set; }
    }
}