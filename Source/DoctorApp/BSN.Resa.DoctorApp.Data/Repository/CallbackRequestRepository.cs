using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BSN.Resa.DoctorApp.Data.Repository
{
    public interface ICallbackRequestRepository : IRepository<CallbackRequest>
    {
        IEnumerable<CallbackRequest> GetActiveCallbackRequests(bool asReadOnly = false);

        int UnSeenCount();
    }

    /*
     * Important note:
     * (Implementation details)
    * For every methods that conceptually get CallbackRequest(s) from DB we SHOULD use
    * AddServiceCommunicator() method to set DoctorServiceCommunicationToken and IDoctorServiceCommunicator
    * for the CallbackRequest(s).
    */
    public sealed class CallbackRequestRepository : RepositoryBase<CallbackRequest>, ICallbackRequestRepository
    {
        #region Constructor

        public CallbackRequestRepository(
            IDatabaseFactory databaseFactory,
            IDoctorRepository doctorRepository,
            IDoctorServiceCommunicator doctorServiceCommunicator) : base(databaseFactory)
        {
            _doctorServiceCommunicator = doctorServiceCommunicator;
            _serviceCommunicationToken = doctorRepository.Get(false).GetServiceCommunicatorToken();
        }

        #endregion

        public override CallbackRequest Get(Expression<Func<CallbackRequest, bool>> @where, bool asReadOnly = false)
        {
            CallbackRequest rawCallbackRequest = base.Get(@where, asReadOnly);

            return AddServiceCommunicator(rawCallbackRequest);
        }

        public override CallbackRequest Get(string id, bool asReadOnly = false)
        {
            CallbackRequest rawCallbackRequest = base.Get(id, asReadOnly);

            return AddServiceCommunicator(rawCallbackRequest);
        }

        public override IEnumerable<CallbackRequest> GetMany(Expression<Func<CallbackRequest, bool>> @where, bool asReadOnly = false)
        {
            var rawCallbackRequests = base.GetMany(@where, asReadOnly);

            return rawCallbackRequests.Select(AddServiceCommunicator);
        }

        public override IEnumerable<CallbackRequest> GetAll(bool asReadOnly = false)
        {
            var rawCallbackRequests = base.GetAll(asReadOnly);

            return rawCallbackRequests.Select(AddServiceCommunicator);
        }

        /// <summary>
        /// Returns only those callback requests that is valid to be called either by doctor or patient.
        /// </summary>
        /// <param name="asReadOnly"></param>
        /// <returns></returns>
        public IEnumerable<CallbackRequest> GetActiveCallbackRequests(bool asReadOnly = false)
        {
            Expression<Func<CallbackRequest, bool>> where = (callbackRequest => !callbackRequest.IsCancelled && (!callbackRequest.IsExpired && !callbackRequest.ReturnCallHasBeenEstablished));

            var result = asReadOnly ? DbSet.AsNoTracking().Where(where) : DbSet.Where(where);

            return result.AsEnumerable().Select(AddServiceCommunicator);
        }

        public int UnSeenCount()
        {
            var unseenCalls = GetAllUnSeenCallbacks();
            return unseenCalls?.Count() ?? 0;
        }

        #region Private Methods

        /// <summary>
        /// This returns a collection of callback requests that the doctor has not yet seen.
        /// This will be used for showing the number of unseen calls to doctor,
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CallbackRequest> GetAllUnSeenCallbacks(bool asReadOnly = false)
        {
            var allFreshCallbacks = GetActiveCallbackRequests(asReadOnly);
            var result = allFreshCallbacks?.Where(call => !call.IsSeen);
            return result;
        }

        private CallbackRequest AddServiceCommunicator(CallbackRequest rawCallbackRequest)
        {
            return CallbackRequest.AddServiceCommunicator(rawCallbackRequest, _serviceCommunicationToken, _doctorServiceCommunicator);
        }

        #endregion

        #region Private Fields

        private readonly IDoctorServiceCommunicator _doctorServiceCommunicator;
        private readonly DoctorServiceCommunicationToken _serviceCommunicationToken;

        #endregion
    }
}