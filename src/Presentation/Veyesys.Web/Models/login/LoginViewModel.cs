using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Veyesys.Web.Models.login
{
    public class LoginViewModel
    {

        public LoginViewModel()
        {
            AvailableLanguages = new List<SelectListItem>();
        }

        [Required]
        public string Username { get; set; }

        [Required]
        public string language { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public List<SelectListItem> AvailableLanguages { get; set; }
    }
}
