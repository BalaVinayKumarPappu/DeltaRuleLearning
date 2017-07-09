using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;

namespace DeltaLearning
{
    public static class DeltaLearningExtension
    {
        public static LearningApi UseDeltaRuleLearning(this LearningApi api, double learningRate, int iterations)
        {

            //Invoking the class of the algorithm to use
            var alg = new DeltaLearning(iterations);

            //now we add this algorithm(new instance of the class) to the learning api
            api.AddModule(alg, "Delta Learning");

            //after adding the instance to the api, return the api
            return api;
        }

        static void Main(string[] args)
        {

            Console.ReadLine();
        }
    }
}
