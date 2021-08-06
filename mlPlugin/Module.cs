using System;
using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;

namespace mlPlugin
{
    /// <summary>
    /// This class will control the lifecycle of the Module.
    /// The order of the methods are the same as the calling order.
    /// </summary>
    public class Module : IModule
    {
        private Process m_mlprocessregressworkstepInstance;
        private Process m_mlevaluateworkstepInstance;
        private Process m_mlprocessworkstepInstance;
        public Module()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region IModule Members

        /// <summary>
        /// This method runs once in the Module life; when it loaded into the petrel.
        /// This method called first.
        /// </summary>
        public void Initialize()
        {
            // TODO:  Add Module.Initialize implementation
        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the not UI related components.
        /// (eg: datasource, plugin)
        /// </summary>
        public void Integrate()
        {
            // Register mlPlugin.mlProcessRegressWorkstep
            mlPlugin.mlProcessRegressWorkstep mlprocessregressworkstepInstance = new mlPlugin.mlProcessRegressWorkstep();
            PetrelSystem.WorkflowEditor.Add(mlprocessregressworkstepInstance);
            m_mlprocessregressworkstepInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(mlprocessregressworkstepInstance);
            PetrelSystem.ProcessDiagram.Add(m_mlprocessregressworkstepInstance, "Plug-ins");
            // Register mlPlugin.mlEvaluateWorkstep
            mlPlugin.mlEvaluateWorkstep mlevaluateworkstepInstance = new mlPlugin.mlEvaluateWorkstep();
            PetrelSystem.WorkflowEditor.Add(mlevaluateworkstepInstance);
            m_mlevaluateworkstepInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(mlevaluateworkstepInstance);
            PetrelSystem.ProcessDiagram.Add(m_mlevaluateworkstepInstance, "Plug-ins");
            // Register mlPlugin.mlProcessWorkstep
            mlPlugin.mlProcessWorkstep mlprocessworkstepInstance = new mlPlugin.mlProcessWorkstep();
            PetrelSystem.WorkflowEditor.Add(mlprocessworkstepInstance);
            m_mlprocessworkstepInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(mlprocessworkstepInstance);
            PetrelSystem.ProcessDiagram.Add(m_mlprocessworkstepInstance, "Plug-ins");

            // TODO:  Add Module.Integrate implementation
        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the UI related components.
        /// (eg: settingspages, treeextensions)
        /// </summary>
        public void IntegratePresentation()
        {

            // TODO:  Add Module.IntegratePresentation implementation
        }

        /// <summary>
        /// This method runs once in the Module life.
        /// right before the module is unloaded. 
        /// It usually happens when the application is closing.
        /// </summary>
        public void Disintegrate()
        {
            PetrelSystem.ProcessDiagram.Remove(m_mlprocessregressworkstepInstance);
            PetrelSystem.ProcessDiagram.Remove(m_mlevaluateworkstepInstance);
            PetrelSystem.ProcessDiagram.Remove(m_mlprocessworkstepInstance);
            // TODO:  Add Module.Disintegrate implementation
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO:  Add Module.Dispose implementation
        }

        #endregion

    }


}