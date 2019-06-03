using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scribe.Connector.Common.Reflection;
using Scribe.Connector.Common.Reflection.Actions;
using Scribe.Connector.Common.Reflection.PropertyType;
using Newtonsoft.Json;

namespace CDK.Models
{
    [Query]
    [ObjectDefinition(Name = "RealTimeSearch")]
    public class Rootobject
    {
        [PropertyDefinition]
        public Item[] items { get; set; }

        [PropertyDefinition(UsedInLookupCondition = true)]
        [JsonIgnore]
        public string FilterType { get; set; }
    }

    [ObjectDefinition]
    public class Item
    {
        [PropertyDefinition (UsedInQuerySelect = true)]
        public int index { get; set; }
        public string alternatePartNumber { get; set; }
        public string cage { get; set; }
        public string conditionCode { get; set; }
        public string currency { get; set; }
        public DateTime lastUpdateDate { get; set; }
        public string manufacturer { get; set; }
        public string partDescription { get; set; }
        public string partNumber { get; set; }
        public string quantity { get; set; }
        public Seller seller { get; set; }
        public float unitPrice { get; set; }
        public string uoM { get; set; }
    }

    public class Seller
    {
        public Certificate[] certificates { get; set; }
        public string extension { get; set; }
        public string fax { get; set; }
        public bool hasMoreCertificates { get; set; }
        public string phone { get; set; }
        public string seller { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string specialInstruction { get; set; }
        public int sellerId { get; set; }
        public string contactName { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string webSite { get; set; }
        public string state { get; set; }
    }

    public class Certificate
    {
        public string name { get; set; }
    }

}
