using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_application.Models
{
    public class ProjectsContext : DbContext
    {
        public ProjectsContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<BasicProject> BasicProjects { get; set; }
        public DbSet<PreparatoryProject> PreparatoryProjects { get; set; }
        public DbSet<ReservedProject> ReservedProjects { get; set; }
        public DbSet<ConstructionProject> ConstructionProjects { get; set; }
        public DbSet<DoneProject> DoneProjects { get; set; }

        public DbSet<ProjectStage> ProjectStages { get; set; }
        public DbSet<ProjectType> ProjectTypes { get; set; }
        public DbSet<FundsSource> FundsSources { get; set; }
        public DbSet<ConstructionProgress> ConstructionProgresses { get; set; }
        public DbSet<ConstructionPeriod> ConstructionPeriods { get; set; }
        public DbSet<UserDepartment> UserDepartments { get; set; }
    }

    // 基础项目
    public class BasicProject
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "项目编号")]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 0)]
        public string Number { get; set; }

        [Required]
        [Display(Name = "项目名称")]
        [StringLength(40, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "项目类型")]
        public ProjectType Type { get; set; }

        [Required]
        [Display(Name = "项目部门")]
        public UserDepartment Department { get; set; }

        [Required]
        [Display(Name = "项目阶段")]
        public ProjectStage Stage { get; set; }

        [Display(Name = "备注")]
        public string Memo { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> GetProjectTypeList()
        {
            using (var db = new ProjectsContext())
            {
                var tempList = db.ProjectTypes.Select(a => new MyKeyValue { Key = a.ID, Value = a.Name });
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

        public IEnumerable<System.Web.Mvc.SelectListItem> GetProjectStageList()
        {
            using (var db = new ProjectsContext())
            {
                var tempList = db.ProjectStages.Select(a => new MyKeyValue { Key = a.ID, Value = a.Name });
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

    // 预备项目
    public class PreparatoryProject
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "所属项目")]
        public BasicProject project { get; set; }

        [Display(Name = "预算资金(万元)")]
        public decimal BudgetFunds { get; set; }
    }

    // 储备项目
    public class ReservedProject
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "所属项目")]
        public BasicProject project { get; set; }

        [Display(Name = "预算资金(万元)")]
        public decimal BudgetFunds { get; set; }
    }

    // 建设项目
    public class ConstructionProject
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [Display(Name = "所属项目")]
        public BasicProject project { get; set; }

        [Required]
        [Display(Name = "年度")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "项目负责人")]
        public UserProfile Principal { get; set; } // 储备/预备项目需要部门，其他项目需要负责人

        [Required]
        [Display(Name = "资金来源")]
        public FundsSource FundsSource { get; set; }

        [Required]
        [Display(Name = "审批资金(万元)")]
        public decimal ApprovedFunds { get; set; }

        [Display(Name = "政府采购部分建设进度")]
        public ConstructionProgress ConstructionProgressOfGovernmentProcurement { get; set; }

        [Display(Name = "自行采购部分建设进度")]
        public string ConstructionProgressOfSelfProcurement { get; set; }

        [Required]
        [Display(Name = "建设期限")]
        public ConstructionPeriod ConstructionPeriod { get; set; }
    }

    // 完成项目
    public class DoneProject
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [Display(Name = "所属项目")]
        public BasicProject project { get; set; }

        [Required]
        [Display(Name = "完成时间")]
        [DataType(DataType.Date)]
        public DateTime DoneTime { get; set; }
    }

    // 项目阶段
    [DisplayColumn("Name")]
    public class ProjectStage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "项目阶段")]
        public string Name { get; set; }

        [Required]
        [Range(1, 10)]
        [Display(Name = "阶段序号")]
        public int Order { get; set; }
    }

    // 项目类型
    [DisplayColumn("Name")]
    public class ProjectType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "项目类型")]
        public string Name { get; set; }
    }

    // 资金来源
    [DisplayColumn("Name")]
    public class FundsSource
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "资金来源")]
        public string Name { get; set; }
    }

    // 建设进度
    [DisplayColumn("Name")]
    public class ConstructionProgress
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "政府采购部分建设进度")]
        public string Name { get; set; }

        [Required]
        [Range(1, 20)]
        [Display(Name = "进度序号")]
        public int Order { get; set; }
    }

    // 建设期限
    [DisplayColumn("Name")]
    public class ConstructionPeriod
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "建设期限")]
        public string Name { get; set; }
    }

    [DisplayColumn("DepartmentName")]
    public class UserDepartment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "部门")]
        public string DepartmentName { get; set; }
    }

    // 项目模型
    public class ProjectModel
    {
        public int Sequence { get; set; }

        public int ID { get; set; }

        [Display(Name="项目编号")]
        public string Number { get; set; }

        [Display(Name = "项目名称")]
        public string Name { get; set; }

        public int TypeId;

        [Display(Name = "项目类型")]
        public string TypeName { get; set; }

        public int DepartmentId;

        [Display(Name = "所属部门")]
        public string DepartmentName { get; set; }

        public int StageId;

        [Display(Name = "所处阶段")]
        public string StageName { get; set; }

        [Display(Name = "备注")]
        public string Memo { get; set; }
    }

    public class ProjectStageModel
    {
        public int Sequence { get; set; }

        public int ID { get; set; }

        [Display(Name = "项目阶段")]
        public string Name { get; set; }

        [Display(Name = "阶段序号")]
        public int Order { get; set; }
    }

    public class ProjectTypeModel
    {
        public int Sequence { get; set; }

        public int ID { get; set; }

        [Display(Name = "项目类型")]
        public string Name { get; set; }
    }

    public class FundsSourceModel
    {
        public int Sequence { get; set; }

        public int ID { get; set; }

        [Display(Name = "资金来源")]
        public string Name { get; set; }
    }

    public class ConstructionProgressModel
    {
        public int Sequence { get; set; }

        public int ID { get; set; }

        [Display(Name = "政府采购部分建设进度")]
        public string Name { get; set; }

        [Display(Name = "进度序号")]
        public int Order { get; set; }
    }

    public class ConstructionPeriodModel
    {
        public int Sequence { get; set; }

        public int ID { get; set; }

        [Display(Name = "建设期限")]
        public string Name { get; set; }
    }

    public class DepartmentModel
    {
        public int Sequence { get; set; }

        public int ID { get; set; }

        [Display(Name = "部门")]
        public string Name { get; set; }
    }
}
