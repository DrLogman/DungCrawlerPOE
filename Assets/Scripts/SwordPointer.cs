using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPointer : MonoBehaviour
{
    public float angle;
    public Vector3 mousePos;
    public bool connected = false;
    // Update is called once per frame
    void Update ()
    {
        if(!connected)
        {
            MouseUpdate();
        } else
        {
            ControllerUpdate();
        }
        
    }

    void MouseUpdate()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void ControllerUpdate()
    {
        float ControllerX = Input.GetAxis("ControllerX");
        float ControllerY = Input.GetAxis("ControllerY");

        float angle = Mathf.Atan2(ControllerY, ControllerX) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();

            if (!connected && controllers.Length > 0 && controllers[0].Length > 1)
            {
                connected = true;
                Debug.Log("Connected");

            }
            else if (connected && controllers.Length == 0)
            {
                connected = false;
                Debug.Log("Disconnected");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void Awake()
    {
        StartCoroutine(CheckForControllers());
    }


}
