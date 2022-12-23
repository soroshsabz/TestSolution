using System;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Commons.MedicalTestViewModels
{
    public class MedicalTestRootViewModel
    {
        public int Total { get; set; }

        public int PerPage { get; set; }

        public int Page { get; set; }

        public int LastPage { get; set; }

        public IList<MedicalTestViewModel> Data { get; set; }
    }

    public class MedicalTestViewModel
    {
        public string Id { get; set; }

        public MedicalTestStatus Status { get; set; }

        public int Price { get; set; }

        public IList<string> Files { get; set; }

        public MedicalTestPatient User { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class MedicalTestPatient
    {
        public string Id { get; set; }

        public string Phone { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}