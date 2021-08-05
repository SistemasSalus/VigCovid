using System.ComponentModel.DataAnnotations;

namespace VigCovidApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Usuario es requerido")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public int EmpresaCodigo { get; set; }
    }
}