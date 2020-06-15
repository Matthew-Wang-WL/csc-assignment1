using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task3.Models
{
    public class ReCaptchaModel
    {
        [Required]
        public string UserToken { get; set; }
    }
}