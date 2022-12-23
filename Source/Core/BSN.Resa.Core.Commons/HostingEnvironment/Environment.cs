namespace BSN.Resa.Core.Commons
{
    /// <summary>
    /// این کلاس متغیرهای محیطی را نگهداری میکند. 
    /// متغیرهای محیطی متغییرهایی هستند که بسته به محل نصب نرم افزار و نیز محیط اجرایی آن متفاوت خواهند بود.
    /// </summary>
    public static class Environment
    {
        /// <summary>
        /// این متغیر برای ذخیره و بازیابی مسیر فایل های اجرایی برنامه 
        /// مورد استفاده قرار می گیرد.
        /// از این آدرس میتوان در آدرس دهی های محلی برای کارهای ورودی/خروجی و ... 
        /// استفاده نمود
        /// </summary>
        public static string ApplicationPhysicalPath
        {
            get
            { 
              return _applicationPhysicalPath;
                
            }
            set { _applicationPhysicalPath = value; }
        }
        private static string _applicationPhysicalPath;
    }
}
