using System;
using elZach.Common;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public PushButton button;
    public AnimatableChildren boxTutorial, tableTutorial, buttonTutorial;
    private enum Stage{None, Box, Table, Button}

    private Stage stage = Stage.None;

    private void OnEnable()
    {
        boxTutorial.SetTo(0);
        tableTutorial.SetTo(0);
        buttonTutorial.SetTo(0);
    }

    private int stickyBehaviourAtStartOfRound;
    private int stickyBehaviourAtStartOfStage;
    private int creationBehavioursAtStartOfStage;
    void Update()
    {
        switch (stage)
        {
            case Stage.None:
                stickyBehaviourAtStartOfRound = StickySurface.active.Count;
                creationBehavioursAtStartOfStage = CreationBehaviour.active.Count;
                boxTutorial.PlayAt(1);
                stage = Stage.Box;
                break;
            case Stage.Box:
                if (CreationBehaviour.active.Count > creationBehavioursAtStartOfStage)
                {
                    boxTutorial.PlayAt(0);
                    tableTutorial.PlayAt(1);
                    stage = Stage.Table;
                    stickyBehaviourAtStartOfStage = StickySurface.active.Count;
                }
                break;
            case Stage.Table:
                if (StickySurface.active.Count > stickyBehaviourAtStartOfRound + 3)
                {
                    tableTutorial.PlayAt(0);
                    buttonTutorial.PlayAt(1);
                    waiting = true;
                    button.OnPush.AddListener(WaitForButton);
                    
                    stage = Stage.Button;
                } else if (StickySurface.active.Count > stickyBehaviourAtStartOfStage)
                {
                    tableTutorial.PlayAt(0);
                    boxTutorial.PlayAt(1);
                    creationBehavioursAtStartOfStage = CreationBehaviour.active.Count;
                    stage = Stage.Box;
                }
                break;
            case Stage.Button:
                if (!waiting)
                {
                    buttonTutorial.PlayAt(0);
                    stage = Stage.None;
                }
                break;
        }
    }

    private bool waiting = false;
    void WaitForButton()
    {
        waiting = false;
        button.OnPush.RemoveListener(WaitForButton);
    }
}
