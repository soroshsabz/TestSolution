using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BSN.Resa.DoctorApp.Data.Repository
{
    public interface IMedicalTestRepository : IRepository<MedicalTest>
    {
        void DeleteAll();
    }

    /*
     * Important note:
     * (Implementation details)
    * For every methods that conceptually get MedicalTest(s) from DB we SHOULD use
    * AddServiceCommunicator() method to set DoctorServiceCommunicationToken and IDoctorServiceCommunicator
    * for the MedicalTest(s).
    */
    public class MedicalTestRepository : RepositoryBase<MedicalTest>, IMedicalTestRepository
    {
        #region Constructor

        public MedicalTestRepository(
            IDatabaseFactory databaseFactory,
            IDoctorRepository doctorRepository,
            IDoctorServiceCommunicator doctorServiceCommunicator) : base(databaseFactory)
        {
            _doctorRepository = doctorRepository;
            _doctorServiceCommunicator = doctorServiceCommunicator;
            _serviceCommunicationToken = _doctorRepository.Get(false).GetServiceCommunicatorToken();
        }

        #endregion

        #region Public Methods

        public override IEnumerable<MedicalTest> GetAll(bool asReadOnly = false)
        {
            var rawMedicalTests = base.GetAll(asReadOnly);

            return rawMedicalTests.Select(AddServiceCommunicator);
        }

        public void DeleteAll()
        {
            Context.RemoveRange(GetAll());
        }

        public override MedicalTest Get(Expression<Func<MedicalTest, bool>> @where, bool asReadOnly = false)
        {
            return AddServiceCommunicator(base.Get(where, asReadOnly));
        }

        public override MedicalTest Get(string id, bool asReadOnly = false)
        {
            return AddServiceCommunicator(base.Get(id, asReadOnly));
        }

        public override IEnumerable<MedicalTest> GetMany(Expression<Func<MedicalTest, bool>> @where, bool asReadOnly = false)
        {
            var rawMedicalTests = base.GetMany(where, asReadOnly);

            return rawMedicalTests.Select(AddServiceCommunicator);
        }

        MedicalTest IRepository<MedicalTest>.Get(Expression<Func<MedicalTest, bool>> @where, bool asReadOnly)
        {
            return AddServiceCommunicator(Get(@where, asReadOnly));
        }

        MedicalTest IRepository<MedicalTest>.Get(string id, bool asReadOnly)
        {
            return AddServiceCommunicator(Get(id, asReadOnly));
        }

        IEnumerable<MedicalTest> IRepository<MedicalTest>.GetMany(Expression<Func<MedicalTest, bool>> @where, bool asReadOnly)
        {
            var rawMedicalTests = GetMany(@where, asReadOnly);

            return rawMedicalTests.Select(AddServiceCommunicator);
        }

        #endregion

        #region Private Methods

        private MedicalTest AddServiceCommunicator(MedicalTest rawMedicalTest)
        {
            return MedicalTest.AddServiceCommunicator(rawMedicalTest, _serviceCommunicationToken, _doctorServiceCommunicator);
        }

        #endregion

        #region Private Fields

        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorServiceCommunicator _doctorServiceCommunicator;
        private readonly DoctorServiceCommunicationToken _serviceCommunicationToken;

        #endregion
    }
}