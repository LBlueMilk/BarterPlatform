using System;
using System.Collections.Generic;

namespace BarterPlatform.Models
{
    public partial class AdministrativeRegion
    {
        public AdministrativeRegion()
        {
            Employee = new HashSet<Employee>();
            Member = new HashSet<Member>();
        }

        public string AdmID { get; set; } = null!;
        public string AdminRegion { get; set; } = null!;

        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<Member> Member { get; set; }
    }
}
