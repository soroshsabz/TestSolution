using System;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class Remark
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Body { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public string AccountId { get; set; }
    }
}
