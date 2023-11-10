using Autofac;
using AutofacHandyMVCTest.Models;

namespace AutofacHandyMVCTest.Modules
{
    public class DummyModule : Module
    {
        public bool IsDummyAUsed { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            if (IsDummyAUsed)
            {
                builder.RegisterType<DummyA>().As<IDummyModel>();
            }
            else
            {
                builder.RegisterType<DummyB>().As<IDummyModel>();
            }
        }
    }
}
