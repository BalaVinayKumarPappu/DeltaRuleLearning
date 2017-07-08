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
        double[] H,ti,to  = null;
        double x,y;
        


        /// <summary>
        /// Performs the DeltaRule Learning  on specified dataset with 10 iteration and 0.15 learning rate.
        /// </summary>
        [Fact]
        public void DeltaRuleLearning_Test_iterations_100_learningrate_013()
        {
            loadRealDataSample();

            var desc = loadMetaData();
            LearningApi api = new LearningApi(desc);

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<double[], double[]>((input, ctx) =>
            {
                return loadRealDataSample();
            });

            // run input = UseActionModule output 

            // Use mapper for data, which will extract (map) required columns 
            //api.UseDefaultDataMapper();
           
            //run logistic regression for 10 iterations with learningRate=0.13
            api.UseDeltaRuleLearning(0.13, 1000);
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
                Assert.Equal(Math.Round(result[i], 4), Math.Round(to[i], 4));
            }

            
                   
            
                    
        }

        private DataDescriptor loadMetaData()
        {

            //create a new instance of the data descriptor class
            var des = new DataDescriptor();

            //create the descriptor features as a column of attributes
            des.Features = new Column[1];

            //define the attributes
            des.Features[0] = new Column { Id = 1, Name = "FIR Filter", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };

            //return the created descriptor
            return des;
        }

        private double[] loadRealDataSample()
        {
            
            double h ;


            using (var fs = File.OpenRead(@"H_Input1.csv"))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    H = new double[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        double.TryParse(values[i] , out h);
                        H[i] = h;
                    }
                }
            }

            var data = new object[H.Length][];

            for (int i = 0; i < H.Length; i++)
            {
                Console.Write(H[i]);
                data.Append(new object[] { H[i] });
            }
            return H;
            
        }
    }
}
