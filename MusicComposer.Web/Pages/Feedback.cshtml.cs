using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace MusicComposer.Web.Pages
{
    public class FeedbackModel : PageModel
    {
        private readonly IConfiguration _configuration;
        [BindProperty]
        public BL.ContactModel Contact { get; set; }
        
        [BindProperty]
        public bool? Success { get; set; }
        [BindProperty]
        public bool FeedbackAvailable { get; set; }
        public FeedbackModel(IConfiguration configuration)
        {
            _configuration = configuration;
            FeedbackAvailable = BL.SendEmail.IsSendEmailAvailable(configuration);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var message = @$"Feedback,
Name: {Contact.Name}
Email: {Contact.Email}
Message: {Contact.Message}";
            Success = await BL.SendEmail.Send(_configuration, message);
            return Page();
        }
    }
}
