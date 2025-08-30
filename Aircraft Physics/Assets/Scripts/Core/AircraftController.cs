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
    [SerializeField] private float liftCoefficient = 10f; // Kaldýrma katsayýsý
    [SerializeField] private float dragCoefficient = 0.5f; // Sürüklenme katsayýsý

    private Rigidbody rb;
    private ControlInput controlInput;

    [Header("Stability")]
    [SerializeField] private float stabilityDamping = 10f; // Dönüþ hýzýný yavaþlatma
    [SerializeField] private float levelingForce = 50f; // Uçaðý yere paralel hale getirme

    // Yumuþatýlmýþ girdi deðerleri
    private float smoothedRollInput;
    private float smoothedPitchInput;
    private float smoothedYawInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controlInput = GetComponent<ControlInput>();

        // Baþlangýçta yerçekimini kapat
        // Çünkü kaldýrma kuvvetini kendimiz yöneteceðiz
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        // 1. Yumuþatýlmýþ girdileri hesapla
        // Tuþ basýlýysa, kontrol hýzýna göre hedefe (input deðeri) doðru ilerle.
        if (controlInput.RollInput != 0)
            smoothedRollInput = Mathf.MoveTowards(smoothedRollInput, controlInput.RollInput, Time.fixedDeltaTime * controlSpeed);
        else
            // Tuþ býrakýldýysa, kontrol hýzýnýn yarýsýyla 0'a geri dön.
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
        // Ýleri itme kuvveti (hýzlanma)
        rb.AddRelativeForce(Vector3.forward * thrust * 1000f, ForceMode.Force);
    }

    private void ApplyAerodynamicForces()
    {
        float speed = rb.linearVelocity.magnitude;

        // Sürüklenme kuvveti (Drag): Hýzýn karesiyle orantýlý daha gerçekçi bir drag ekle
        rb.AddForce(-rb.linearVelocity.normalized * (speed * speed * dragCoefficient));

        // Kaldýrma kuvveti (Lift): Açý ve hýza baðlý olarak deðiþsin
        float lift = speed * speed * liftCoefficient;
        rb.AddForce(transform.up * lift);
    }

    private void ApplyTorques()
    {
        // Roll, Pitch ve Yaw için torklarý uygula
        rb.AddRelativeTorque(Vector3.forward * -smoothedRollInput * rollTorque);
        rb.AddRelativeTorque(Vector3.right * smoothedPitchInput * pitchTorque);
        rb.AddRelativeTorque(Vector3.up * smoothedYawInput * yawTorque);
    }

    private void ApplyStabilizingForces()
    {
        // Eðer kullanýcý herhangi bir kontrol girdisi saðlamýyorsa dengelemeyi uygula
        if (controlInput.RollInput == 0 && controlInput.PitchInput == 0 && controlInput.YawInput == 0)
        {
            // Angular Damping (Açýsal Yavaþlatma)
            rb.AddTorque(-rb.angularVelocity * stabilityDamping);

            // Leveling Force (Düzleþtirme Kuvveti)
            rb.AddTorque(Vector3.Cross(transform.up, Vector3.up) * levelingForce);
            rb.AddTorque(Vector3.Cross(transform.forward, Vector3.forward) * (levelingForce / 2f));
        }
    }
}