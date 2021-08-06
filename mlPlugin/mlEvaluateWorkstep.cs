using System;
using System.IO;

using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.DomainObject.PillarGrid;
using Slb.Ocean.Basics;
using System.Collections.Generic;

namespace mlPlugin
{
    /// <summary>
    /// This class contains all the methods and subclasses of the mlEvaluateWorkstep.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class mlEvaluateWorkstep : Workstep<mlEvaluateWorkstep.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>

        protected override mlEvaluateWorkstep.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
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
                return "40a23eab-f97e-44cc-b6bd-6cd42426c725";
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
                if (this.arguments.LabelProperty == null && this.arguments.RegressLabelProperty==null)
                {
                    PetrelLogger.InfoOutputWindow("未输入有效的LabelProperty");
                    return;
                }
                List<Property> inputPropertyList = new List<Property>();
                if (this.arguments.InputProperty1 != null && !inputPropertyList.Contains(arguments.InputProperty1))
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
                // 填入数据
                Index3 numCellsIJK = new Index3();
                if(arguments.LabelProperty == null)
                {
                    numCellsIJK = this.arguments.RegressLabelProperty.NumCellsIJK;
                }
                else
                {
                    numCellsIJK = this.arguments.LabelProperty.NumCellsIJK;
                }


                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


                using (StreamWriter sw = new StreamWriter(Path.Combine(desktopPath, "mlDataset")))
                {
                    int addtion = 3;

                    if (arguments.LabelProperty != null)
                    {
                        addtion++;
                    }
                    if (arguments.RegressLabelProperty != null)
                    {
                        addtion++;
                    }

                    sw.WriteLine("PETREL: Properties");
                    // i j k 和 litho
                    sw.WriteLine("" + (inputPropertyList.Count + addtion));
                    sw.WriteLine("i_index unit1 scale1");
                    sw.WriteLine("j_index unit1 scale1");
                    sw.WriteLine("k_index unit1 scale1");

                    foreach (Property p in inputPropertyList)
                    {
                        sw.WriteLine(p.Name + " unit1 scale1");
                    }

                    if (arguments.LabelProperty != null)
                    {
                        sw.WriteLine(arguments.LabelProperty.Name + " unit1 scale1");
                    }
                    if (arguments.RegressLabelProperty != null)
                    {
                        sw.WriteLine(arguments.RegressLabelProperty.Name + " unit1 scale1");
                    }
                    

                    for (int i = 0; i < numCellsIJK.I; ++i)
                    {
                        for (int j = 0; j < numCellsIJK.J; ++j)
                        {
                            for (int k = 0; k < numCellsIJK.K; ++k)
                            {
                                string valueString = "" + i + " " + j + " " + k;
                                foreach (Property p in inputPropertyList)
                                {
                                    float p_value = p[i, j, k];
                                    if (float.IsNaN(p_value))
                                    {
                                        valueString += " -99.00";
                                    }
                                    else
                                    {
                                        valueString += " " + p_value;
                                    }

                                }
                                if (arguments.LabelProperty != null)
                                {
                                    int l_value = arguments.LabelProperty[i, j, k];
                                    if (l_value < 0 || l_value > 1)
                                    {
                                        valueString += " -99.00";
                                    }
                                    else
                                    {
                                        valueString += " " + l_value;
                                    }
                                }
                                if (arguments.RegressLabelProperty != null)
                                {
                                    float p_value = arguments.RegressLabelProperty[i, j, k];
                                    if (float.IsNaN(p_value))
                                    {
                                        valueString += " -99.00";
                                    }
                                    else
                                    {
                                        valueString += " " + p_value;
                                    }
                                }
                               
                                sw.WriteLine(valueString);
                            }
                        }
                    }
                }

                System.Diagnostics.Process proc = System.Diagnostics.Process.Start(Path.Combine(desktopPath, @"main\main.exe"));
            }
     
        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for mlEvaluateWorkstep.
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

            private Property regressLabelProperty;

            public Property RegressLabelProperty
            {
                internal get { return this.regressLabelProperty; }
                set { this.regressLabelProperty = value; }
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
        /// Gets the description of the mlEvaluateWorkstep
        /// </summary>
        public IDescription Description
        {
            get { return mlEvaluateWorkstepDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the mlEvaluateWorkstep.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class mlEvaluateWorkstepDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private static mlEvaluateWorkstepDescription instance = new mlEvaluateWorkstepDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static mlEvaluateWorkstepDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of mlEvaluateWorkstep
            /// </summary>
            public string Name
            {
                get { return "数据分析与算法评估"; }
            }
            /// <summary>
            /// Gets the short description of mlEvaluateWorkstep
            /// </summary>
            public string ShortDescription
            {
                get { return ""; }
            }
            /// <summary>
            /// Gets the detailed description of mlEvaluateWorkstep
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