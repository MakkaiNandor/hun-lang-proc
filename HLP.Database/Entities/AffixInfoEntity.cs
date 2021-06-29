using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class AffixInfoEntity : BaseEntity
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public int GroupNumber { get; set; }
        public string WordTypeBefore { get; set; }
        public string WordTypeAfter { get; set; }
        public string Description { get; set; }
    }
}
