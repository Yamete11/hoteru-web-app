using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Deposit
    {
        [Key]
        public int IdDeposit { get; set; }
        public float Sum { get; set; }
        public int IdDepositType { get; set; }

        [Required]
        [ForeignKey(nameof(IdDepositType))]
        public DepositType DepositType { get; set; }
    }
}
