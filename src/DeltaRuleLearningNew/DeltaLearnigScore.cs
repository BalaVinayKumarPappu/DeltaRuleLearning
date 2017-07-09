using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;

namespace DeltaLearning
{
    public class DeltaLearningScore : IScore
    {
        public double[] Errors { get; set; } 

        public double[] Weights { get; set; }

    }
}
