namespace BSN.Resa.Core.Commons.ViewModels
{
    public class VirtualPhoneNumberAssignment
    {
        public int Id { get; set; }

        public string SourcePhoneNumber { get; set; }

        public string DestinationPhoneNumber { get; set; }
        
        public string[] Whitelist { get; set; }
    }
}
