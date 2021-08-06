using System;
using System.IO;
using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.DomainObject.PillarGrid;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Microsoft.ML.Runtime;
using Slb.Ocean.Basics;
using System.Collections.Generic;


namespace mlPlugin
{
    public class ModelData
    {
        [LoadColumn(0)]
        public int i;
        [LoadColumn(1)]
        public int j;
        [LoadColumn(2)]
        public int k;

        [LoadColumn(3)]
        public float f1;
        [LoadColumn(4)]
        public float f2;
        [LoadColumn(5)]
        public float f3;
        [LoadColumn(6)]
        public float f4;
        [LoadColumn(7)]
        public float f5;
        [LoadColumn(8)]
        public float f6;
        [LoadColumn(9)]
        public float f7;
        [LoadColumn(10)]
        public float f8;
        [LoadColumn(11)]
        public float f9;
        [LoadColumn(12)]
        public float f10;
        [LoadColumn(13)]
        public bool litho { get; set; }
    }

    public class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public bool litho { get; set; }

        public float Score { get; set; }
    }
    /// <summary>
    /// This class contains all the methods and subclasses of the mlProcessWorkstep.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class mlProcessWorkstep : Workstep<mlProcessWorkstep.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>

        protected override mlProcessWorkstep.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
        {
            return new Arguments(dataSourceManager);
        }
        /// <summary>
        /// Copies the Arguments instance.
        /// </summary>
        /// <param name="fromArgumentPackage">the source Arguments instance</param>
        /// <param name="toArgumentPackage">the target Arguments instance</param>
        protected override void CopyArgumentPackageCore(Arguments fromArgumentPackage, Arguments toArgumentPackage)
        {
            DescribedArgumentsHelper.Copy(fromArgumentPackage, toArgumentPackage);
        }

        /// <summary>
        /// Gets the unique identifier for this Workstep.
        /// </summary>
        protected override string UniqueIdCore
        {
            get
            {
                return "aac8fc86-0fa0-44f5-9d19-e351adde7ef7";
            }
        }
        #endregion

        #region IExecutorSource Members and Executor class

        /// <summary>
        /// Creates the Executor instance for this workstep. This class will do the work of the Workstep.
        /// </summary>
        /// <param name="argumentPackage">the argumentpackage to pass to the Executor</param>
        /// <param name="workflowRuntimeContext">the context to pass to the Executor</param>
        /// <returns>The Executor instance.</returns>
        public Slb.Ocean.Petrel.Workflow.Executor GetExecutor(object argumentPackage, WorkflowRuntimeContext workflowRuntimeContext)
        {
            return new Executor(argumentPackage as Arguments, workflowRuntimeContext);
        }

        public class Executor : Slb.Ocean.Petrel.Workflow.Executor
        {
            Arguments arguments;
            WorkflowRuntimeContext context;

            public Executor(Arguments arguments, WorkflowRuntimeContext context)
            {
                this.arguments = arguments;
                this.context = context;
            }

            public override void ExecuteSimple()
            {
                PetrelLogger.InfoOutputWindow("开始执行");
                //check input
                if (this.arguments.LabelProperty == null)
                {
                    PetrelLogger.InfoOutputWindow("未输入有效的LabelProperty");
                    return;
                }
                List<Property> inputPropertyList = new List<Property>();
                if(this.arguments.InputProperty1 != null && !inputPropertyList.Contains(arguments.InputProperty1)) 
                    inputPropertyList.Add(this.arguments.InputProperty1);
                if (this.arguments.InputProperty2 != null && !inputPropertyList.Contains(arguments.InputProperty2))
                    inputPropertyList.Add(this.arguments.InputProperty2);
                if (this.arguments.InputProperty3 != null && !inputPropertyList.Contains(arguments.InputProperty3))
                    inputPropertyList.Add(this.arguments.InputProperty3);
                if (this.arguments.InputProperty4 != null && !inputPropertyList.Contains(arguments.InputProperty4))
                    inputPropertyList.Add(this.arguments.InputProperty4);
                if (this.arguments.InputProperty5 != null && !inputPropertyList.Contains(arguments.InputProperty5))
                    inputPropertyList.Add(this.arguments.InputProperty5);
                if (this.arguments.InputProperty6 != null && !inputPropertyList.Contains(arguments.InputProperty6))
                    inputPropertyList.Add(this.arguments.InputProperty6);
                if (this.arguments.InputProperty7 != null && !inputPropertyList.Contains(arguments.InputProperty7))
                    inputPropertyList.Add(this.arguments.InputProperty7);
                if (this.arguments.InputProperty8 != null && !inputPropertyList.Contains(arguments.InputProperty8))
                    inputPropertyList.Add(this.arguments.InputProperty8);
                if (this.arguments.InputProperty9 != null && !inputPropertyList.Contains(arguments.InputProperty9))
                    inputPropertyList.Add(this.arguments.InputProperty9);
                if (this.arguments.InputProperty10 != null && !inputPropertyList.Contains(arguments.InputProperty10))
                    inputPropertyList.Add(this.arguments.InputProperty10);

                if (inputPropertyList.Count == 0)
                {
                    PetrelLogger.InfoOutputWindow("未输入有效的InputProperty");
                    return;
                }
                // TODO: Implement the workstep logic here.
                var mlContext = new MLContext(seed: 0);
                var dataProcessPipeline = mlContext.Transforms.Concatenate("Features", new[] { "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10" });
                var trainer = mlContext.BinaryClassification.Trainers.LightGbm(labelColumnName: "litho", featureColumnName: "Features");
                // Appending algorithm to pipeline
                var trainingPipeline = dataProcessPipeline.Append(trainer);



                // 填入数据
                Index3 numCellsIJK = this.arguments.LabelProperty.NumCellsIJK;
                PetrelLogger.InfoOutputWindow("要处理数据总共 i: " + numCellsIJK.I + " j: " + numCellsIJK.J + " k: " + numCellsIJK.K);

                //List<ModelData> inputList = new List<ModelData>();
                //List<ModelData> toPredict = new List<ModelData>();

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string inputPath = Path.Combine(desktopPath, "tempinput.csv");
                string topredPath = Path.Combine(desktopPath, "temp.csv");
                int input_count = 0;
                int toPred_count = 0;

                StreamWriter inputWriter = new StreamWriter(inputPath);
                StreamWriter toPredWriter = new StreamWriter(topredPath);


                float[] temp = new float[10];

                int litho_int = -1;
                for (int i = 0; i < numCellsIJK.I; ++i)
                {
                    for (int j = 0; j < numCellsIJK.J; ++j)
                    {
                        for (int k = 0; k < numCellsIJK.K; ++k)
                        {
                            for (int x = 0; x < temp.Length; ++x)
                            {
                                temp[x] = 0.0f;
                            }
                            int idx = 0;
                            foreach (Property p in inputPropertyList)
                            {
                                if (p.IsValid)
                                {
                                    temp[idx] = p[i, j, k];
                                    idx++;
                                }

                            }

                            litho_int = arguments.LabelProperty[i, j, k];

                            string arrayStr = "";
                            bool skip = false;
                            for (int x = 0; x < temp.Length; ++x)
                            {
                                arrayStr += " " + temp[x];
                                if (float.IsNaN(temp[x]))
                                {
                                    skip = true;
                                    break;
                                }
                            }

                            if (skip)
                            {
                                continue;
                            }


                            ModelData md = new ModelData();

                            md.i = i;
                            md.j = j;
                            md.k = k;

                            md.f1 = temp[0];
                            md.f2 = temp[1];
                            md.f3 = temp[2];
                            md.f4 = temp[3];
                            md.f5 = temp[4];
                            md.f6 = temp[5];
                            md.f7 = temp[6];
                            md.f8 = temp[7];
                            md.f9 = temp[8];
                            md.f10 = temp[9];

                            string to_write = i + "," + j + "," + k + ",";
                            for (int x = 0; x < temp.Length; ++x)
                            {
                                to_write += temp[x] + ",";
                            }

                            if (litho_int == 1)
                            {
                                md.litho = true;
                                //inputList.Add(md);
                                //PetrelLogger.InfoOutputWindow(arrayStr + " " + litho_int);
                                to_write += "1";
                                inputWriter.WriteLine(to_write);
                                input_count++;
                            }
                            else if (litho_int == 0)
                            {
                                md.litho = false;
                                //inputList.Add(md);
                                //PetrelLogger.InfoOutputWindow(arrayStr + " " + litho_int);
                                to_write += "0";
                                inputWriter.WriteLine(to_write);
                                input_count++;
                            }
                            else
                            {
                                md.litho = true;
                                //toPredict.Add(md);
                                to_write += "1";
                                toPredWriter.WriteLine(to_write);
                                toPred_count++;
                            }



                        }
                    }
                }

                inputWriter.Flush();
                inputWriter.Close();
                toPredWriter.Flush();
                toPredWriter.Close();

                PetrelLogger.InfoOutputWindow("有效数据" + input_count + " 需要预测数据: " + toPred_count);

                var data = mlContext.Data.LoadFromTextFile<ModelData>(inputPath,
                    separatorChar: ','
                );


                var trainTestData = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);
                var trainData = trainTestData.TrainSet;
                var testData = trainTestData.TestSet;

                var model = trainingPipeline.Fit(trainData);
                PetrelLogger.InfoOutputWindow("fit 模型成功");
                var testResult = model.Transform(testData);

                IEnumerable<ModelData> testResultEnumerable =
                     mlContext.Data.CreateEnumerable<ModelData>(testResult, reuseRowObject: true);

                // Iterate over each row
                foreach (ModelData row in testResultEnumerable)
                {
                    //PetrelLogger.InfoOutputWindow("test result  " + row.litho);
                }

                var metrice = mlContext.BinaryClassification.Evaluate(testResult, labelColumnName: "litho");
                PetrelLogger.InfoOutputWindow("F1 score: " + metrice.F1Score);


                //var toPredictData = mlContext.Data.LoadFromEnumerable<ModelData>(toPredict);
                //var predictResult = model.Transform(toPredictData);

                //IEnumerable<ModelData> predictResultEnumerable =
                //     mlContext.Data.CreateEnumerable<ModelData>(predictResult, reuseRowObject: true);

                var predEngine = mlContext.Model.CreatePredictionEngine<ModelData, ModelOutput>(model);
                int count = 0;
                using (ITransaction t = DataManager.NewTransaction())
                {
                    PropertyCollection pc = arguments.LabelProperty.PropertyCollection;
                    t.Lock(pc);
                    DictionaryProperty newDp = pc.CreateDictionaryProperty(arguments.LabelProperty.DictionaryTemplate);
                    newDp.Name = "predict_result";
                    PetrelLogger.InfoOutputWindow("start predicting =================");
                    //t.Lock(arguments.LabelProperty);
                    IDataView toPredData = mlContext.Data.LoadFromTextFile<ModelData>(topredPath,
                          separatorChar: ','
                      );
                    IEnumerable<ModelData> toPredEnumerable =
                    mlContext.Data.CreateEnumerable<ModelData>(toPredData, reuseRowObject: true);
                    foreach (ModelData md_iter in toPredEnumerable)
                    {

                        //PetrelLogger.InfoOutputWindow(" to predict " + md_iter.f1 + " " + md_iter.f2 + " " + md_iter.f3 + " " + md_iter.f4 + " " + md_iter.f5 + " " + md_iter.f6 + " " + md_iter.f7 + " ");
                        ModelOutput predictionResult = predEngine.Predict(md_iter);
                        //PetrelLogger.InfoOutputWindow(count+" predictionResult " + md_iter.litho);
                        if (predictionResult.litho)
                        {
                            newDp[md_iter.i, md_iter.j, md_iter.k] = 1;
                        }
                        else
                        {
                            newDp[md_iter.i, md_iter.j, md_iter.k] = 0;
                        }
                        count++;
                    }
                    IEnumerable<ModelData> inputEnumerable =
                    mlContext.Data.CreateEnumerable<ModelData>(data, reuseRowObject: true);
                    foreach (ModelData md_iter in inputEnumerable)
                    {
                        if (md_iter.litho)
                        {
                            newDp[md_iter.i, md_iter.j, md_iter.k] = 1;
                        }
                        else
                        {
                            newDp[md_iter.i, md_iter.j, md_iter.k] = 0;
                        }
                        count++;
                    }

                }

                PetrelLogger.InfoOutputWindow("预测结束 ");
            }
        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for mlProcessWorkstep.
        /// Each public property is an argument in the package.  The name, type and
        /// input/output role are taken from the property and modified by any
        /// attributes applied.
        /// </summary>
        public class Arguments : DescribedArgumentsByReflection
        {
            public Arguments()
                : this(DataManager.DataSourceManager)
            {                
            }

            public Arguments(IDataSourceManager dataSourceManager)
            {
            }

            private DictionaryProperty labelProperty;

            public DictionaryProperty LabelProperty
            {
                internal get { return this.labelProperty; }
                set { this.labelProperty = value; }
            }


            private Property inputProperty1;
            private Property inputProperty2;
            private Property inputProperty3;
            private Property inputProperty4;
            private Property inputProperty5;
            private Property inputProperty6;
            private Property inputProperty7;
            private Property inputProperty8;
            private Property inputProperty9;
            private Property inputProperty10;

            public Property InputProperty1
            {
                internal get { return this.inputProperty1; }
                set { this.inputProperty1 = value; }
            }
            public Property InputProperty2
            {
                internal get { return this.inputProperty2; }
                set { this.inputProperty2 = value; }
            }
            public Property InputProperty3
            {
                internal get { return this.inputProperty3; }
                set { this.inputProperty3 = value; }
            }
            public Property InputProperty4
            {
                internal get { return this.inputProperty4; }
                set { this.inputProperty4 = value; }
            }
            public Property InputProperty5
            {
                internal get { return this.inputProperty5; }
                set { this.inputProperty5 = value; }
            }
            public Property InputProperty6
            {
                internal get { return this.inputProperty6; }
                set { this.inputProperty6 = value; }
            }
            public Property InputProperty7
            {
                internal get { return this.inputProperty7; }
                set { this.inputProperty7 = value; }
            }
            public Property InputProperty8
            {
                internal get { return this.inputProperty8; }
                set { this.inputProperty8 = value; }
            }
            public Property InputProperty9
            {
                internal get { return this.inputProperty9; }
                set { this.inputProperty9 = value; }
            }
            public Property InputProperty10
            {
                internal get { return this.inputProperty10; }
                set { this.inputProperty10 = value; }
            }



        }
    
        #region IAppearance Members
        public event EventHandler<TextChangedEventArgs> TextChanged;
        protected void RaiseTextChanged()
        {
            this.TextChanged?.Invoke(this, new TextChangedEventArgs(this));
        }

        public string Text
        {
            get { return Description.Name; }
            private set 
            {
                // TODO: implement set
                this.RaiseTextChanged();
            }
        }

        public event EventHandler<ImageChangedEventArgs> ImageChanged;
        protected void RaiseImageChanged()
        {
            this.ImageChanged?.Invoke(this, new ImageChangedEventArgs(this));
        }

        public System.Drawing.Bitmap Image
        {
            get { return PetrelImages.Modules; }
            private set 
            {
                // TODO: implement set
                this.RaiseImageChanged();
            }
        }
        #endregion

        #region IDescriptionSource Members

        /// <summary>
        /// Gets the description of the mlProcessWorkstep
        /// </summary>
        public IDescription Description
        {
            get { return mlProcessWorkstepDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the mlProcessWorkstep.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class mlProcessWorkstepDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private static mlProcessWorkstepDescription instance = new mlProcessWorkstepDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static mlProcessWorkstepDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of mlProcessWorkstep
            /// </summary>
            public string Name
            {
                get { return "数据预测(分类)"; }
            }
            /// <summary>
            /// Gets the short description of mlProcessWorkstep
            /// </summary>
            public string ShortDescription
            {
                get { return ""; }
            }
            /// <summary>
            /// Gets the detailed description of mlProcessWorkstep
            /// </summary>
            public string Description
            {
                get { return ""; }
            }

            #endregion
        }
        #endregion


    }
}