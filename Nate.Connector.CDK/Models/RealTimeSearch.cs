using Newtonsoft.Json;
using Scribe.Connector.Common.Reflection;
using Scribe.Connector.Common.Reflection.Actions;
using System;

namespace CDK.Models.RealTimeSearch
{
    [ObjectDefinition (Name = "RealTimeSearch")]
    [Query]
    public class Rootobject
    {
        [PropertyDefinition]
        public Item[] Items { get; set; }

        //Filters
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false)]
        [JsonIgnore]
        public string FilterType { get; set; }
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false)]
        [JsonIgnore]
        public string Filter { get; set; }
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false)]
        [JsonIgnore]
        public string Location { get; set; }
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false)]
        [JsonIgnore]
        public string XrefType { get; set; }
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false)]
        [JsonIgnore]
        public string Quantity { get; set; }
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false)]
        [JsonIgnore]
        public string SortBy { get; set; }
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false)]
        [JsonIgnore]
        public string ConiditionCode { get; set; }
    }

    [ObjectDefinition]
    public class Item
    {
        [PropertyDefinition]
        public int index { get; set; }
        [PropertyDefinition]
        public string alternatePartNumber { get; set; }
        [PropertyDefinition]
        public string cage { get; set; }
        [PropertyDefinition]
        public string conditionCode { get; set; }
        [PropertyDefinition]
        public string currency { get; set; }
        [PropertyDefinition]
        public DateTime lastUpdateDate { get; set; }
        [PropertyDefinition]
        public string manufacturer { get; set; }
        [PropertyDefinition]
        public string partDescription { get; set; }
        [PropertyDefinition]
        public string partNumber { get; set; }
        [PropertyDefinition]
        public string quantity { get; set; }
        [PropertyDefinition]
        public Seller seller { get; set; }
        [PropertyDefinition]
        public decimal unitPrice { get; set; }
        [PropertyDefinition]
        public string uoM { get; set; }
    }

    [ObjectDefinition]
    public class Seller
    {
        [PropertyDefinition]
        public Certificate[] certificates { get; set; }
        [PropertyDefinition]
        public string fax { get; set; }
        [PropertyDefinition]
        public bool hasMoreCertificates { get; set; }
        [PropertyDefinition]
        public string phone { get; set; }
        [PropertyDefinition]
        public string seller { get; set; }
        [PropertyDefinition]
        public string city { get; set; }
        [PropertyDefinition]
        public string state { get; set; }
        [PropertyDefinition]
        public string country { get; set; }
        [PropertyDefinition]
        public string specialInstruction { get; set; }
        [PropertyDefinition]
        public int sellerId { get; set; }
        [PropertyDefinition]
        public string contactName { get; set; }
        [PropertyDefinition]
        public string email { get; set; }
        [PropertyDefinition]
        public string address { get; set; }
        [PropertyDefinition]
        public string postalCode { get; set; }
        [PropertyDefinition]
        public string webSite { get; set; }
        [PropertyDefinition]
        public string extension { get; set; }
    }

    [ObjectDefinition]
    public class Certificate
    {
        [PropertyDefinition]
        public string name { get; set; }
    }

}
