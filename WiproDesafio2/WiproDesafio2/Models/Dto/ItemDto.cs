using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiproDesafio2.Models.Dto {
    public class ItemDto {
        public string moeda { get; set; }

        public DateTime data_inicio { get; set; }

        public DateTime data_fim { get; set; }

        public string error { get; set;}
    }
}
