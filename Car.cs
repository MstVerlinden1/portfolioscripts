using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Wheel[] Wheels;

    [SerializeField] private float power;
    [SerializeField] private float maxAngle;
    [SerializeField] public float maxSpeed;
    [SerializeField] private float wheelStiffness;

    [SerializeField] private GameObject[] chargeObjs;
    [SerializeField] private GameObject speedEffect;
    [SerializeField] private float speedBoost;
    [SerializeField] private float camShake;
    [SerializeField] private float shakeDuration;
    public int maxBattery = 5;
    public int batteryCharge;

    private Rigidbody carRb;

    private float m_Forward;
    private float m_Angle;
    private float m_Brake;

    [Range(0f, 100f)]
    [SerializeField] private float ForwardsFriction;
    [Range(0f, 100f)]
    [SerializeField] private float SidewaysFriction;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        //get battery charges and sort them
        chargeObjs = GameObject.FindGameObjectsWithTag("Charges");
        Array.Sort(chargeObjs, CompareObNames);
        int CompareObNames(GameObject y, GameObject x)
        {
            return x.name.CompareTo(y.name);
        }
        //set batterycharges inactive
        for (int i = 0; i < chargeObjs.Length; i++)
        {
            chargeObjs[i].SetActive(false);
        }
        for (int i = 0; i < batteryCharge; i++)
        {
            chargeObjs[i].SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            applySpeedBoost();
        }
        Inputs();
        MaxVelocity();
    }
    private void FixedUpdate()
    {
        //on each wheel apply the forces to drive
        foreach (Wheel _wheel in Wheels)
        {
            _wheel.Accelerate(m_Forward * power);
            _wheel.Turn(m_Angle * maxAngle);
            _wheel.Brake(m_Brake * power * 2);
            _wheel.applyWheelModifiers(ForwardsFriction, SidewaysFriction, wheelStiffness);
        }
    }
    private void MaxVelocity()
    {
        //get the max velocity of the car compare it to the maximum speed and turn of forward movement
        if (carRb.velocity.sqrMagnitude > maxSpeed)
        {
            m_Forward = 0;
        }
    }
    private void Inputs()
    {
        //get input for movement
        m_Forward = Input.GetAxis("Vertical");
        m_Angle = Input.GetAxis("Horizontal");
        m_Brake = Input.GetAxis("Jump");
    }
    private void applySpeedBoost()
    {
        if (batteryCharge > 0)
        {
            //if the battery is more than zero apply forwards force
            carRb.AddRelativeForce(Vector3.forward * speedBoost, ForceMode.Acceleration);
            batteryCharge--;
            chargeObjs[batteryCharge].SetActive(false);
            Instantiate(speedEffect, transform);
            CamShake.Instance.ShakeCam(camShake, shakeDuration);
        }
    }
    public void GiveCharge(int addCharge)
    {
        //adds charge to the battery
        batteryCharge += addCharge;
        for (int i = 0; i < batteryCharge; i++)
        {
            chargeObjs[i].SetActive(true);
        }
    }
}