using GymApp.Models;
using GymApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Fakes
{
    public class FTS : ITrainingService
    {
        private readonly List<Training> _trainings;

        public FTS(List<Training> trainings)
        {
            _trainings = trainings;
        }

        public List<Training> GetTrainingsInTheLastMonth(Guid trainerId)
        {
            return _trainings;
        }
    }
}
