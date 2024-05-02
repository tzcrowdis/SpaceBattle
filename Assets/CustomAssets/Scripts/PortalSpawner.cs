using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PortalSpawner : MonoBehaviour
{
    //controller vars
    InputDevice leftController;
    InputDevice rightController;

    bool leftTracked;
    bool rightTracked;

    Vector3 leftControllerPosition;
    Vector3 rightControllerPosition;

    Vector3 midpoint;

    Quaternion temp;
    Vector3 leftControllerRotation;
    Vector3 rightControllerRotation;
    Vector3 previousRotation;
    Vector3 currentRotation;
    Vector3 deltaRotation;

    float previousDistance = 0f;
    float currentDistance;
    float deltaDistance = 0f;

    bool leftTriggerHeld;
    bool rightTriggerHeld;
    bool leftGripHeld;
    bool rightGripHeld;

    //portal vars
    GameObject portalResource;
    GameObject portal;
    GameObject portalHolo;

    float minScale = 0.05f;
    float midScale = 0.15f;
    float maxScale = 0.25f;

    bool lockPortal = false;

    void Start()
    {
        portalResource = Resources.Load("Portal") as GameObject;
    }

    void Update()
    {
        GetControllerValues();

        if (leftTracked & rightTracked & !lockPortal)
        {
            //instantiate portal if not already
            if (portal == null)
            {
                if (Vector3.Distance(leftControllerPosition, rightControllerPosition) < 0.18f)
                {
                    if (leftGripHeld & rightGripHeld)
                    {
                        midpoint = (leftControllerPosition + rightControllerPosition + 2 * transform.GetChild(3).transform.position) / 2; //CLOSE BUT NOT PERFECT
                        portal = Instantiate(portalResource, midpoint, Quaternion.identity);
                        portal.transform.localScale = Vector3.zero;
                        var color = portal.GetComponent<Renderer>().material.color;
                        color.a = 0.0f;
                        portal.GetComponent<Renderer>().material.color = color;

                        portalHolo = Instantiate(portalResource, midpoint, Quaternion.identity);
                        portalHolo.transform.localScale = Vector3.zero;
                        color = portalHolo.GetComponent<Renderer>().material.color;
                        //color = Color.cyan;
                        color.a = 0.25f;
                        portalHolo.GetComponent<Renderer>().material.color = color;
                    }
                }
            }
            else
            {
                //scale and orient portal
                if (leftGripHeld & rightGripHeld)
                {
                    //link portal scale to distance btwn controllers
                    portal.transform.localScale += deltaDistance * Vector3.one;
                    if (portal.transform.lossyScale.magnitude < 0f)
                        portal.transform.localScale = Vector3.zero;

                    //portal step sizes
                    if (0f < portal.transform.localScale.x & portal.transform.localScale.x < 0.1f)
                        portalHolo.transform.localScale = minScale * Vector3.one;
                    else if (0.1f < portal.transform.localScale.x & portal.transform.localScale.x < 0.2f)
                        portalHolo.transform.localScale = midScale * Vector3.one;
                    else if (0.2f < portal.transform.localScale.x & portal.transform.localScale.x < 0.3f)
                        portalHolo.transform.localScale = maxScale * Vector3.one;

                    //position and orientation are based on the controllers
                    midpoint = (leftControllerPosition + rightControllerPosition + 2 * transform.GetChild(3).transform.position) / 2; //CLOSE BUT NOT PERFECT
                    portal.transform.position = midpoint;
                    portalHolo.transform.position = midpoint;

                    //TODO: FIX ROTATION
                    //portal.transform.rotation = Quaternion.Euler(leftControllerRotation + new Vector3(90f, 0f, 0f));
                    //portal.transform.Rotate(deltaRotation);
                    //portal.transform.rotation = Quaternion.LookRotation(currentRotation);

                    //lock in portal
                    if (leftTriggerHeld & rightTriggerHeld)
                    {
                        portal.transform.localScale = portalHolo.transform.localScale;
                        portal.GetComponent<Portal>().SetRate(minScale, midScale, maxScale);
                        Destroy(portalHolo);
                        lockPortal = true;
                    }
                }
                else 
                {
                    //destroy if all buttons released without placing portal
                    Destroy(portal);
                    Destroy(portalHolo);
                }
            }
        }
    }

    void GetControllerValues()
    {
        //device
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        //tracked
        leftController.TryGetFeatureValue(CommonUsages.isTracked, out leftTracked);
        rightController.TryGetFeatureValue(CommonUsages.isTracked, out rightTracked);

        //position
        leftController.TryGetFeatureValue(CommonUsages.devicePosition, out leftControllerPosition);
        rightController.TryGetFeatureValue(CommonUsages.devicePosition, out rightControllerPosition);

        //distance
        currentDistance = Vector3.Distance(leftControllerPosition, rightControllerPosition);
        deltaDistance = currentDistance - previousDistance;
        previousDistance = currentDistance;

        //rotation
        leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out temp);
        leftControllerRotation = temp.eulerAngles;
        rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out temp);
        rightControllerRotation = temp.eulerAngles;

        //currentRotation = (leftControllerRotation + rightControllerRotation) / 2;
        //deltaRotation = currentRotation - previousRotation;
        //previousRotation = currentRotation;

        //trigger
        leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftTriggerHeld);
        rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTriggerHeld);

        //grip
        leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGripHeld);
        rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripHeld);
    }
}
