using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.iOS.Commons;
using BSN.Resa.DoctorApp.iOS.Commons.Utilities;
using BSN.Resa.Locale;
using Foundation;
using PhoneNumbers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BSN.Resa.DoctorApp.iOS.EventConsumers.UrlCallConsumers
{
	public class ShareContactUrlCallConsumer : IUrlCallConsumer
	{
		public ShareContactUrlCallConsumer(
			IDoctorRepository doctorRepository,
			IAppUpdateRepository appUpdateRepository,
			IUnitOfWork unitOfWork,
			ICallBlockAndIdentification callBlockAndIdentification,
			IUserDialogs userDialogs,
			IConfigiOS configiOS)
		{
			_doctorRepository = doctorRepository;
			_appUpdateRepository = appUpdateRepository;
			_unitOfWork = unitOfWork;
			_callBlockAndIdentification = callBlockAndIdentification;
			_userDialogs = userDialogs;
			_configiOS = configiOS;
		}

		public bool OnUrlCall(NSUrl url)
		{
			try
			{
                ExceptionfulOnUrlCall(url);
				return true;
			}
			catch(Exception)
			{
                return false;
			}
		}

		#region Private Methods

		private void ExceptionfulOnUrlCall(NSUrl url)
		{
			AppUpdate appUpdate = _appUpdateRepository.Get();
			try
			{
				if (appUpdate.HasUrgentUpdateAsync(_configiOS.Version).ResultWithUnwrappedExceptions())
					return;
			}
			catch (ServiceCommunicationException)
			{
				// ignored
			}

			NSUrlComponents urlComponents = NSUrlComponents.FromUrl(url, false);

			if (urlComponents.Scheme != _configiOS.UrlScheme)
				throw new InvalidArgumentException($"Url scheme must be {_configiOS.UrlScheme}.");

			if (urlComponents.Host != _configiOS.ShareContactUrlIdentifier)
				throw new InvalidArgumentException($"Url identifier must be {_configiOS.ShareContactUrlIdentifier}.");

            Match queryStringMatch = Regex.Match(urlComponents.Query, QueryStringPattern);
			if (!queryStringMatch.Success)
				throw new InvalidArgumentException("Query param pattern is invalid.");


			_sharedPhoneNumbers = queryStringMatch.Groups["CommaSeperatedPhoneNumbers"].Value
				.FindPhoneNumbers()
				.ToArray()
				.ToE164PhoneNumberFormat();


			if (!CheckUserLoggedIn())
				throw new NullReferenceException("User must be logged in first.");

			if (!CheckNotAddingResaPhoneNumbers())
				throw new InvalidArgumentException("Cannot sharing Resa phone numbers.");

			NormalizePhoneNumbers();

			ShowIsPatientPhoneNumber();
		}

		private void NormalizePhoneNumbers()
		{
            for (int i = 0; i < _sharedPhoneNumbers.Length; ++i)
			{
				try
				{
					_sharedPhoneNumbers[i] = _sharedPhoneNumbers[i].ToE164PhoneNumberFormat();
				}
				catch (NumberParseException)
				{
				    var message = $"{Resources.TheNumber} {_sharedPhoneNumbers[i]} {Resources.IsInvalid}.";

					_userDialogs.Alert(
						message,
						Resources.ContactsSharingWithResa,
						Resources.Ok);

					throw new InvalidArgumentException($"Phone number {_sharedPhoneNumbers[i]} is invalid.");
				}
			}
        }

		private bool CheckUserLoggedIn()
		{
			if (_doctorRepository.Get().IsLoggedIn)
				return true;

			_userDialogs.Alert(
				Resources.YouMustLoginToShareContactWithResa,
				Resources.LoginError,
			    Resources.Ok);
			return false;
		}

		private bool CheckNotAddingResaPhoneNumbers()
		{
			bool hasResaPhoneNumber = false;
			foreach (string phoneNumber in _sharedPhoneNumbers)
			{
				if (_doctorRepository.Get().Contacts.Any(c => c.PhoneNumber == phoneNumber && c.IsResaContact))
				{
					hasResaPhoneNumber = true;
					break;
				}
			}

			if (!hasResaPhoneNumber)
				return true;

			_userDialogs.Alert(
				Resources.ResaIsAmongSharedContactsErrorMessage,
				Resources.NotAbleSharingResaNumber,
			    Resources.Ok);

			return false;
		}

		private void ShowIsPatientPhoneNumber()
		{
			var config = new ActionSheetConfig();
			config.SetTitle(Resources.SharingNumberWithResa);
			config.SetMessage($"{Resources.NumbersSmart} {_sharedPhoneNumbers.ToString(Environment.NewLine, true)}");
			config.SetCancel(Resources.Cancel);
			config.SetDestructive(Resources.IsMyPatient, OnPatientPhoneNumberAccept);
			config.Add(Resources.IsNotMyPatient, OnPatientPhoneNumberDeny);
			_userDialogs.ActionSheet(config);
		}

		private void OnPatientPhoneNumberDeny()
		{
			try
			{
				_doctorRepository
					.Get()
					.AddOrUpdateContactsAsync(_sharedPhoneNumbers.Select(p => Contact.Allowed(p)).ToList()).WaitWithUnwrappedExceptions();

				_doctorRepository.Update();
				_unitOfWork.Commit();
			}
			catch
			{
				ShowInternalError();
			}
		}

		private async void OnPatientPhoneNumberAccept()
		{
			try
			{
                var doctor = _doctorRepository.Get();

                await doctor.AddOrUpdateContactsAsync(_sharedPhoneNumbers.Select(p => Contact.Blocked(p)).ToList());

                _doctorRepository.Update();
                _unitOfWork.Commit();

                if (!_callBlockAndIdentification.IsBlockingEnabled)
                    _userDialogs.Alert(
                        Resources.ResaHasNotCallBlockingPermissionMessage,
                        Resources.CallBlockingPermission,
                            Resources.Ok);
            }
			catch
			{
				ShowInternalError();
			}
		}

		private void ShowInternalError()
		{
			_userDialogs.Alert(
				Resources.InternalError,
				Resources.InternalError,
				Resources.Close);
		}

		#endregion

		#region Fields

		private readonly IDoctorRepository _doctorRepository;

		private readonly IAppUpdateRepository _appUpdateRepository;

		private readonly ICallBlockAndIdentification _callBlockAndIdentification;

		private readonly IUnitOfWork _unitOfWork;

		private readonly IConfigiOS _configiOS;

		private readonly IUserDialogs _userDialogs;

		private string[] _sharedPhoneNumbers;

		private const string QueryStringPattern = "phoneNumbers=\"\\[(?<CommaSeperatedPhoneNumbers>.*)\\]\"";

		#endregion
	}
}