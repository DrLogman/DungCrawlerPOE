using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPointer : MonoBehaviour
{
    public float angle;
    float ControllerX, ControllerY;
    public Vector3 mousePos;
    public bool connected = false;
    static public bool staticConnected = false;
    [SerializeField] SpriteRenderer sprite;
    // Update is called once per frame
    void Update ()
    {
        if(!connected)
        {
            sprite.enabled = false;
            MouseUpdate();
        } else
        {
            sprite.enabled = true;
            ControllerUpdate();
        }
        staticConnected = connected;
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
        if(Mathf.Abs(Input.GetAxis("ControllerX")) > 0.01)
        {
            ControllerX = Input.GetAxis("ControllerX");
        }
        if (Mathf.Abs(Input.GetAxis("ControllerY")) > 0.01)
        {
            ControllerY = Input.GetAxis("ControllerY");
        }

        float angle = Mathf.Atan2(ControllerY, ControllerX) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            string[] controllers = Input.GetJoystickNames();

            if (controllers.Length > 0 && controllers[0].Length > 1)
            {
                connected = true;
                Debug.Log("Connected");
                Debug.Log(controllers[0]);
            }
            else
            {
                connected = false;
                Debug.Log("Disconnected");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(CheckForControllers());
    }


}
