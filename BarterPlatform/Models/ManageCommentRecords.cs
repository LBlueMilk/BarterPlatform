using System;
using System.Collections.Generic;

namespace BarterPlatform.Models
{
    public partial class ManageCommentRecords
    {
        public int MCRID { get; set; }
        public string Employee_EmpID { get; set; } = null!;
        public int Comment_ComID { get; set; }
        public string BeforeContent { get; set; } = null!;
        public string AfterContent { get; set; } = null!;
        public DateTime ModTime { get; set; }

        public virtual Comment Comment_Com { get; set; } = null!;
        public virtual Employee Employee_Emp { get; set; } = null!;
    }
}
