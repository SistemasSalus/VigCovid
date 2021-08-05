using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("Parametro")]
    public class Parametro
    {
        [Column(Order = 0), Key]
        public int i_GroupId { get; set; }

        [Column(Order = 1), Key]
        public int i_ParameterId { get; set; }

        public string v_Value1 { get; set; }
        public string v_Value2 { get; set; }
    }
}