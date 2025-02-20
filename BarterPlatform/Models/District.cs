using System;
using System.Collections.Generic;

namespace BarterPlatform.Models
{
    public partial class District
    {
        public District()
        {
            Employee = new HashSet<Employee>();
            Member = new HashSet<Member>();
        }

        public string DisID { get; set; } = null!;
        public string DistrictName { get; set; } = null!;

        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<Member> Member { get; set; }
    }
}
