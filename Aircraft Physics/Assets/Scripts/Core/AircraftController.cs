// AircraftController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AircraftController : MonoBehaviour
{
    [Header("Flight Dynamics")]
    [SerializeField] private float thrust = 50f;
    [SerializeField] private float rollTorque = 100f;
    [SerializeField] private float pitchTorque = 100f;
    [SerializeField] private float yawTorque = 100f;

    [Header("Control Sensitivity")]
    [SerializeField] private float controlSpeed = 5f;

    [Header("Aerodynamic Forces")]
    [SerializeField] private float liftCoefficient = 10f; // Kald�rma katsay�s�
    [SerializeField] private float dragCoefficient = 0.5f; // S�r�klenme katsay�s�

    private Rigidbody rb;
    private ControlInput controlInput;

    [Header("Stability")]
    [SerializeField] private float stabilityDamping = 10f; // D�n�� h�z�n� yava�latma
    [SerializeField] private float levelingForce = 50f; // U�a�� yere paralel hale getirme

    // Yumu�at�lm�� girdi de�erleri
    private float smoothedRollInput;
    private float smoothedPitchInput;
    private float smoothedYawInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controlInput = GetComponent<ControlInput>();

        // Ba�lang��ta yer�ekimini kapat
        // ��nk� kald�rma kuvvetini kendimiz y�netece�iz
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        // 1. Yumu�at�lm�� girdileri hesapla
        // Tu� bas�l�ysa, kontrol h�z�na g�re hedefe (input de�eri) do�ru ilerle.
        if (controlInput.RollInput != 0)
            smoothedRollInput = Mathf.MoveTowards(smoothedRollInput, controlInput.RollInput, Time.fixedDeltaTime * controlSpeed);
        else
            // Tu� b�rak�ld�ysa, kontrol h�z�n�n yar�s�yla 0'a geri d�n.
            smoothedRollInput = Mathf.MoveTowards(smoothedRollInput, 0, Time.fixedDeltaTime * controlSpeed / 2f);

        if (controlInput.PitchInput != 0)
            smoothedPitchInput = Mathf.MoveTowards(smoothedPitchInput, controlInput.PitchInput, Time.fixedDeltaTime * controlSpeed);
        else
            smoothedPitchInput = Mathf.MoveTowards(smoothedPitchInput, 0, Time.fixedDeltaTime * controlSpeed / 2f);

        if (controlInput.YawInput != 0)
            smoothedYawInput = Mathf.MoveTowards(smoothedYawInput, controlInput.YawInput, Time.fixedDeltaTime * controlSpeed);
        else
            smoothedYawInput = Mathf.MoveTowards(smoothedYawInput, 0, Time.fixedDeltaTime * controlSpeed / 2f);

        // 2. Fiziksel kuvvetleri uygula
        ApplyThrust();
        ApplyAerodynamicForces();
        ApplyTorques();

        ApplyStabilizingForces();
    }

    private void ApplyThrust()
    {
        // �leri itme kuvveti (h�zlanma)
        rb.AddRelativeForce(Vector3.forward * thrust * 1000f, ForceMode.Force);
    }

    private void ApplyAerodynamicForces()
    {
        float speed = rb.linearVelocity.magnitude;

        // S�r�klenme kuvveti (Drag): H�z�n karesiyle orant�l� daha ger�ek�i bir drag ekle
        rb.AddForce(-rb.linearVelocity.normalized * (speed * speed * dragCoefficient));

        // Kald�rma kuvveti (Lift): A�� ve h�za ba�l� olarak de�i�sin
        float lift = speed * speed * liftCoefficient;
        rb.AddForce(transform.up * lift);
    }

    private void ApplyTorques()
    {
        // Roll, Pitch ve Yaw i�in torklar� uygula
        rb.AddRelativeTorque(Vector3.forward * -smoothedRollInput * rollTorque);
        rb.AddRelativeTorque(Vector3.right * smoothedPitchInput * pitchTorque);
        rb.AddRelativeTorque(Vector3.up * smoothedYawInput * yawTorque);
    }

    private void ApplyStabilizingForces()
    {
        // E�er kullan�c� herhangi bir kontrol girdisi sa�lam�yorsa dengelemeyi uygula
        if (controlInput.RollInput == 0 && controlInput.PitchInput == 0 && controlInput.YawInput == 0)
        {
            // Angular Damping (A��sal Yava�latma)
            rb.AddTorque(-rb.angularVelocity * stabilityDamping);

            // Leveling Force (D�zle�tirme Kuvveti)
            rb.AddTorque(Vector3.Cross(transform.up, Vector3.up) * levelingForce);
            rb.AddTorque(Vector3.Cross(transform.forward, Vector3.forward) * (levelingForce / 2f));
        }
    }
}