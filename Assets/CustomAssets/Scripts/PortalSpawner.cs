using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PortalSpawner : MonoBehaviour
{
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

    GameObject portalResource;
    GameObject portal;

    void Start()
    {
        portalResource = Resources.Load("Portal") as GameObject;
    }

    void Update()
    {
        GetControllerValues();

        if (leftTracked & rightTracked)
        {
            //instantiate portal if not already
            if (portal == null)
            {
                if (Vector3.Distance(leftControllerPosition, rightControllerPosition) < 0.18f)
                {
                    if (leftTriggerHeld & rightTriggerHeld & leftGripHeld & rightGripHeld)
                    {
                        midpoint = (leftControllerPosition + rightControllerPosition + 2 * transform.GetChild(3).transform.position) / 2; //CLOSE BUT NOT PERFECT
                        portal = Instantiate(portalResource, midpoint, Quaternion.identity);
                        portal.transform.localScale = Vector3.zero;
                    }
                }
            }
            else
            {
                //scale and orient portal
                if (leftTriggerHeld & rightTriggerHeld & leftGripHeld & rightGripHeld)
                {
                    //link portal scale to distance btwn controllers
                    portal.transform.localScale += deltaDistance * Vector3.one;
                    if (portal.transform.lossyScale.magnitude < 0f)
                        portal.transform.localScale = Vector3.zero;

                    //TODO: limit portal size (and create steps?)

                    //position and orientation are based on the controllers
                    midpoint = (leftControllerPosition + rightControllerPosition + 2 * transform.GetChild(3).transform.position) / 2; //CLOSE BUT NOT PERFECT
                    portal.transform.position = midpoint;

                    //portal.transform.rotation = Quaternion.Euler(leftControllerRotation + new Vector3(90f, 0f, 0f));
                    //portal.transform.Rotate(deltaRotation);
                    portal.transform.rotation = Quaternion.LookRotation(currentRotation + new Vector3(90f, 0f, 0f));
                }
                else 
                {
                    //if portal scale is zero then destroy it when one of the buttons is released
                    if (portal.transform.localScale.magnitude < 0.1)
                        Destroy(portal);
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

        currentRotation = (leftControllerRotation + rightControllerRotation) / 2;
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
