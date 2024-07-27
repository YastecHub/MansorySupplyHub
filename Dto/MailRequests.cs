namespace MansorySupplyHub.Dto
{
    public class MailRequests
    {
        public string ToEmail { get; set; }
        public string HtmlContent { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

    }

    public class MailReceiverDto
    {
        public string Name { get; set; }
        public string Email { get; set; }

    }
}
