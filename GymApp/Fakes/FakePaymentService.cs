using GymApp.Models;
using GymApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Fakes
{
    public class FPS : IPaymentService
    {
        public BonusPayment ReceivedBonus { get; private set; }
        public Guid ReceivedTrainerId { get; private set; }

        public void UpdateTrainerBonusPayment(Guid trainerId, BonusPayment payment)
        {
            ReceivedTrainerId = trainerId;
            ReceivedBonus = payment;
        }
    }
}
