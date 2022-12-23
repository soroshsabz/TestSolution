using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class FileViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string ContentPath { get; set; }
    }

    public class FileModificationViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public Byte[] Content { get; set; }
    }
}
