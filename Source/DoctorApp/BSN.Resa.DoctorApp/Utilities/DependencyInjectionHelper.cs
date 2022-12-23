using System.Collections.Generic;
using Unity;

namespace BSN.Resa.DoctorApp.Utilities
{
    public static class DependencyInjectionHelper
    {
        public static T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Container.ResolveAll<T>();
        }

        public static IUnityContainer Container;
    }
}