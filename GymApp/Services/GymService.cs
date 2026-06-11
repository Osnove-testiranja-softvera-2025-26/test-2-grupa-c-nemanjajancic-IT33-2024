

using GymApp.Exceptions;
using GymApp.Fakes;
using GymApp.Models;
using System.Collections.Generic;

namespace GymApp.Services
{
    public class GymService
    {
        private readonly IPaymentService _paymentService;
        private readonly ITrainingService _trainingService;
        private readonly ITrainerPerformanceService _trainerPerformanceService;

        public GymService()
        {
        }

        public GymService(ITrainingService subTrainingService, ITrainerPerformanceService subPerformanceService, IPaymentService subPaymentService)
        {
        }

        public GymService(FTS fakeTrainingService, FTPS fakePerformanceService, FPS fakePaymentService)
        {
            FakeTrainingService = fakeTrainingService;
            FakePerformanceService = fakePerformanceService;
            FakePaymentService = fakePaymentService;
        }

        public FTS FakeTrainingService { get; }
        public FTPS FakePerformanceService { get; }
        public FPS FakePaymentService { get; }

        public void DoStaffBonusPaymentCalculation(Trainer trainer)
        {
            
            List<Training> trainings = _trainingService.GetTrainingsInTheLastMonth(trainer.Id);
            if(trainings.Count == 0)
            {
                throw new NoTrainingsInTheLastMonthException("Bonus payment cannot be calculated");
            }
            int numberOfGroupTrainigs = trainings.FindAll(x => x.Type.Equals(TrainingType.Group)).Count;
            PerformanceReport performanceReport = _trainerPerformanceService.GetTrainerPerformanceReport(trainer.Id);
            
            BonusPayment bonus = new BonusPayment();

            if (performanceReport.PerformanceRank.Equals(PerformanceRank.Second) && performanceReport.PercentOfTrainingsNotHeld < 10)
            {
                if (performanceReport.NumberOfFreeDaysLeft > 13)
                {
                    bonus.Amount = 150.0;
                }
                else
                {
                    bonus.Amount = 120.0;
                }
            }
            else if (performanceReport.PerformanceRank.Equals(PerformanceRank.First))
            {
                if (numberOfGroupTrainigs >= 7 || performanceReport.PercentOfTrainingsNotHeld < 5)
                {
                    bonus.Amount = 200.0;
                }
                else
                {
                    bonus.Amount = 150.0;
                }
            }
            else
            {
                bonus.Amount = 0;
            }

            _paymentService.UpdateTrainerBonusPayment(trainer.Id, bonus);
        }

        public MembershipType? GetMemberhipType(int numberOfMonths, bool groupTrainings, double monthlyPriceBudget, TrainingTime trainingTime)
        {
            if (trainingTime.Equals(TrainingTime.WholeDay))
            {
                if (numberOfMonths >= 5 && monthlyPriceBudget > 20.0)
                {
                    if (groupTrainings)
                        return MembershipType.TypeA;
                    return MembershipType.TypeB;
                }
                return null;
            }
            else if (trainingTime.Equals(TrainingTime.OnlyMorning))
            {
                if (numberOfMonths > 12 || monthlyPriceBudget > 15.0)
                    return MembershipType.TypeC;
                return null;
            }

            return MembershipType.TypeD;
        }
    }
}
