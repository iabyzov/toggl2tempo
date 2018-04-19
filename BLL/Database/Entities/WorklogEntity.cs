using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Database.Entities
{
    public class WorklogEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long MasterId { get; set; }

        public long SecondaryId { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
