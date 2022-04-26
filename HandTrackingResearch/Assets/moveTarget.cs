using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTarget : MonoBehaviour
{
    private bool IKForward= false;
    private bool IKBackward = false;

    private bool IKUp = false;
    private bool IKDown = false;

    private bool IKLeft = false;
    private bool IKRight = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IKForward) {
            moveIKTargeTForward();
        }
        if (IKBackward) {
            moveIKTargeTBackwards();
        }
        if (IKUp)
        {
            moveIKTargeTForward();
        }
        if (IKDown)
        {
            moveIKTargeTBackwards();
        }
        if (IKLeft)
        {
            moveIKTargeTLeft();
        }
        if (IKRight)
        {
            moveIKTargeTRight();
        }
    }
    public void moveIKTargeTForward()
    {
        this.transform.localPosition = this.transform.localPosition + new Vector3(0, .02f * 1, 0);
    }
    public void moveIKTargeTBackwards()
    {
        this.transform.localPosition = this.transform.localPosition + new Vector3(0, -.02f * 1, 0);
    }
    public void moveIKTargeTLeft()
    {
        this.transform.localPosition = this.transform.localPosition + new Vector3(.02f * 1, 0, 0);
    }
    public void moveIKTargeTRight()
    {
        this.transform.localPosition = this.transform.localPosition + new Vector3(-.02f * 1, 0, 0);
    }
    public void moveIKTargeTUp()
    {
        this.transform.localPosition = this.transform.localPosition + new Vector3(0, 0, -.02f * 1);
    }
    public void moveIKTargeTDown()
    {
        this.transform.localPosition = this.transform.localPosition + new Vector3(0, 0, -.02f * 1);
    }

    public void SetIKForward(bool state) {
        IKForward = state;
    }
    public void SetIKBackward(bool state)
    {
        IKBackward = state;
    }

    public void SetIKUp(bool state)
    {
        IKUp = state;
    }
    public void SetIKDown(bool state)
    {
        IKDown = state;
    }

    public void SetIKLeft(bool state)
    {
        IKLeft = state;
    }
    public void SetIKRight(bool state)
    {
        IKRight = state;
    }
}
