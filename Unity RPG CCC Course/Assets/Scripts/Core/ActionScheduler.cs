using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCCC.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction oldAction;
        IAction currentAction;
        public void StartAction(IAction action)
        {
            currentAction = action;
            if (oldAction != null && oldAction != currentAction) 
            {
                oldAction.Cancel();
            }           
            oldAction = action;     
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}