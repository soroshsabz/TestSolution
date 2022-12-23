using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Views;
using BSN.Resa.DoctorApp.Views.CallbackRequests;
using System;
using FlyoutPage = BSN.Resa.DoctorApp.Views.FlyoutPage;

namespace BSN.Resa.DoctorApp.EventConsumers.PrismAppStartConsumers
{
    public class PrismAppStartConsumer : IPrismAppStartConsumer
    {
        #region Constructor

        public PrismAppStartConsumer(
            IAppUpdateRepository appUpdateRepository,
            IUnitOfWork unitOfWork,
            IConfig config,
            ICrashReporter crashReporter)
        {
            _appUpdateRepository = appUpdateRepository;
            _unitOfWork = unitOfWork;
            _config = config;
            _crashReporter = crashReporter;
        }

        #endregion

        public string OnStart()
        {
            if (DoctorAppSettings.IsUrgentUpdateDownloaded)
            {
                DoctorAppSettings.IsUrgentUpdatePagePassed = false;

                return $"{nameof(AppNavigationPage)}/{nameof(UrgentUpdatePage)}";
            }

            AppUpdate appUpdate = _appUpdateRepository.Get();
            try
            {
                bool hasAppUrgentUpdate = appUpdate.HasUrgentUpdateAsync(_config.Version).ResultWithUnwrappedExceptions();

                _appUpdateRepository.Update();
                _unitOfWork.Commit();

                if (hasAppUrgentUpdate)
                {
                    DoctorAppSettings.IsUrgentUpdatePagePassed = false;

                    return $"{nameof(AppNavigationPage)}/{nameof(UrgentUpdatePage)}";
                }
            }
            catch (Exception exception)
            {
                _crashReporter.SendException(exception);
            }

            DoctorAppSettings.IsUrgentUpdatePagePassed = true;

            if (!DoctorAppSettings.IsDoctorLoggedIn)
                return nameof(LoginPage);

            if (!DoctorAppSettings.IsPermissionPagePassed)
            {
                return $"/{nameof(PermissionsPage)}";
            }

            bool isForegroundServiceNotificationClicked = DoctorAppSettings.IsForegroundServiceNotificationClicked;

            if (isForegroundServiceNotificationClicked)
            {
                DoctorAppSettings.IsForegroundServiceNotificationClicked = false;

                var appSettingsAdvancedPage = $"{nameof(FlyoutPage)}/{nameof(AppNavigationPage)}/{nameof(AppSettingsAdvancedPage)}";

                return appSettingsAdvancedPage;
            }

            var callbackRequestsPage = $"{nameof(FlyoutPage)}/{nameof(AppNavigationPage)}/{nameof(CallbackRequestsPage)}";

            return callbackRequestsPage;
        }

        #region private fields

        private readonly IAppUpdateRepository _appUpdateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfig _config;
        private readonly ICrashReporter _crashReporter;

        #endregion
    }
}