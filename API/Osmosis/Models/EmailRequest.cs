
namespace Osmosis.Models
{
    public class EmailRequest
    {
        public Email sender { get; set; }
        public List<Email> to { get; set; }
        public string subject { get; set; }
        public string htmlContent { get; set; }
    }

    public class Email
    {
        public string name { get; set; }
        public string email { get; set; }
    }
}
