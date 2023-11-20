using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class DepositType
    {
        [Key]
        public int IdDepositType { get; set; }
        public string Title { get; set; }

        public ICollection<Deposit> Deposits { get; set; }
    }
}
