using GymApp.Models;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Test
{
    internal class Parser
    {
        internal class MojParser
        {
            public static IEnumerable Parsiraj(string filename)
            {
                string path = $@"{AppDomain.CurrentDomain.BaseDirectory}\{filename}";
                string[] lines = File.ReadAllLines(path);
                List<TestCaseData> testCases = new List<TestCaseData>();

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] values = line.Split('\t');

                    int numberOfMonths = int.Parse(values[0]);
                    bool groupTrainings = bool.Parse(values[1]);
                    double monthlyPriceBudget = double.Parse(values[2]);
                    TrainingTime trainingTime = (TrainingTime)Enum.Parse(typeof(TrainingTime), values[3]);
                    MembershipType? expectedMembershipType;
                    if (values[4].Trim() == "null")
                        expectedMembershipType = null;
                    else
                        expectedMembershipType = (MembershipType)Enum.Parse(typeof(MembershipType), values[4]);

                    testCases.Add(new TestCaseData(numberOfMonths, groupTrainings, monthlyPriceBudget, trainingTime, expectedMembershipType));
                }

                return testCases;
            }
        }
    }
}
