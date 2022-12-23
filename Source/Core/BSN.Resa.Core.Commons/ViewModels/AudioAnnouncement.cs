using BSN.Resa.Locale;
using System;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class AudioAnnouncement
    {
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }

        [Display(Name = "AccountId", ResourceType = typeof(Resources))]
        public string AccountId { get; set; }
        
        [Display(Name = "NotValidBefore", ResourceType = typeof(Resources))]
        public DateTime NotValidBefore { get; set; }

        [Display(Name = "NotValidAfter", ResourceType = typeof(Resources))]
        public DateTime? NotValidAfter { get; set; }

        [Display(Name = "Audio", ResourceType = typeof(Resources))]
        public string AudioFileId { get; set; }

        [Display(Name = "AbortAfterCompletion", ResourceType = typeof(Resources))]
        public bool AbortAfterCompletion { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }

        [Display(Name = "IsOperational", ResourceType = typeof(Resources))]
        public bool IsOperational { get; set; }
    }

    public class AudioAnnouncementPreview
    {
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }

        [Display(Name = "Audio", ResourceType = typeof(Resources))]
        public string AudioFileId { get; set; }

        [Display(Name = "AbortAfterCompletion", ResourceType = typeof(Resources))]
        public bool AbortAfterCompletion { get; set; }
    }

    public class AudioAnnouncementModificationViewModel
    {
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }

        [Display(Name = "AccountId", ResourceType = typeof(Resources))]
        public string AccountId { get; set; }

        [Display(Name = "NotValidBefore", ResourceType = typeof(Resources))]
        public DateTime NotValidBefore { get; set; }

        [Display(Name = "NotValidAfter", ResourceType = typeof(Resources))]
        public DateTime? NotValidAfter { get; set; }

        [Display(Name = "Audio", ResourceType = typeof(Resources))]
        public FileModificationViewModel AudioFile { get; set; }

        [Display(Name = "AbortAfterCompletion", ResourceType = typeof(Resources))]
        public bool AbortAfterCompletion { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
    }
}