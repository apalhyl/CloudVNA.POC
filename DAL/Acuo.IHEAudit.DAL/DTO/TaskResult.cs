using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acuo.IHEAudit.DAL.DTO
{
    public class TaskResult
    {
        public int TaskId { get; set; }

        public Guid TaskTargetId { get; set; }

        public Guid TaskIheAuditId { get; set; }

        public int TaskStatus { get; set; }

        public string TaskLastError { get; set; }

        public DateTime TaskQueuedTime { get; set; }

        public byte[] TaskXmlMessage { get; set; }
    }
}
