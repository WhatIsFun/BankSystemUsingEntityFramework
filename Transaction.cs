using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem_Using_Entity_Framework
{
    public class Transaction
    {
        [Key]
        public int T_Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int SorAccId { get; set; }
        public int TarAccId { get; set; }
        public Account Account { get; set; }
        [ForeignKey("Account")]
        public int User_Id { get; set; }
        
    }
}
