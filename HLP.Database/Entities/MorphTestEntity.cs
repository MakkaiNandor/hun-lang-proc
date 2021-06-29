using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class MorphTestEntity : BaseEntity
    {
        public string Word { get; set; }
        public string Analysis { get; set; }
        public string MorphCode { get; set; }
    }
}
