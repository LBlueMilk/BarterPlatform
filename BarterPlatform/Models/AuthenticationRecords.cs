using System;
using System.Collections.Generic;

namespace BarterPlatform.Models
{
    public partial class AuthenticationRecords
    {
        public int AuthID { get; set; }
        public string Employee_EmpID { get; set; } = null!;
        public string Member_MemID { get; set; } = null!;
        public bool AuthStatus { get; set; }
        public DateTime AuthTime { get; set; }

        public virtual Employee Employee_Emp { get; set; } = null!;
        public virtual Member Member_Mem { get; set; } = null!;
    }
}
