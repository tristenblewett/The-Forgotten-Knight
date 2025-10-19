using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
    public static event System.Action<Lever> OnLeverPulled;
    private bool isActivated = false;

    protected override void Interact()
    {
        if(!isActivated)
        {
            isActivated = true;
            Debug.Log("Lever Pulled: " + gameObject.name);
            LeverPuzzleManager.Instance.OnLeverPulled(this);

            Invoke(nameof(ResetLever), 10f);
        }
    }

    private void ResetLever()
    {
        isActivated = false;
    }
}
