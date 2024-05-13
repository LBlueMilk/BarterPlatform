using System;
using System.Collections.Generic;

namespace BarterPlatform.Models
{
    public partial class ManageItemRecords
    {
        public int MIRID { get; set; }
        public string Employee_EmpID { get; set; } = null!;
        public int Item_IteID { get; set; }
        public string BeforeContent { get; set; } = null!;
        public string AfterContent { get; set; } = null!;
        public DateTime ModTime { get; set; }

        public virtual Employee Employee_Emp { get; set; } = null!;
        public virtual Item Item_Ite { get; set; } = null!;
    }
}
