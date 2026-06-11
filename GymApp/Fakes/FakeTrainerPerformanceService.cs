using GymApp.Models;
using GymApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Fakes
{
        public class FTPS : ITrainerPerformanceService
        {
            private readonly PerformanceReport _report;

            public FTPS(PerformanceReport report)
            {
                _report = report;
            }

            public PerformanceReport GetTrainerPerformanceReport(Guid trainerId)
            {
                return _report;
            }
        }
}
