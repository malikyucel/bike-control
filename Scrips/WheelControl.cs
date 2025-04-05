using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Rigidbody bikeRb;

    [Header("Wheel Mash and Collider")]
    [SerializeField] private WheelCollider frontWheelCollidere;
    [SerializeField] private WheelCollider backWheelCollidere;
    [SerializeField] private Transform frontWheelMesh;
    [SerializeField] private Transform backWheelMesh;

    [Header("Settings")]
    [SerializeField] private float torque;
    [SerializeField] private float brakeForce;
    [SerializeField] private float angle;
    [SerializeField] private float currentAngle;
    [SerializeField] private float speedteercontrolTime;
    [SerializeField] private float bodyZPosSmooth;
    [SerializeField] private List<float> turnigAngle;
    [SerializeField] private List<float> turningMagnitude;
    
    [Header("Input")]
    [SerializeField] private float forwardTorque,wheelAngle;

    private void Start() 
    {
        bikeRb.centerOfMass = new Vector3(0, -0.5f, -0.1f);
    }
    private void FixedUpdate() 
    {
        forwardTorque = Input.GetAxis("Vertical");
        wheelAngle = Input.GetAxis("Horizontal");

        FrontWheelColliderAngleContro();
        BikeBalance();
        WheelColldierForceAndAngle();
        Brake();

        UpdateWheelMesh(frontWheelCollidere, frontWheelMesh);
        UpdateWheelMesh(backWheelCollidere, backWheelMesh);
    }

    private void Brake()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            backWheelCollidere.brakeTorque = brakeForce;
            frontWheelCollidere.brakeTorque = brakeForce/3;
        }
        else
        {
            backWheelCollidere.brakeTorque = 0f;
            frontWheelCollidere.brakeTorque = 0f;
        }
    }

    private void WheelColldierForceAndAngle()
    {
        currentAngle = Mathf.Lerp(currentAngle, angle * wheelAngle, speedteercontrolTime);
        frontWheelCollidere.steerAngle = currentAngle;

        backWheelCollidere.motorTorque = forwardTorque * torque;
    }

    private void BikeBalance()
    {
        Vector3 currentRot = transform.rotation.eulerAngles;
        if (bikeRb.velocity.magnitude < 1 || Mathf.Abs(wheelAngle) < 0.1f)
        {
            float smoothZ = Mathf.LerpAngle(currentRot.z, 0f, bodyZPosSmooth);
            transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, smoothZ);
        }
    }

    private void FrontWheelColliderAngleContro()
    {
        if (bikeRb.velocity.magnitude < turningMagnitude[0] )
        {
            angle = Mathf.LerpAngle(angle, turnigAngle[0], speedteercontrolTime);
        }
        else if (bikeRb.velocity.magnitude > turningMagnitude[0] && bikeRb.velocity.magnitude < turningMagnitude[1] )
        {			
            angle = Mathf.LerpAngle(angle, turnigAngle[1], speedteercontrolTime);
        }
        else if (bikeRb.velocity.magnitude > turningMagnitude[1] && bikeRb.velocity.magnitude < turningMagnitude[2] )
        {			
            angle = Mathf.LerpAngle(angle, turnigAngle[2], speedteercontrolTime);
        }
        else if (bikeRb.velocity.magnitude > turningMagnitude[2] && bikeRb.velocity.magnitude < turningMagnitude[3] )
        {			
            angle = Mathf.LerpAngle(angle,  turnigAngle[3], speedteercontrolTime);
        }
    }

    private void UpdateWheelMesh(WheelCollider wheelCollider, Transform wheelTrasform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);

        wheelTrasform.position = pos;
        wheelTrasform.rotation = rot;
    }
}
