# ITNOA

If you want to use two types of implementation for single interface, based on specific condition, and want to use configuration file only.
you have to use [Autofac Module](https://autofac.readthedocs.io/en/latest/configuration/modules.html)

I add some example in https://github.com/soroshsabz/TestSolution

but for further explanation I add this example below

First of all, I write a simple module for specify conditions of dependency loading, like below

```csharp
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
```

Then I write some configuration like below

```json
{
  "modules": [
    {
      "type": "AutofacHandyMVCTest.Modules.DummyModule, AutofacHandyMVCTest",
      "properties": {
          "IsDummyAUsed": false
        }
    }
  ]
}
```

as you can see, if I want to load `DummyA` I just need to make `IsDummyAUsed` properties true in configuration file.
