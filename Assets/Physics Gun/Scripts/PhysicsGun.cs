using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(ControllerScript))]
public class PhysicsGun : MonoBehaviour
{
    //Welcome
    public Color32 lazerColour;
    [SerializeField] private float lazerDistance, lazerSpeed, objectSpeed;
    public bool holdingGun;
    [SerializeField] private bool lazerOn, holdingObject;
    public float targetDistance;

    private GameObject lazer, targetPoint, heldObject;
    private ControllerScript controller;
    private Rigidbody sim;
    private LineRenderer lr;

    private void Start()
    {
        controller = GetComponent<ControllerScript>();
        lazer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lazer.GetComponent<BoxCollider>().isTrigger = true;
        lazer.name = transform.name + " - Lazer";
        lazer.transform.parent = transform;
        lazer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        

        lazer.GetComponent<Renderer>().material.color = lazerColour;

        lazer.AddComponent<Rigidbody>().isKinematic = true;

        targetPoint = new GameObject();
        targetPoint.name = "Target Point";
        targetPoint.transform.parent = lazer.transform;
        targetPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);

        sim = new GameObject().AddComponent<Rigidbody>();
        sim.name = "sim";

        controller = GetComponent<ControllerScript>();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    private void FixedUpdate()
    {
        if (holdingGun)
        {
            lazer.transform.localPosition = new Vector3(0, 0, lazerDistance / 2);
            targetPoint.transform.localPosition = new Vector3(0, 0, targetDistance);
            lazer.transform.localScale = new Vector3(0.001f, 0.001f, lazerDistance);
            if (!lazer.activeInHierarchy && controller.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                lazer.SetActive(true);
                lazerOn = true;
            }
            else if (lazer.activeInHierarchy && controller.controller.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                lazer.SetActive(false);
                lazerOn = false;
            }

            if (lazerOn && !holdingObject)
            {
                Collider[] cols = Physics.OverlapBox(lazer.transform.position, lazer.transform.localScale);
                foreach (Collider col in cols)
                {
                    if (heldObject == null && col.GetComponent<HeldObject>() && col.GetComponent<HeldObject>().pickable)
                    {
                        heldObject = col.gameObject;
                        heldObject.GetComponent<Rigidbody>().isKinematic = true;
                        holdingObject = true;
                    }
                }
            }
            else if (lazerOn && holdingObject)
            {
                sim.velocity = (targetPoint.transform.position - sim.position) * 25f;
                float step = objectSpeed * Time.deltaTime;
                heldObject.transform.position = Vector3.MoveTowards(heldObject.transform.position, targetPoint.transform.position, step);
            }
            else if (!lazerOn && holdingObject)
            {
                heldObject.GetComponent<Rigidbody>().isKinematic = false;
                heldObject.GetComponent<Rigidbody>().velocity = sim.velocity;
                heldObject = null;
                holdingObject = false;
            }


            if (controller.controller.GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad))
            {
                lazerDistance += controller.controller.GetAxis().y / lazerSpeed;
                targetDistance = (lazerDistance / 2) / 10;

                lazerDistance = Mathf.Clamp(lazerDistance, 2, 100);
            }

        }
        else
        {
            if (lazer.activeInHierarchy) lazer.SetActive(false);
        }
    }

    /*
    private void LateUpdate()
    {
        if (holdingGun && holdingObject && lazerOn)
        {
            if (!lr.enabled) lr.enabled = true;
            int points = 10;
            Vector3[] linePoints = new Vector3[points];

            float pointDis = (lazerDistance / 2) / (points / 2);

            for (int i = 0; i < (points / 2); i++)
            {
                Vector3 localpos = new Vector3(0, 0, (i * pointDis));
                Vector3 globalpos = transform.TransformDirection(localpos);
                print("Local Pos: " + localpos + " |  Global Pos: " + globalpos);
                lr.SetPosition(i, globalpos);
            }


            Vector3 lastpos = lazer.transform.position;
            for (int i = 0; i < (points / 2); i++)
            {
                lr.SetPosition(i + (points / 2), Vector3.MoveTowards(lastpos, heldObject.transform.position, pointDis));
                lastpos = Vector3.MoveTowards(lastpos, heldObject.transform.position, pointDis);
            }
        }
    }
    */
}
