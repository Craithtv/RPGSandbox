using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {

        IAction currentAction;

        public void StartAction(IAction action)//since mover and fighter both have monobehaviors we can substitute MB for either
        {
            if (currentAction == action) return;//stops next action is the same. so move to move wont fire the rest.
            if (currentAction != null)
            {
                print("Cancelling" + currentAction);
                currentAction.Stop();
            }
           
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);//cancels all action. called by health to detect death
        }

    }
}
