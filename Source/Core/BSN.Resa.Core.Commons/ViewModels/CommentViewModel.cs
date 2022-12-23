using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public string AccountId { get; set; }
    }
}
