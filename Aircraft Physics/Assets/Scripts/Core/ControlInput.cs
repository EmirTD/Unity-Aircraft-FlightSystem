// ControlInput.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlInput : MonoBehaviour
{
    private FlightControls flightControls;
    public enum ControlMode { Flight, Ground, Landing, Crash }
    public ControlMode currentMode { get; private set; }

    public float RollInput { get; private set; }
    public float PitchInput { get; private set; }
    public float YawInput { get; private set; }
    public float LandInput { get; private set; }

    public float LandingRollInput { get; private set; }
    public float LandingPitchInput { get; private set; }
    public float LandingYawInput { get; private set; }

    public float SmoothedRollInput { get; private set; }
    public float SmoothedPitchInput { get; private set; }
    public float SmoothedYawInput { get; private set; }

    public float ThrottleGroundInput { get; private set; }
    public float SteerInput { get; private set; }
    public float BrakeInput { get; private set; }
    public float BoostInput { get; private set; }

    public float SmoothedThrottleGroundInput { get; private set; }
    public float SmoothedSteerInput { get; private set; }



    [Header("Input Smoothing")]
    [SerializeField] private float inputSmoothingSpeed = 10f; 

    private void Awake()
    {
        flightControls = new FlightControls();
    }

    private void OnEnable()
    {
        flightControls.Flight.Roll.performed += OnRollPerformed;
        flightControls.Flight.Roll.canceled += OnRollCanceled;
        flightControls.Flight.Pitch.performed += OnPitchPerformed;
        flightControls.Flight.Pitch.canceled += OnPitchCanceled;
        flightControls.Flight.Yaw.performed += OnYawPerformed;
        flightControls.Flight.Yaw.canceled += OnYawCanceled;
        flightControls.Flight.Land.started += OnLandPerformed;
        flightControls.Flight.Land.canceled += OnLandCanceled;

        // Ground Action Map
        flightControls.Ground.ThrottleGround.performed += OnThrottleGroundPerformed;
        flightControls.Ground.ThrottleGround.canceled += OnThrottleGroundCanceled;
        flightControls.Ground.Steer.performed += OnSteerPerformed;
        flightControls.Ground.Steer.canceled += OnSteerCanceled;
        flightControls.Ground.Brake.performed += OnBrakePerformed;
        flightControls.Ground.Brake.canceled += OnBrakeCanceled;
        flightControls.Ground.BoostThrottleGround.performed += OnBoostPerformed;
        flightControls.Ground.BoostThrottleGround.canceled += OnBoostCanceled;


        // Landing Action Map
        flightControls.Landing.Roll.performed += OnLandingRollPerformed;
        flightControls.Landing.Roll.canceled += OnLandingRollCanceled;
        flightControls.Landing.Pitch.performed += OnLandingPitchPerformed;
        flightControls.Landing.Pitch.canceled += OnLandingPitchCanceled;
        flightControls.Landing.Yaw.performed += OnLandingYawPerformed;
        flightControls.Landing.Yaw.canceled += OnLandingYawCanceled;
        flightControls.Landing.Land.started += OnLandPerformed;
        flightControls.Landing.Land.canceled += OnLandCanceled;


        SetControlMode(ControlMode.Ground);

        flightControls.Flight.Enable();
    }

    private void OnDisable()
    {
        flightControls.Flight.Roll.performed -= OnRollPerformed;
        flightControls.Flight.Roll.canceled -= OnRollCanceled;
        flightControls.Flight.Pitch.performed -= OnPitchPerformed;
        flightControls.Flight.Pitch.canceled -= OnPitchCanceled;
        flightControls.Flight.Yaw.performed -= OnYawPerformed;
        flightControls.Flight.Yaw.canceled -= OnYawCanceled;


        flightControls.Ground.ThrottleGround.performed -= OnThrottleGroundPerformed;
        flightControls.Ground.ThrottleGround.canceled -= OnThrottleGroundCanceled;
        flightControls.Ground.Steer.performed -= OnSteerPerformed;
        flightControls.Ground.Steer.canceled -= OnSteerCanceled;
        flightControls.Ground.Brake.performed -= OnBrakePerformed;
        flightControls.Ground.Brake.canceled -= OnBrakeCanceled;

        flightControls.Flight.Disable();
    }

    private void Update()
    {

        SmoothedRollInput = Mathf.Lerp(SmoothedRollInput, RollInput, Time.deltaTime * inputSmoothingSpeed);
        SmoothedPitchInput = Mathf.Lerp(SmoothedPitchInput, PitchInput, Time.deltaTime * inputSmoothingSpeed);
        SmoothedYawInput = Mathf.Lerp(SmoothedYawInput, YawInput, Time.deltaTime * inputSmoothingSpeed);

        SmoothedThrottleGroundInput = Mathf.Lerp(SmoothedThrottleGroundInput, ThrottleGroundInput, Time.deltaTime * inputSmoothingSpeed);
        SmoothedSteerInput = Mathf.Lerp(SmoothedSteerInput, SteerInput, Time.deltaTime * inputSmoothingSpeed);
    }

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        RollInput = context.ReadValue<float>();
    }

    private void OnRollCanceled(InputAction.CallbackContext context)
    {
        RollInput = 0f;
    }

    private void OnPitchPerformed(InputAction.CallbackContext context)
    {
        PitchInput = context.ReadValue<float>();
    }

    private void OnPitchCanceled(InputAction.CallbackContext context)
    {
        PitchInput = 0f;
    }

    private void OnYawPerformed(InputAction.CallbackContext context)
    {
        YawInput = context.ReadValue<float>();
    }

    private void OnYawCanceled(InputAction.CallbackContext context)
    {
        YawInput = 0f;
    }

    // Yeni giriþ metotlarý
    private void OnThrottleGroundPerformed(InputAction.CallbackContext context)
    {
        ThrottleGroundInput = context.ReadValue<float>();
    }

    private void OnThrottleGroundCanceled(InputAction.CallbackContext context)
    {
        ThrottleGroundInput = 0f;
    }

    private void OnSteerPerformed(InputAction.CallbackContext context)
    {
        SteerInput = context.ReadValue<float>();
    }

    private void OnSteerCanceled(InputAction.CallbackContext context)
    {
        SteerInput = 0f;
    }

    private void OnBrakePerformed(InputAction.CallbackContext context)
    {
        BrakeInput = context.ReadValue<float>();
    }

    private void OnBrakeCanceled(InputAction.CallbackContext context)
    {
        BrakeInput = 0f;
    }
    private void OnBoostPerformed(InputAction.CallbackContext context)
    {
        BoostInput = context.ReadValue<float>();
    }
    private void OnBoostCanceled(InputAction.CallbackContext context)
    {
        BoostInput = 0f;
    }
    
    private void OnLandPerformed(InputAction.CallbackContext context)
    {
        LandInput = context.ReadValue<float>();
    }
    private void OnLandCanceled(InputAction.CallbackContext context)
    {
        LandInput = 0f;
    }

    private void OnLandingRollPerformed(InputAction.CallbackContext context) => LandingRollInput = context.ReadValue<float>();
    private void OnLandingRollCanceled(InputAction.CallbackContext context) => LandingRollInput = 0f;
    private void OnLandingPitchPerformed(InputAction.CallbackContext context) => LandingPitchInput = context.ReadValue<float>();
    private void OnLandingPitchCanceled(InputAction.CallbackContext context) => LandingPitchInput = 0f;
    private void OnLandingYawPerformed(InputAction.CallbackContext context) => LandingYawInput = context.ReadValue<float>();
    private void OnLandingYawCanceled(InputAction.CallbackContext context) => LandingYawInput = 0f;


    public void SetControlMode(ControlMode newMode)
    {
        currentMode = newMode;

        flightControls.Flight.Disable();
        flightControls.Ground.Disable();
        flightControls.Landing.Disable();

        switch (currentMode)
        {
            case ControlMode.Flight:
                flightControls.Flight.Enable();
                break;
            case ControlMode.Ground:
                flightControls.Ground.Enable();
                break;
            case ControlMode.Landing:    
                flightControls.Landing.Enable();
                break;
            case ControlMode.Crash:
                
                break;
        }
    }
}