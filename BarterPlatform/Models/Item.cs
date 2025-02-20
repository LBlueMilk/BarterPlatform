using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarterPlatform.Models
{
    public partial class Item
    {
        public Item()
        {
            ManageItemRecords = new HashSet<ManageItemRecords>();
            Comment = new HashSet<Comment>();
        }

        [Key]
        [Display(Name = "*易物編號")]
        public int IteID { get; set; }

        [Display(Name = "易物名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(25, ErrorMessage = "易物名稱最多只能包含25個字元")]
        public string ItemName { get; set; } = null!;

        [Display(Name = "易物圖片")]
        public byte[]? IteImage { get; set; }

        [Display(Name = "*易物說明")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(500, ErrorMessage = "易物名稱最多只能包含500個字元")]
        public string IteDesc { get; set; } = null!;

        [Display(Name = "*上傳時間")]
        public DateTime UploadTime { get; set; }

        [Display(Name = "議價空間")]
        public bool Bargain { get; set; }

        [Display(Name = "*貼文者編號")]
        public string Member_MemID { get; set; } = null!;

        public virtual Member Member_Mem { get; set; } = null!;
        public virtual ICollection<ManageItemRecords> ManageItemRecords { get; set; }

        //外鍵
        [ForeignKey("Item_IteID")]
        public virtual ICollection<Comment> Comment { get; set; }
    }
}
