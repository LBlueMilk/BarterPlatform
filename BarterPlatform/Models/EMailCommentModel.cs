using System.ComponentModel.DataAnnotations;

namespace BarterPlatform.Models
{
    public class EMailCommentModel
    {
        [Required(ErrorMessage = "姓名是必填的")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "姓名長度應在2到100個字符之間")]
        public string Name { get; set; }

        [Required(ErrorMessage = "電子郵件是必填的")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "留言是必填的")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "留言長度應在5到1000個字符之間")]
        public string Message { get; set; }
    }
}
