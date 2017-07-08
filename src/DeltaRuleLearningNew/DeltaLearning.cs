using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using DeltaLearning;
using System.IO;

namespace DeltaLearning
{

    public class DeltaLearning : IPipelineModule<double[], double[]>
    {
        static int I = 1000;
        static double[] Input = new double[I];
        static double[] Desired = new double[I];
        double[] W = new double[10] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        static double[] TestInput = new double[I];
        static double[] TestDesired = new double[I];
        static int M = 10;
        static double mu = 0.2f;
        static double[] H;
        double[] errorsReduced = new double[I];


        public double[] Predict(double[] data, IContext ctx)
        {
            
            Array.Reverse(W);
            for (int i = 0; i < data.Length; i++)
                for (int j = 0; j < M; j++)
                    if (i - j >= 0)
                        TestDesired[i] += data[i - j] * W[j];

            return TestDesired;
        }

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

                        ti[i] = x;
                        to[i] = y;
                    }
                }
            }
            return Predict(ti, ctx);


        }

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

                D = Desired[T];                 //desired signal

                Y = 0;                      //filter’output set to zero
                                            //X = Input;


                //co = new double[Input.Length];
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
                //w = W[T];
                Console.WriteLine("y_out" + (float)T + "=" + Y);
                Console.WriteLine("error" + (float)T + "=" + E);


            }
            for (int i = 0; i < M; i++)
            {
                Console.WriteLine("weights" + T + "=" + W[i]);
                Console.WriteLine("inside forloop");
                // weights.Write(test, T, (int)W[i]);
            }

            ctx.Score.Errors = errorsReduced;
            ctx.Score.Weights = W;


            return ctx.Score;
        }


        public DeltaLearning(int Itr)
        {
            I = Itr;
        }

        private static void preInitialize()
        {
            Console.WriteLine("inside preinitialize");
            for (int i = 0; i < I; i++)
            {
                Input[i] = 0;
                Desired[i] = 0;
            }
        }

      
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
