using Unity.VisualScripting;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    [Header("General Movement")]
    [SerializeField] private float flightSpeed = 50f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float levelingSpeed = 5f;

    [Header("Takeoff Settings")]
    [SerializeField] private float takeoffSpeedThreshold = 60f;
    [SerializeField] private float takeoffHeightThreshold = 10f;

    [Header("Landing Settings")]
    [SerializeField] private float descentRate = 5f;
    [SerializeField] private float landingSpeed = 15f;
    [SerializeField] private float runwayHeight = 0.5f;

    [Header("Ground Movement")]
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float brakeDeceleration = 30f;
    [SerializeField] private float steeringSpeed = 50f;

    [Header("References")]
    [SerializeField] private Transform frontWheel;
    [SerializeField] private Transform[] rearWheels;

    private ControlInput controlInput;
    private Rigidbody rb;
    private float currentSpeed;

    private void Awake()
    {
        controlInput = GetComponent<ControlInput>();
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

    }

    private void Start()
    {
        controlInput.SetControlMode(ControlInput.ControlMode.Ground);
    }

    private void Update()
    {
        switch (controlInput.currentMode)
        {
            case ControlInput.ControlMode.Ground:
                HandleGroundMovement();
                UpdateWheelVisuals();
                CheckForTakeoff();
                break;
            case ControlInput.ControlMode.Flight:
                HandleFlightMovement();
                break;
            case ControlInput.ControlMode.Landing:
                HandleLandingMovement();
                break;
        }
    }

    private void HandleGroundMovement()
    {
        float forwardInput = controlInput.SmoothedThrottleGroundInput;
        float boostInput = controlInput.BoostInput; // yeni eklenen
        float steeringInput = controlInput.SmoothedSteerInput;
        float brakeInput = controlInput.BrakeInput;

        AlignToGround();

        // Ýleri hýzlanma
        currentSpeed += forwardInput * acceleration * Time.deltaTime;

        // Boost varsa hýz artýþý
        if (boostInput > 0)
            currentSpeed += acceleration * Time.deltaTime;

        // Fren
        if (brakeInput > 0)
            currentSpeed = Mathf.Max(0, currentSpeed - brakeDeceleration * Time.deltaTime);

        if (currentSpeed < 0.1f)
            currentSpeed = 0;

        // Maks hýz kontrolü
        float maxSpeed = boostInput > 0 ? takeoffSpeedThreshold : 50f;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        // Yönlendirme
        if (Mathf.Abs(currentSpeed) > 0.1f && Mathf.Abs(steeringInput) > 0.1f)
            transform.Rotate(0, steeringInput * steeringSpeed * Time.deltaTime, 0);

        // Ýleri hareket
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Kalkýþ kontrolü
        if (currentSpeed >= takeoffSpeedThreshold && controlInput.SmoothedThrottleGroundInput > 0.1f)
        {
            controlInput.SetControlMode(ControlInput.ControlMode.Flight);
            Debug.Log("Takeoff! -> Flight mode");
        }
    }

    private void HandleFlightMovement()
    {
        if(controlInput.LandInput > 0)
        {
            Debug.Log("landing");
            controlInput.SetControlMode(ControlInput.ControlMode.Landing);
        }

        ApplyForwardMovement(flightSpeed);
        ApplyRotation(controlInput.SmoothedRollInput, controlInput.SmoothedPitchInput, controlInput.SmoothedYawInput);

        // Auto leveling
        if (controlInput.RollInput == 0 && controlInput.PitchInput == 0 && controlInput.YawInput == 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * levelingSpeed);
        }
    }

    private void HandleLandingMovement()
    {
        if (controlInput.LandInput > 0)
        {
            Debug.Log("Flight");
            controlInput.SetControlMode(ControlInput.ControlMode.Flight);
        }
        // Forward deceleration
        currentSpeed = Mathf.Lerp(currentSpeed, landingSpeed, Time.deltaTime * 0.5f);
        ApplyForwardMovement(currentSpeed);
        ApplyRotation(controlInput.SmoothedRollInput, controlInput.SmoothedPitchInput, controlInput.SmoothedYawInput);

        // Vertical descent
        float descentSpeed = descentRate;
        float distanceToRunway = transform.position.y - runwayHeight;
        if (distanceToRunway < 20f && distanceToRunway > 0f)
            descentSpeed = Mathf.Lerp(descentRate, 0, (20f - distanceToRunway) / 20f);

        transform.Translate(Vector3.down * descentSpeed * Time.deltaTime, Space.World);
    }

    private void ApplyForwardMovement(float speed)
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void ApplyRotation(float rollInput, float pitchInput, float yawInput)
    {
        float roll = -rollInput * rotationSpeed * Time.deltaTime;
        float pitch = -pitchInput * rotationSpeed * Time.deltaTime;
        float yaw = yawInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    private void UpdateWheelVisuals()
    {
        float wheelRotationSpeed = currentSpeed * 10f;

        foreach (Transform wheel in rearWheels)
            wheel.Rotate(-Vector3.right, wheelRotationSpeed * Time.deltaTime);

        if (frontWheel != null)
        {
            frontWheel.Rotate(-Vector3.right, wheelRotationSpeed * Time.deltaTime);
            float steeringAngle = -controlInput.SteerInput * 30f;
            Quaternion targetRotation = Quaternion.Euler(0, steeringAngle, 0);
            frontWheel.localRotation = Quaternion.Slerp(frontWheel.localRotation, targetRotation, Time.deltaTime * steeringSpeed);
        }
    }

    private void CheckForTakeoff()
    {
        if (transform.position.y > takeoffHeightThreshold &&
            controlInput.currentMode == ControlInput.ControlMode.Ground)
        {
            controlInput.SetControlMode(ControlInput.ControlMode.Flight);
            Debug.Log("Takeoff!");
        }
    }

    private void AlignToGround()
    {
        
        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f * Time.deltaTime);

        
        Vector3 pos = transform.position;
        pos.y = runwayHeight;
        transform.position = Vector3.Lerp(transform.position, pos, 2f * Time.deltaTime);;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controlInput.currentMode == ControlInput.ControlMode.Landing &&
            other.CompareTag("Ground"))
        {

            Debug.Log("Landing successful -> switching to Ground");

            AlignToGround();

            controlInput.SetControlMode(ControlInput.ControlMode.Ground);
        }
    }
}
