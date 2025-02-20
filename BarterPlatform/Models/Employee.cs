using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BarterPlatform.Models
{
    public partial class Employee
    {
        public Employee()
        {
            AuthenticationRecords = new HashSet<AuthenticationRecords>();
            ManageCommentRecords = new HashSet<ManageCommentRecords>();
            ManageItemRecords = new HashSet<ManageItemRecords>();
        }

        [Key]
        [Display(Name = "*員工欄位")]
        [RegularExpression(@"^[A-Z][0-9]{7}$", ErrorMessage = "格式無效，應為大寫字母 + 七個數字。")]
        [StringLength(8)]
        public string EmpID { get; set; } = null!;
        
        [Display(Name = "*姓名")]
        [Required(ErrorMessage = "必填欄位")]
        public string PersonalName { get; set; } = null!;

        [Display(Name = "*暱稱")]
        [Required(ErrorMessage = "必填欄位")]
        public string Nickname { get; set; } = null!;

        [Display(Name = "*性別")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^(男|女|其他)$", ErrorMessage = "性別必須為'男'、'女'或'其他'")]
        public string Gender { get; set; } = null!;

        [Display(Name = "*生日")]
        [Required(ErrorMessage = "必填欄位")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "*郵遞區號")]
        [Required(ErrorMessage = "必填欄位")]
        public string PostalCode { get; set; } = null!;

        [Display(Name = "*行政區")]
        [Required(ErrorMessage = "必填欄位")]
        public string AdminRegion_AdmID { get; set; } = null!;

        [Display(Name = "*鄉鎮市區")]
        [Required(ErrorMessage = "必填欄位")]
        public string District_DisID { get; set; } = null!;

        [Display(Name = "*其他地址")]
        [Required(ErrorMessage = "必填欄位")]
        public string OtherAddress { get; set; } = null!;

        [Display(Name = "*帳號")]
        [Required(ErrorMessage = "必填欄位")]
        public string Username { get; set; } = null!;

        [Display(Name = "*密碼")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,50}$", ErrorMessage = "密碼必須包含至少一個大寫字母、一個小寫字母、一個符號和一個數字，並且長度在6到50個字符之間")]
        public string Password { get; set; } = null!;

        [Display(Name = "*電子信箱")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "請輸入有效的電子郵件地址")]
        public string Email { get; set; } = null!;

        [Display(Name = "*電話")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^\d{1,12}$", ErrorMessage = "電話號碼最多包含12個數字")]
        public string Phone { get; set; } = null!;

        [Display(Name = "*身分證字號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(10, ErrorMessage = "為什麼你可以超過10字?蛤!")]
        [RegularExpression(@"^[A-Z]{1}[1-2]{1}[0-9]{8}$", ErrorMessage = "請輸入有效的身分證字號")]
        public string NationalID { get; set; } = null!;

        [Display(Name = "*入職時間")]
        public DateTime HireDate { get; set; }

        [Display(Name = "離職時間")]
        public DateTime? TermDate { get; set; }

        [Display(Name ="薪資")]
        public decimal? Salary { get; set; }

        [Display(Name = "在職狀態")]
        public bool Status { get; set; }

        //修改生日規格，讓Index取得
        public string FormattedBirthDate => BirthDate.ToString("yyyy/MM/dd");

        public virtual AdministrativeRegion? AdminRegion_Adm { get; set; }
        public virtual District? District_Dis { get; set; }
        public virtual ICollection<AuthenticationRecords> AuthenticationRecords { get; set; }
        public virtual ICollection<ManageCommentRecords> ManageCommentRecords { get; set; }
        public virtual ICollection<ManageItemRecords> ManageItemRecords { get; set; }
    }


}
