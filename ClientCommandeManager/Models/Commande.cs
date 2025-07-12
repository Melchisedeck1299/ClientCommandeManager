using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommandeManager.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal Montant { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }

}
