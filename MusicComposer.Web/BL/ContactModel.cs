using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicComposer.Web.BL
{
    public class ContactModel
    { 
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
