// ControlInput.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlInput : MonoBehaviour
{
    private FlightControls flightControls;

    public float RollInput { get; private set; }
    public float PitchInput { get; private set; }
    public float YawInput { get; private set; }

    // Yumuþatýlmýþ input deðerleri
    public float SmoothedRollInput { get; private set; }
    public float SmoothedPitchInput { get; private set; }
    public float SmoothedYawInput { get; private set; }

    [Header("Input Smoothing")]
    [SerializeField] private float inputSmoothingSpeed = 10f; // Yumuþatma hýzý

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

        flightControls.Flight.Disable();
    }

    private void Update()
    {
        // Hedef input deðerlerine doðru yumuþakça ilerle
        SmoothedRollInput = Mathf.Lerp(SmoothedRollInput, RollInput, Time.deltaTime * inputSmoothingSpeed);
        SmoothedPitchInput = Mathf.Lerp(SmoothedPitchInput, PitchInput, Time.deltaTime * inputSmoothingSpeed);
        SmoothedYawInput = Mathf.Lerp(SmoothedYawInput, YawInput, Time.deltaTime * inputSmoothingSpeed);
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
}