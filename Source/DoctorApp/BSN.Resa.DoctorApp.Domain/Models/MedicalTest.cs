using BSN.Resa.DoctorApp.Commons.MedicalTestViewModels;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Domain.Models
{
    public class MedicalTest
    {
        public static MedicalTest AddServiceCommunicator(MedicalTest medicalTest,
            DoctorServiceCommunicationToken communicationToken, IDoctorServiceCommunicator serviceCommunicator)
        {
            if (medicalTest == null)
                return null;

            medicalTest.SetServiceCommunicationToken(communicationToken);
            medicalTest.SetServiceCommunicator(serviceCommunicator);
            return medicalTest;
        }

        #region Properties

        public string Id { get; set; }

        public MedicalTestStatus Status { get; set; }

        public int Price { get; set; }

        public IList<string> Photos { get; set; }

        public string PatientId { get; set; }

        public string PatientPhone { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string TextReply { get; set; }

        public FileStream VoiceFileStream { get; set; }

        #endregion

        #region Methods

        public async Task SubmitTextReply(CancellationToken cancellationToken = default)
        {
            CheckPreCondition();

            _serviceCommunicator.CommunicationToken = _communicationToken;

            await _serviceCommunicator.SubmitMedicalTestTextReply(int.Parse(Id), TextReply, cancellationToken);
        }

        public async Task SubmitVoiceReply(string voiceFileNameWithExtension, CancellationToken cancellationToken = default)
        {
            CheckPreCondition();

            _serviceCommunicator.CommunicationToken = _communicationToken;

            await _serviceCommunicator.SubmitMedicalTestVoiceReply(int.Parse(Id), VoiceFileStream, voiceFileNameWithExtension, cancellationToken);
        }

        private void SetServiceCommunicationToken(DoctorServiceCommunicationToken communicationToken)
        {
            _communicationToken = communicationToken;
        }

        private void SetServiceCommunicator(IDoctorServiceCommunicator serviceCommunicator)
        {
            _serviceCommunicator = serviceCommunicator;
        }

        private void CheckPreCondition()
        {
            if (_serviceCommunicator == null || _communicationToken == null)
                throw new NullReferenceException("Before this request, " +
                                                 "you should call SetCommunicationToken() and SetServiceCommunicator()");
        }

        #endregion

        #region Private fields

        private DoctorServiceCommunicationToken _communicationToken;
        private IDoctorServiceCommunicator _serviceCommunicator;

        #endregion
    }
}