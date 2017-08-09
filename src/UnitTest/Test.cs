using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LearningFoundation;
using LearningFoundation.DataMappers;
using DeltaLearning;
using System.IO;
using System.Diagnostics;

namespace UnitTest
{
    public class deltarulelearningtests
    {
        double[] H,ti,to,I,O  = null;
        double x,y;
        


        /// <summary>
        /// Performs the DeltaRule Learning  on specified dataset with 1000 iteration and 0.2f learning rate.
        /// </summary>
        [Fact]
        public void DeltaRuleLearning_Test_iterations_1000_learningrate_02()
        {
            loadRealDataSample();// System coefficients initialization.

            var desc = loadMetaData();// Description of the system.
            LearningApi api = new LearningApi(desc);

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<double[,], double[,]>((input, ctx) =>
            {
                return loadRealDataSample(); // return actual System coefficients data
            });

            // run input = UseActionModule output 
            //run Delta Rule for 1000 iterations with learningRate=0.2
            api.UseDeltaRuleLearning(0.2, 1000); 
            var result = api.Run() as double[];
            Debug.WriteLine("************ Output Predictions***********");
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != 0)
                {
                    Debug.WriteLine(result[i]);
                }
            }
            using (var fs = File.OpenRead(@"H_Test.csv"))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    ti = new double[values.Length];
                    to = new double[values.Length];
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
             for (int i = 0; i < to.Length; i++)
             {  
                //Testing of Test data with Predicted System model 
                Assert.Equal(Math.Round(result[i], 4), Math.Round(to[i], 4));
            }   
            
                    
        }


        /// <summary>
        /// Performs the DeltaRule Learning  on specified dataset with 1000 iteration and 0.2f learning rate.
        /// </summary>
        [Fact]
        public void DeltaRuleLearning_Test_iterations_1000_3GapTest()
        {
            loadRealDataSample();// System coefficients initialization.

            var desc = loadMetaData();// Description of the system.
            LearningApi api = new LearningApi(desc);

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<double[,], double[,]>((input, ctx) =>
            {
                return loadRealDataSample(); // return actual System coefficients data
            });

            // run input = UseActionModule output 
            //run Delta Rule for 1000 iterations with learningRate=0.2
            api.UseDeltaRuleLearning(0.2, 1000);
            var result = api.Run() as double[];
            api.Algorithm.Predict
            Debug.WriteLine("************ Output Predictions***********");
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != 0)
                {
                    Debug.WriteLine(result[i]);
                }
            }
            using (var fs = File.OpenRead(@"H_Input1.csv"))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    ti = new double[values.Length];
                    to = new double[values.Length];
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
            var test_data = new double[ti.Length][];

            for (int i1 = 0; i1 < ti.Length; i1=i1+3)
            {
                test_data[i1] = new double[] { ti[i1], 0.0 };
            }
            result = api.Algorithm.Predict(test_data, api.Context);
            for (int i = 0; i < to.Length; i=i+3)
            {
                //Testing of Test data with Predicted System model 
                Assert.Equal(Math.Round(result[i], 4), Math.Round(to[i], 4));
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataDescriptor loadMetaData()
        {

            //create a new instance of the data descriptor class
            var des = new DataDescriptor();

            //create the descriptor features as a column of attributes
            des.Features = new Column[2];

            //define the attributes
            des.Features[0] = new Column { Id = 1, Name = "Input", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "Desired Output", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };


            //return the created descriptor
            return des;
        }


        private double[,] loadRealDataSample()
        {
            
            double inp,oup ;
            I = new double[1000];
            O = new double[1000];
            int i = 0;

            using (var fs = File.OpenRead(@"H_Input1.csv"))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(' ');
                    
                    
                        double.TryParse(values[0] , out inp);
                        double.TryParse(values[1], out oup);

                        I[i] = inp;
                        O[i] = oup;

                    i++;
                    
                }
            }

            var data = new double[I.Length,2];

             for (int i1 = 0; i1 < I.Length; i1++)
            {
                data[i1,0] = I[i1];
                data[i1, 1] = O[i1];
            }
            return data;
            
        }
    }
}
