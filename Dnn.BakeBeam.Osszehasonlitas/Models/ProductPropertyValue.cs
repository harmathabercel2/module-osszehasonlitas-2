
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace Dnn.BakeBeam.Osszehasonlitas.Models
{
    [TableName("hcc_ProductPropertyValue")]
    [PrimaryKey(nameof(Id), AutoIncrement = true)]
    public class ProductPropertyValue
    {
        public long Id { get; set; }

        public Guid ProductBvin { get; set; }

        public long PropertyId { get; set; }

        public string PropertyValue { get; set; } = string.Empty;

        public long StoreId { get; set; }



    }
}
