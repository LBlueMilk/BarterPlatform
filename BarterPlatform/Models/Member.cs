using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BarterPlatform.Models
{
    public partial class Member
    {
        public Member()
        {
            AuthenticationRecords = new HashSet<AuthenticationRecords>();
            Comment = new HashSet<Comment>();
            Item = new HashSet<Item>();
        }

        [Key]
        [Display(Name = "*會員編號")]
        public string? MemID { get; set; } = null!;

        [Display(Name = "*姓名")]
        [Required(ErrorMessage = "必填欄位")]
        public string PersonalName { get; set; } = null!;

        [Display(Name = "暱稱")]
        public string? Nickname { get; set; }

        [Display(Name = "*性別")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^(男|女|其他)$", ErrorMessage = "性別必須為'男'、'女'或'其他'")]
        public string Gender { get; set; } = null!;

        [Display(Name = "*生日")]
        [Required(ErrorMessage = "必填欄位")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "郵遞區號")]
        public string? PostalCode { get; set; }

        [Display(Name = "*行政區")]
        [Required(ErrorMessage = "必填欄位")]
        public string AdminRegion_AdmID { get; set; } = null!;

        [Display(Name = "*鄉鎮市區")]
        [Required(ErrorMessage = "必填欄位")]
        public string District_DisID { get; set; } = null!;

        [Display(Name = "其他地址")]
        public string? OtherAddress { get; set; }

        [Display(Name = "*帳號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "為什麼你可以超過50字?蛤!")]
        public string Username { get; set; } = null!;

        [Display(Name = "*密碼")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "為什麼你可以超過50字?蛤!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,50}$", ErrorMessage = "密碼必須包含至少一個大寫字母、一個小寫字母、一個符號和一個數字，並且長度在6到50個字符之間")]
        public string Password { get; set; } = null!;

        [Display(Name = "*電子信箱")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "請輸入有效的電子郵件地址")]
        public string Email { get; set; } = null!;

        [Display(Name = "電話")]
        [StringLength(12, ErrorMessage = "為什麼你可以超過12字?蛤!")]
        [RegularExpression(@"^\d{1,12}$", ErrorMessage = "電話號碼最多包含12個數字")]
        [MinLength(7, ErrorMessage = "電話最少7碼")]
        public string? Phone { get; set; }

        [Display(Name = "*身分證字號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(10, ErrorMessage = "為什麼你可以超過10字?蛤!")]
        [RegularExpression(@"^[A-Z]{1}[1-2]{1}[0-9]{8}$", ErrorMessage = "請輸入有效的身分證字號")]
        public string NationalID { get; set; } = null!;

        [Display(Name = "身分證圖片(請上傳 JPG 或 PNG 檔)")]        
        public byte[]? IDImage { get; set; }

        [Display(Name = "*創帳時間")]
        public DateTime CreationTime { get; set; }

        [Display(Name = "刪帳時間")]
        public DateTime? DeletionTime { get; set; }

        [Display(Name = "*最後登入時間")]
        public DateTime LastLogin { get; set; }

        [Display(Name = "*帳號狀態")]
        public bool Status { get; set; }

        //修改生日規格，讓Index取得
        public string FormattedBirthDate => BirthDate.ToString("yyyy/MM/dd");

        public virtual AdministrativeRegion? AdminRegion_Adm { get; set; }
        public virtual District? District_Dis { get; set; }
        public virtual ICollection<AuthenticationRecords> AuthenticationRecords { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Item> Item { get; set; }
    }
}
