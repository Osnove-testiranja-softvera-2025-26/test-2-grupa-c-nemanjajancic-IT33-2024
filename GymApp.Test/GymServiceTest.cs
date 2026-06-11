using GymApp.Exceptions;
using GymApp.Fakes;
using GymApp.Models;
using GymApp.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static GymApp.Test.Parser;

using static System.Net.WebRequestMethods;

namespace GymApp.Test
{
    //Guid example: "00000000-0000-0000-0000-000000000001"

    [TestFixture]
    public class GymServiceTest
    {
        [Test]
        [TestCase(PerformanceRank.Second, 9, 14, 3, TrainingType.Personal, 150.0)]
        [TestCase(PerformanceRank.Second, 9, 13, 3, TrainingType.Personal, 120.0)]
        [TestCase(PerformanceRank.Second, 10, 14, 3, TrainingType.Personal, 0.0)]
        [TestCase(PerformanceRank.First, 15, 5, 7, TrainingType.Group, 200.0)]
        [TestCase(PerformanceRank.First, 4, 5, 3, TrainingType.Personal, 200.0)]
        [TestCase(PerformanceRank.First, 8, 5, 3, TrainingType.Personal, 150.0)]
        [TestCase(PerformanceRank.Third, 20, 5, 3, TrainingType.Personal, 0.0)]
        public void DoStaffBonusPaymentCalculation_ProveraIznosa(PerformanceRank rank, int percentNotHeld, int freeDaysLeft, int numberOfGroupTrainings, TrainingType trainingType, double expectedAmount)
        {
            Trainer trainer = new Trainer { Id = Guid.NewGuid(), FullName = "Test Trener" };

            List<Training> trainings = new List<Training>();
            for (int i = 0; i < numberOfGroupTrainings; i++)
                trainings.Add(new Training { TrainerId = trainer.Id, Type = TrainingType.Group });
            trainings.Add(new Training { TrainerId = trainer.Id, Type = TrainingType.Personal });

            PerformanceReport report = new PerformanceReport
            {
                PerformanceRank = rank,
                PercentOfTrainingsNotHeld = percentNotHeld,
                NumberOfFreeDaysLeft = freeDaysLeft
            };

            FTS fakeTrainingService = new FTS(trainings);
            FTPS fakePerformanceService = new FTPS(report);
            FPS fakePaymentService = new FPS();

            GymService service = new GymService(fakeTrainingService, fakePerformanceService, fakePaymentService);

            service.DoStaffBonusPaymentCalculation(trainer);

            Assert.That(fakePaymentService.ReceivedBonus.Amount, Is.EqualTo(expectedAmount));
            Assert.That(fakePaymentService.ReceivedTrainerId, Is.EqualTo(trainer.Id));
        }
    [Test]
        public void DoStaffBonusPaymentCalculation_BacaException_KadaNemaTreninga()
        { 
            Trainer trainer = new Trainer { Id = Guid.NewGuid(), FullName = "Test Trener" };

            FTS fakeTrainingService = new FTS(new List<Training>());
            FTPS fakePerformanceService = new FTPS(new PerformanceReport
            {
                PerformanceRank = PerformanceRank.First,
                PercentOfTrainingsNotHeld = 0,
                NumberOfFreeDaysLeft = 10
            });
            FPS fakePaymentService = new FPS();

            GymService service = new GymService(fakeTrainingService, fakePerformanceService, fakePaymentService);

            Assert.That(
                (Action)(() => service.DoStaffBonusPaymentCalculation(trainer)),
                Throws.TypeOf<NoTrainingsInTheLastMonthException>()
            );
        }
        [TestCase(PerformanceRank.Second, 9, 14, 3, TrainingType.Personal, 150.0)]  
        [TestCase(PerformanceRank.Second, 9, 13, 3, TrainingType.Personal, 120.0)]
        [TestCase(PerformanceRank.Second, 10, 14, 3, TrainingType.Personal, 0.0)]
        [TestCase(PerformanceRank.First, 15, 5, 7, TrainingType.Group, 200.0)]
        [TestCase(PerformanceRank.First, 4, 5, 3, TrainingType.Personal, 200.0)]
        [TestCase(PerformanceRank.First, 8, 5, 3, TrainingType.Personal, 150.0)]
        [TestCase(PerformanceRank.Third, 20, 5, 3, TrainingType.Personal, 0.0)]
        public void DoStaffBonusPaymentCalculation_ProveraIznosa_NSubstitute(PerformanceRank rank,int percentNotHeld,int freeDaysLeft,int numberOfGroupTrainings,TrainingType trainingType,double expectedAmount)
        {
            Trainer trainer = new Trainer { Id = Guid.NewGuid(), FullName = "Test Trener" };
            List<Training> trainings = new List<Training>();
            for (int i = 0; i < numberOfGroupTrainings; i++)
                trainings.Add(new Training { TrainerId = trainer.Id, Type = TrainingType.Group });
            trainings.Add(new Training { TrainerId = trainer.Id, Type = TrainingType.Personal });
            PerformanceReport report = new PerformanceReport
            {
                PerformanceRank = rank,
                PercentOfTrainingsNotHeld = percentNotHeld,
                NumberOfFreeDaysLeft = freeDaysLeft
            };
            ITrainingService subTrainingService = Substitute.For<ITrainingService>();
            subTrainingService.GetTrainingsInTheLastMonth(trainer.Id).Returns(trainings);

            ITrainerPerformanceService subPerformanceService = Substitute.For<ITrainerPerformanceService>();
            subPerformanceService.GetTrainerPerformanceReport(trainer.Id).Returns(report);

            IPaymentService subPaymentService = Substitute.For<IPaymentService>();

            GymService service = new GymService(subTrainingService, subPerformanceService, subPaymentService);
            service.DoStaffBonusPaymentCalculation(trainer);

            subPaymentService.Received(1).UpdateTrainerBonusPayment(trainer.Id,Arg.Is<BonusPayment>(b => b.Amount == expectedAmount)
            );
        }
        [Test]
        public void DoStaffBonusPaymentCalculation_BacException_NSubstitute()
        {
            Trainer trainer = new Trainer { Id = Guid.NewGuid(), FullName = "Test Trener" };

            ITrainingService subTrainingService = Substitute.For<ITrainingService>();
            subTrainingService.GetTrainingsInTheLastMonth(trainer.Id).Returns(new List<Training>());

            ITrainerPerformanceService subPerformanceService = Substitute.For<ITrainerPerformanceService>();
            IPaymentService subPaymentService = Substitute.For<IPaymentService>();

            GymService service = new GymService(subTrainingService, subPerformanceService, subPaymentService);

            Assert.That(
                (Action)(() => service.DoStaffBonusPaymentCalculation(trainer)),
                Throws.TypeOf<NoTrainingsInTheLastMonthException>()
            );
        }
        [TestCaseSource(typeof(MojParser), "Parsiraj", new object[] { "Rezultat.txt" })]
        public void GetMembershipType_PICT(int numberOfMonths,bool groupTrainings,double monthlyPriceBudget,TrainingTime trainingTime,MembershipType? expectedMembershipType)
        {
            GymService service = new GymService();
            MembershipType? result = service.GetMemberhipType(numberOfMonths, groupTrainings, monthlyPriceBudget, trainingTime);
            Assert.That(result, Is.EqualTo(expectedMembershipType));
        }
    }
}

