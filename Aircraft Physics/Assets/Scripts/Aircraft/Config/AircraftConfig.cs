using UnityEngine;


[CreateAssetMenu(menuName = "Flight/AircraftConfig")]
public class AircraftConfig : ScriptableObject
{
    [Header("Geometry")]
    public float Sref = 30f;     // m^2
    public float span = 10f;     // m (kanat açýklýðý)
    public float chord = 3f;     // m (mean aerodynamic chord)

    [Header("Lift")]
    public bool useCLvsAlphaCurve = false;
    public bool curveInDegrees = true;
    public AnimationCurve CLvsAlpha;
    public bool enableCLClamp = true;
    public float CLmin = -1.2f;
    public float CLmax = 1.8f;

    public float CL0 = 0.2f;
    public float CLa = 5.5f;   // per rad
    public float CLq = -8.0f;  // rate damping
    public float CLde = 0.7f;  // elevator etkisi

    [Header("Drag")]
    public float CD0 = 0.025f;
    public float k = 0.06f;    // induced drag faktörü
    public float CDa2 = 0.0f;    // alpha^2 ek terim
    public float CDq = 0.02f;   // rate damping

    [Header("Mach drag rise")]
    public bool enableMachDragRise = true;
    public float machDragRiseStart = 0.72f;
    public float machDragRiseK = 0.4f;

    [Header("Sideforce")]
    public float CYb = -0.9f;
    public float CYp = -0.1f;
    public float CYr = 0.25f;
    public float CYda = 0.0f;
    public float CYdr = 0.2f;

    [Header("Moments")]
    public bool useCMvsAlphaCurve = false;
    public AnimationCurve CMvsAlpha;

    public float Cm0 = 0.02f;
    public float Cma = -1.0f;
    public float Cmq = -20f;
    public float Cmde = -1.1f;

    public float Clb = -0.12f;
    public float Clp = -0.5f;
    public float Clr = 0.25f;
    public float Clda = 0.08f;
    public float Cldr = 0.02f;

    public float Cnb = 0.25f;
    public float Cnp = -0.06f;
    public float Cnr = -0.3f;
    public float Cnda = 0.02f;
    public float Cndr = -0.1f;
}