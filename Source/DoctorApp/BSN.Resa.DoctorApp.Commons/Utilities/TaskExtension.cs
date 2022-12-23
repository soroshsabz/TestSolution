using System;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Commons.Utilities
{
	public static class TaskExtension
	{
		public static void WaitWithUnwrappedExceptions(this Task task)
		{
			try
			{
				task.Wait();
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.Count == 1 && e.InnerException != null)
					throw e.InnerException;
				throw;
			}
		}

		public static T ResultWithUnwrappedExceptions<T>(this Task<T> task)
		{
			try
			{
				return task.Result;
			}
			catch (AggregateException e)
			{
				if (e.InnerException != null)
					throw e.InnerException;
				throw;
			}
		}
	}
}
