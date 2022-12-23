using Microsoft.Practices.Unity;
using System;
using System.ComponentModel;
using BSN.Resa.Core.Commons;

namespace BSN.Resa.Commons.Helpers
{
    public class AppSettings
    {
        private static IUnityContainer di;

        private static AppSettings inst;

        private AppSettings()
        { }

        public static AppSettings GetInstance()
        {
            return inst = inst ?? new AppSettings();
        }

        public static AppSettings GetInstance(IUnityContainer DI)
        {
            di = di ?? DI;
            return GetInstance();
        }

        /// <summary>
        /// Gets the value associated with the given key defined in appSettings section of web.config file. if not found the defaultValue will return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue">the value to be returned if the key was not found</param>
        /// /// <exception cref="AppSettingNotFoundException"></exception>
        public T Get<T>(string key, T defaultValue) where T : IEquatable<T>
        {
            try
            {
                T value = GetValue<T>(key);
                return value;
            }
            catch (AppSettingNotFoundException)
            {
                return defaultValue;
            }
            catch (AppSettingCastException)
            {
                return defaultValue;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the value associated with the given key defined in appSettings section of web.config file.
        /// it throws exception if the provided key is not found, if you set the defaultValue exception wont be thrown and the defaultValue will be returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue">if set, the method wont throw exception but returns this parameter as result in the case of key not found in AppSettings </param>
        /// /// <exception cref="AppSettingNotFoundException"></exception>
        public T Get<T>(string key) where T : IEquatable<T>
        {
            T value = GetValue<T>(key);
            return value;
        }

        /// <summary>
        /// try to get value associated with the given key defined in appSettings section of .config file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet<T>(string key, out T value) where T : IEquatable<T>
        {
            try
            {
                value = GetValue<T>(key);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        private T GetValue<T>(string key) where T : IEquatable<T>
        {
            T value;

            if (di == null)
                throw new NullReferenceException($"{nameof(di)} is not initialized and it's null");

            value = default;

            IAppConfig appConfig;

            appConfig = di.Resolve<IAppConfig>();

            if (appConfig == null)
                throw new NullReferenceException($"{nameof(appConfig)} cannot be null");

            string appSetting = appConfig.Get(key);

            if (appSetting == null)
                throw new AppSettingNotFoundException($"{key} key not found");

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                value = (T)converter.ConvertFromInvariantString(appSetting);
            }
            catch (Exception ex)
            {
                throw new AppSettingCastException($"unable to cast {appSetting} to {typeof(T).Name}", ex);
            }

            return value;
        }
    }

    [Serializable]
    public class AppSettingNotFoundException : Exception
    {
        public AppSettingNotFoundException()
        { }

        public AppSettingNotFoundException(string message) : base(message)
        { }
    }

    [Serializable]
    public class AppSettingCastException : Exception
    {
        public AppSettingCastException()
        {
        }

        public AppSettingCastException(string message) : base(message)
        {
        }

        public AppSettingCastException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AppSettingCastException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}