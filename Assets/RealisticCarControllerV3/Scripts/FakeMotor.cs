using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeMotor : MonoBehaviour
{
    [SerializeField] private WheelCollider[] m_WheelColliders;
    [SerializeField] private float m_Torque = 10f;
    public Transform[] wheelModels;

    private Rigidbody m_Rigidbody;
    private bool m_IsReversing;

    private void Start() => m_Rigidbody = GetComponent<Rigidbody>();

    private void Update()
    {
        // Convert from world space to local space. Anything less than zero in the forward direction is backwards.
        // Stationary will count as forwards.
        m_IsReversing = transform.InverseTransformDirection(m_Rigidbody.velocity).z < 0f;

        if (RCC_SceneManager.Instance.activePlayerVehicle != null)
        {
            // Make it move!
            float accel = RCC_SceneManager.Instance.activePlayerVehicle.throttleInput;
            float brake = RCC_SceneManager.Instance.activePlayerVehicle.brakeInput;
            Throttle(accel, brake);
        }
    }

    private void Throttle(float accel, float brake)
    {
        for (int i = 0; i < m_WheelColliders.Length; i++)
        {
            // Accelerator is applied
            if (accel > 0.0f)
            {
                if (m_IsReversing)
                {
                    // When reversing, accelerator acts as a brake.
                    m_WheelColliders[i].motorTorque = 0f;
                }
                else
                {
                    // Apply a little bit of motor torque to help us get rolling
                    m_WheelColliders[i].motorTorque = m_Torque;
                    m_WheelColliders[i].brakeTorque = 0f;
                }
            }
            else if (brake > 0.0f)
            {
                if (m_IsReversing)
                {
                    // The car is trying to reverse, give it some help.
                    m_WheelColliders[i].motorTorque = -m_Torque;
                    m_WheelColliders[i].brakeTorque = 0f;
                }
                else
                {
                    // The car is braking!
                    m_WheelColliders[i].motorTorque = 0f;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < m_WheelColliders.Length; i++)
        {
            ApplyLocalPositionToVisuals(m_WheelColliders[i], wheelModels[i]);
        }

    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform visualWheel)
    {

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}
