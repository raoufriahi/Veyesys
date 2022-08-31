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
        [DataType(DataType.Text)]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Language { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public List<SelectListItem> AvailableLanguages { get; set; }
    }
}
