using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Net;
using Java.Security;
using Java.Security.Cert;
using Javax.Net.Ssl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Android.Net;

namespace BSN.Resa.DoctorApp.Droid
{
	// This class is used to create a custom SSLSocketFactory that accepts self-signed certificates
	//
	// https://stackoverflow.com/a/71196389/1539100
	internal class DisablerTrustProvider : Provider
    {
        private const string TRUST_PROVIDER_ALG = "DisablerTrustAlgorithm";
        private const string TRUST_PROVIDER_ID = "DisablerTrustProvider";

        public DisablerTrustProvider() : base(TRUST_PROVIDER_ID, 1, string.Empty)
        {
            var key = "TrustManagerFactory." + DisablerTrustManagerFactory.GetAlgorithm();
            var val = Java.Lang.Class.FromType(typeof(DisablerTrustManagerFactory)).Name;
            Put(key, val);
        }

        public static void Register()
        {
            Provider registered = Security.GetProvider(TRUST_PROVIDER_ID);
            if (null == registered)
            {
                Security.InsertProviderAt(new DisablerTrustProvider(), 1);
                Security.SetProperty("ssl.TrustManagerFactory.algorithm", TRUST_PROVIDER_ALG);
            }
        }

        public class DisablerTrustManager : X509ExtendedTrustManager
        {
            public override void CheckClientTrusted(X509Certificate[] chain, string authType, Socket socket) { }

            public override void CheckClientTrusted(X509Certificate[] chain, string authType, SSLEngine engine) { }

            public override void CheckClientTrusted(X509Certificate[] chain, string authType) { }

            public override void CheckServerTrusted(X509Certificate[] chain, string authType, Socket socket) { }

            public override void CheckServerTrusted(X509Certificate[] chain, string authType, SSLEngine engine) { }

            public override void CheckServerTrusted(X509Certificate[] chain, string authType) { }

            public override X509Certificate[] GetAcceptedIssuers() => Array.Empty<X509Certificate>();
        }

        public class DisablerTrustManagerFactory : TrustManagerFactorySpi
        {
            protected override void EngineInit(IManagerFactoryParameters mgrparams) { }

            protected override void EngineInit(KeyStore keystore) { }

            protected override ITrustManager[] EngineGetTrustManagers() => new ITrustManager[] { new DisablerTrustManager() };

            public static string GetAlgorithm() => TRUST_PROVIDER_ALG;
        }
    }

	static class DisablerAndroidMessageHandlerEmitter
    {
        private static Assembly _emittedAssembly = null;

        public static void Register(string handlerTypeName = "DisablerAndroidMessageHandler", string assemblyName = "DisablerAndroidMessageHandler")
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (e.Name == assemblyName)
                {
                    if (_emittedAssembly == null)
                    {
                        _emittedAssembly = Emit(handlerTypeName, assemblyName);
                    }

                    return _emittedAssembly;
                }
                return null;
            };
        }

        private static AssemblyBuilder Emit(string handlerTypeName, string assemblyName)
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(assemblyName);

            DefineDisablerAndroidMessageHandler(module, handlerTypeName);

            return assembly;
        }

        private static void DefineDisablerAndroidMessageHandler(ModuleBuilder module, string handlerTypeName)
        {
            var typeBuilder = module.DefineType(handlerTypeName, TypeAttributes.Public);
            typeBuilder.SetParent(typeof(AndroidMessageHandler));
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            var methodBuilder = typeBuilder.DefineMethod(
                "GetSSLHostnameVerifier",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(IHostnameVerifier),
                new[] { typeof(HttpsURLConnection) }
            );

            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Call, typeof(DisablerHostNameVerifier).GetMethod("Create"));
            generator.Emit(OpCodes.Ret);

            typeBuilder.CreateType();
        }
    }

    public class DisablerHostNameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {
            return true;
        }

        public static IHostnameVerifier Create() => new DisablerHostNameVerifier();
    }
}