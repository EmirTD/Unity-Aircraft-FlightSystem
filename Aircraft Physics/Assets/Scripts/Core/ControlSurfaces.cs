using UnityEngine;

public class ControlSurfaces : MonoBehaviour
{
    [Header("Control Surface References")]
    [SerializeField] private Transform leftAileron;
    [SerializeField] private Transform rightAileron;
    [SerializeField] private Transform elevator;
    [SerializeField] private Transform rudder;

    [Header("Rotation Settings")]
    [SerializeField] private float aileronRotationLimit = 20f;
    [SerializeField] private float elevatorRotationLimit = 20f;
    [SerializeField] private float rudderRotationLimit = 20f;

    private ControlInput controlInput;


    [Header("Rotation Settings")]
    [SerializeField] private float smoothness = 10f;

    private void Awake()
    {
        controlInput = GetComponent<ControlInput>();
    }

    private void Update()
    {
        float aileronRotation = controlInput.RollInput * aileronRotationLimit;
        float elevatorRotation = NonZero(controlInput.PitchInput * elevatorRotationLimit, controlInput.BoostInput * elevatorRotationLimit);
        float rudderRotation = NonZero(controlInput.YawInput * rudderRotationLimit, controlInput.SteerInput * rudderRotationLimit);

        if (leftAileron != null)
        {
            Quaternion targetAileronRot = Quaternion.Euler(-aileronRotation, 0, 0);
            leftAileron.localRotation = Quaternion.Slerp(leftAileron.localRotation, targetAileronRot, Time.deltaTime * smoothness);
        }
        if (rightAileron != null)
        {
            Quaternion targetAileronRot = Quaternion.Euler(aileronRotation, 0, 0);
            rightAileron.localRotation = Quaternion.Slerp(rightAileron.localRotation, targetAileronRot, Time.deltaTime * smoothness);
        }
        if (elevator != null)
        {
            Quaternion targetElevatorRot = Quaternion.Euler(elevatorRotation, 0, 0);
            elevator.localRotation = Quaternion.Slerp(elevator.localRotation, targetElevatorRot, Time.deltaTime * smoothness);
        }
        if (rudder != null)
        {
            Quaternion targetRudderRot = Quaternion.Euler(0, -rudderRotation, 0);
            rudder.localRotation = Quaternion.Slerp(rudder.localRotation, targetRudderRot, Time.deltaTime * smoothness);
        }
    }

    private float NonZero(float a, float b)
    {
        if (a != 0f) return a;
        if (b != 0f) return b;
        return 0f;
    }
}