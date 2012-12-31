using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Linq;
using System.Web.WebPages.Html;

namespace project_application.Models
{
    public class MyKeyValue
    {
        public int Key;
        public string Value;
    }

    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public UserDepartment UserDepartment { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "部门")]
        public UserDepartment UserDepartment { get; set; }

        [Required]
        [Display(Name = "姓名")]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        public string RealName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> GetUserDepartmentList()
        {
            using (var db = new ProjectsContext())
            {
                var tempList = db.UserDepartments.Select(a => new MyKeyValue { Key = a.DepartmentId, Value = a.DepartmentName });
                List<System.Web.Mvc.SelectListItem> selectList = new List<System.Web.Mvc.SelectListItem>();
                foreach (var pair in tempList)
                {
                    selectList.Add(new System.Web.Mvc.SelectListItem
                    {
                        Text = pair.Value,
                        Value = pair.Key.ToString()
                    });
                }
                return selectList;
            }
        }
    }

    public class ActivateModel
    {
        public int UserId { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "部门")]
        public string DepartmentName { get; set; }

        [Display(Name = "姓名")]
        public string RealName { get; set; }

        [Required]
        [Display(Name = "状态")]
        public String Status { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
