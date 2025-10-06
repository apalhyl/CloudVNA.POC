using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Acuo.CloudVNA.Models
{
    public class UserActivity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public DateTimeOffset Start { get; set; }

        public string MachineName { get; set; }

        public string ProcessId { get; set; }

        public string AppName { get; set; }

        public string UserData { get; set; }

        public DateTimeOffset? End { get; set; }
    }
}
