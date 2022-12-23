using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
	public class PeriodicCommunicationReport
	{
		public int NumberOfEstablishedCalls { get; set; }

        public int NumberOfDroppedCalls { get; set; }

        public int NumberOfFailedCalls { get; set; }

	    public int NumberOfCalls => NumberOfEstablishedCalls + NumberOfDroppedCalls + NumberOfFailedCalls;

        public int NumberOfBillableCalls { get; set; }

        public TimeSpan CumulativeDurationOfBillableCalls { get; set; }
	}
}
