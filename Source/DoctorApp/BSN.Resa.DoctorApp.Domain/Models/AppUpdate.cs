using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Utilities;
using System;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Domain.Models
{
    public partial class AppUpdate
    {
        #region Constructor

        public AppUpdate(IApplicationServiceCommunicator applicationServiceCommunicator)
        {
            ApplicationServiceCommunicator = applicationServiceCommunicator;
        }

        #endregion

        #region Public Properties

        public bool HasUrgentUpdateLocally { get; private set; }

        public bool HasNotifiableUpdateLocally { get; private set; }

        public DateTime? LastSynchronizationTime { get; private set; }

        public Version LatestDownloadableAppUpdateVersionLocally
        {
            get => LatestDownloadableAppUpdateVersionLocallyInString != null ? Version.Parse(LatestDownloadableAppUpdateVersionLocallyInString) : null;
            private set => LatestDownloadableAppUpdateVersionLocallyInString = value?.ToString();
        }

        public string LatestDownloadableAppUpdateUrlLocally { get; private set; }

        public IApplicationServiceCommunicator ApplicationServiceCommunicator { get; set; }

        #endregion

        #region Public Methods

        public Task<bool> HasUrgentUpdateAsync(Version currentVersion)
        {
            return TrySynchronizationThenGetAsync(x => x.HasUrgentUpdateLocally, currentVersion);
        }

        public Task<bool> HasNotifiableUpdateAsync(Version currentVersion)
        {
            return TrySynchronizationThenGetAsync(x => x.HasNotifiableUpdateLocally, currentVersion);
        }

        public Task<string> GetLatestDownloadableAppUpdateUrlAsync(Version currentVersion)
        {
            return TrySynchronizationThenGetAsync(x => x.LatestDownloadableAppUpdateUrlLocally, currentVersion);
        }

        #endregion

        #region Private Methods

        private async Task<T> TrySynchronizationThenGetAsync<T>(Func<AppUpdate, T> propertyAccessExpression, Version currentVersion)
        {
            if (LatestDownloadableAppUpdateVersionLocally != null && LatestDownloadableAppUpdateVersionLocally <= currentVersion)
                ClearAllLocalData();

            if (LastSynchronizationTime != null && LastSynchronizationTime.Value.Add(MinimumSynchronizationPeriod) > DateTime.Now)
                return propertyAccessExpression.Invoke(this);

            try
            {
                var result =
                    await ApplicationServiceCommunicator.GetAppUpdateManifestAsync().ConfigureAwait(false);
                if (result.ResultCode == ServiceResultCode.Success)
                {
                    DoctorAppAutoMapper.Instance.Map(result.Data, this);
                    LastSynchronizationTime = DateTime.Now;
                }
            }
            catch (NetworkConnectionException)
            {
                // ignored
            }
            catch (Exception)
            {
                // ignored
            }

            return propertyAccessExpression.Invoke(this);
        }

        private void ClearAllLocalData()
        {
            HasUrgentUpdateLocally = false;
            HasNotifiableUpdateLocally = false;
            LatestDownloadableAppUpdateUrlLocally = string.Empty;
            LatestDownloadableAppUpdateVersionLocally = null;
        }

        #endregion

        #region Private fields

        private static readonly TimeSpan MinimumSynchronizationPeriod = TimeSpan.FromMinutes(10);

        #endregion
    }
}
