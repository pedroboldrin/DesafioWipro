using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiproDesafio2.Models {
    public class CsvCotacao {
        public double vlr_cotacao { get; set; }
        public int cod_cotacao { get; set; }
        public string moeda_cotacao { get; set;}
        public DateTime dat_cotacao { get; set;}
    }
}
