using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(ControllerScript))]
public class Hand : MonoBehaviour
{
    private GameObject heldObject;
    private ControllerScript controller;
    private Rigidbody sim;

    private void Start()
    {
        sim = new GameObject().AddComponent<Rigidbody>();
        sim.name = "sim";
        sim.transform.parent = transform.parent;
        controller = GetComponent<ControllerScript>();
    }

    private void Update()
    {
        if (heldObject)
        {
            sim.velocity = (transform.position - sim.position) * 50f;
            if (controller.controller.GetPressDown(EVRButtonId.k_EButton_Grip))
            {
                if (heldObject.gameObject.tag == "PhysicsGun")
                {
                    GetComponent<PhysicsGun>().holdingGun = false;
                }

                heldObject.GetComponent<Rigidbody>().isKinematic = false;
                heldObject.transform.parent = null;
                heldObject.GetComponent<Rigidbody>().velocity = sim.velocity;
                heldObject = null;
            }
        }
        else
        {
            if (controller.controller.GetPressDown(EVRButtonId.k_EButton_Grip))
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, 0.1f);
                foreach (Collider col in cols)
                {
                    if (heldObject == null && col.GetComponent<HeldObject>() && col.transform.parent == null && col.GetComponent<HeldObject>().pickable)
                    {
                        heldObject = col.gameObject;
                        heldObject.transform.parent = transform;
                        heldObject.transform.localPosition = Vector3.zero;
                        heldObject.transform.localRotation = Quaternion.identity;
                        heldObject.GetComponent<Rigidbody>().isKinematic = true;
                        heldObject.transform.parent = transform;

                        if (col.gameObject.tag == "PhysicsGun")
                        {
                            GetComponent<PhysicsGun>().holdingGun = true;
                        }
                    }
                }
            }
        }
    }
}
