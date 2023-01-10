using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TestEFCoreTranscation
{
    public class TestModel
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public bool IsEnable { get; set; }

    }

    [Table("Employee")]
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Age { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SequenceId { get; set; }
        public string Remark { get; set; }

        [Required]
        public DateTimeOffset CreateAt { get; set; }

        [Required]
        public string CreateBy { get; set; }
    }
}
