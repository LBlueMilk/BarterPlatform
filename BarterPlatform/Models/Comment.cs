using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarterPlatform.Models
{
    public partial class Comment
    {
        public Comment()
        {
            ManageCommentRecords = new HashSet<ManageCommentRecords>();
        }

        public int ComID { get; set; }

        [Display(Name = "*留言內容")]
        public string ComContent { get; set; } = null!;

        [Display(Name ="*留言時間")]
        public DateTime ComTime { get; set; }

        [Display(Name = "*留言者")]
        public string Member_MemID { get; set; } = null!;

        [Display(Name = "*留言易物編號")]
        public int Item_IteID { get; set; }

        public virtual Item Item_ID { get; set; } = null!;
        public virtual Member Member_Mem { get; set; } = null!;
        public virtual ICollection<ManageCommentRecords> ManageCommentRecords { get; set; }
    }
}
