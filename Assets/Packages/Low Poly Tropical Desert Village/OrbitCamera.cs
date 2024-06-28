using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public Vector3 rotateAxis;
    public AnimationCurve curve;

    public Camera camera;

    void Update()
    {
        this.transform.Rotate(rotateAxis, rotationSpeed * Time.deltaTime);

        camera.transform.LookAt(transform);
        //float y = curve.Evaluate(Time.time - 2);
        //camera.transform.position
        //Debug.Log(curve.Evaluate(Time.time));
    }
}
