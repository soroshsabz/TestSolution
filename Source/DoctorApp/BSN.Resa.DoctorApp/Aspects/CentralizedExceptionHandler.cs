using Acr.UserDialogs;
using ArxOne.MrAdvice.Advice;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.Locale;
using System;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Aspects
{
    public class CentralizedExceptionHandler : Attribute, IMethodAsyncAdvice
    {
        public CentralizedExceptionHandler()
        {
            _userDialogs = DependencyInjectionHelper.Resolve<IUserDialogs>();
            _crashReporter = DependencyInjectionHelper.Resolve<ICrashReporter>();
        }

        public async Task Advise(MethodAsyncAdviceContext context)
        {
            try
            {
                await context.ProceedAsync();
            }
            catch (InternetNotAvailableException)
            {
                await ShowInternetNotAvailableDialogAsync();
            }
            catch (NetworkConnectionException)
            {
                await ShowNetworkConnectionErrorAlertAsync();
            }
            catch (Exception exception)
            {
                await ShowAppInternalErrorAlertAsync(exception.Message);

                _crashReporter.SendException(exception);
            }
        }

        #region Private Methods

        private async Task ShowAppInternalErrorAlertAsync(string errorMessage)
        {
#if DEBUG
            await _userDialogs.AlertAsync(message: errorMessage, okText: Resources.Close);
#else
            await _userDialogs.AlertAsync(message: Resources.InternalError, okText: Resources.Close);
#endif
        }

        private async Task ShowNetworkConnectionErrorAlertAsync()
        {
            await _userDialogs.AlertAsync(message: Resources.ErrorContactingResa, okText: Resources.Close);
        }

        private async Task ShowInternetNotAvailableDialogAsync()
        {
            await _userDialogs.AlertAsync(message: Resources.NoConnectionError, okText: Resources.Close);
        }

#endregion

#region Private Fields

        private readonly IUserDialogs _userDialogs;
        private readonly ICrashReporter _crashReporter;

#endregion
    }
}
