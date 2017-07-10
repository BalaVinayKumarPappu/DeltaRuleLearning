using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using DeltaLearning;
using System.IO;

namespace DeltaLearning
{
    /// <summary>
    /// Defining Input and Output values
    /// 
    /// </summary>
    public class DeltaLearning : IPipelineModule<double[], double[]>
    {
        static int I = 1000;//Number of Iterations
        static double[] Input = new double[I];
        static double[] Desired = new double[I];
        double[] W = new double[10] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };//Initial Weights
        static double[] TestInput = new double[I];
        static double[] TestDesired = new double[I];
        static int M = 10;//order of filter
        static double mu = 0.2f;//convergence rate
        static double[] H;//main system to convolution
        double[] errorsReduced = new double[I];

        /// <summary>
        /// Prediction for the model
        /// </summary>
        /// <param name="data"> Input values from the file to predict </param>
        /// <param name="ctx">Context <seealso cref"LearningFoundation.IContext"></param>
        /// <returns></returns>
        public double[] Predict(double[] testinputdata, IContext ctx)
        {
            
            Array.Reverse(W);//Inversing of  Predicted system coefficients. 
            for (int i = 0; i < testinputdata.Length; i++)
                for (int j = 0; j < M; j++)
                    if (i - j >= 0)
                        TestDesired[i] += testinputdata[i - j] * W[j];//Desired Test Output

            return TestDesired;
        }
        /// <summary>
        /// Runinng to update the weights to model the filter
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx">Context <seealso cref"LearningFoundation.IContext"></param>
        /// <returns></returns>
        public double[] Run(double[] data, IContext ctx)
        {
            double x, y;
            double[] to = null;
            double[] ti = null;
            H = data;
            preInitialize();
            initialize();
            Train(data, ctx);
            using (var fs = File.OpenRead(@"H_Test.csv"))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    ti = new double[values.Length];
                    to= new double[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        var val = values[i].Split(' ');
                        double.TryParse(val[0], out x);
                        double.TryParse(val[1], out y);

                        ti[i] = x;//Input data for Prediction 
                        to[i] = y;//Output data for prediction
                    }
                }
            }
            return Predict(ti, ctx);


        }
        /// <summary>
        /// Training the filter coefficients of the adaptive filter model 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx">Context <seealso cref"LearningFoundation.IContext"></param>
        /// <returns></returns>
        public IScore Train(double[] data, IContext ctx)
        {
            if (ctx.Score as DeltaLearningScore == null)
                ctx.Score = new DeltaLearningScore();

            List<double> errorList = new List<double>();
            List<double> errorDetailsList = new List<double>();
          
           
            int T;
            double Y, D, E;
            double[] X = new double[10] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            byte[] test = new byte[100];
            for (T = 0; T < I; T++)
            {
                for (int m = T; m > T - M; m--)
                {
                    if (m >= 0)
                        X[M + (m - T) - 1] = Input[m];  //X new input sample for LMS filter
                    else break;
                }

                D = Desired[T];                 //desired system signal

                Y = 0;                      //filter’output set to zero
                for (int i = 0; i < M; i++)
                {
                    Y += (W[i] * X[i]);         //calculate filter output    
                    Console.WriteLine("Weight Considered = " + W[i]);
                    Console.WriteLine("Input Filter Value = " + X[i]);
                    Console.WriteLine("Calculated Value = " + Y);
                    Console.WriteLine("=======================");
                    //co[i] = Y;

                }
                E = D - Y;
                Console.WriteLine("Desired Value = " + D);
                Console.WriteLine("Calculated Value = " + Y);
                Console.WriteLine("Error Value = " + E);
                errorsReduced[T] = E; //calculate error signal

                for (int i = 0; i < M; i++)
                {
                    W[i] = W[i] + (mu * E * X[i]);

                }
                //update filter coefficients
                Console.WriteLine("y_out" + (float)T + "=" + Y);
                Console.WriteLine("error" + (float)T + "=" + E);


            }
            for (int i = 0; i < M; i++)
            {
                Console.WriteLine("weights" + T + "=" + W[i]);
                Console.WriteLine("inside forloop");
            }

            ctx.Score.Errors = errorsReduced;
            ctx.Score.Weights = W;


            return ctx.Score;
        }

        /// <summary>
        /// initializing the algorithm
        /// </summary>
        /// <param name="Itr"></param>
        public DeltaLearning(int Itr)
        {
            I = Itr;
        }
        /// <summary>
        /// Assigning Inout and desired output values
        /// </summary>
        private static void preInitialize()
        {
            Console.WriteLine("inside preinitialize");
            for (int i = 0; i < I; i++)
            {
                Input[i] = 0;
                Desired[i] = 0;
            }
        }

        /// <summary>
        /// Initializing the input and desired output
        /// </summary>
      
        private static void initialize()
        {
            Console.WriteLine("inside intialize");
            Random rand = new Random();
            for (int i = 0; i < I; i++)
                Input[i] = rand.NextDouble();

            for (int i = 0; i < I; i++)
                for (int j = 0; j < M; j++)
                    if (i - j >= 0)
                        Desired[i] += Input[i - j] * H[j];
        }

    }
}
