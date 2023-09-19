using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem_Using_Entity_Framework
{
    public class Account
    {
        [Key]
        public int Account_Id { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
        public User User { get; set; }
        public List<Transaction> Transactions { get; set; }
        [ForeignKey("User")]
        public int User_Id { get; set; }
    }
}
