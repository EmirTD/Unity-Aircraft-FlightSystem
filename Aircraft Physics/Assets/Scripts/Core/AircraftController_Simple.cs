// AircraftController_Simple.cs
using UnityEngine;

public class AircraftController_Simple : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 50f; // �leri hareket h�z�
    [SerializeField] private float rotationSpeed = 100f; // D�n�� h�z�

    [Header("Stability")]
    [SerializeField] private float levelingSpeed = 5f; // D�zle�me h�z�

    private ControlInput controlInput;
    private Rigidbody rb;

    private void Awake()
    {
        controlInput = GetComponent<ControlInput>();
        rb = GetComponent<Rigidbody>();

        // Rigidbody'nin fiziksel hareketini devred��� b�rak
        // T�m hareketleri do�rudan transform ile kontrol edece�iz
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        // 1. �leri Hareket
        // U�ak her zaman ileri y�nde sabit bir h�zla hareket etsin
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 2. Kontrol Giri�lerine G�re D�n��
        // Kullan�c�n�n girdi de�erlerine g�re u�a�� d�nd�r
        float roll = -controlInput.SmoothedRollInput * rotationSpeed * Time.deltaTime;
        float pitch = -controlInput.SmoothedPitchInput * rotationSpeed * Time.deltaTime;
        float yaw = controlInput.SmoothedYawInput * rotationSpeed * Time.deltaTime;

        transform.Rotate(pitch, yaw, roll, Space.Self);

        // 3. Otomatik D�zle�me
        // E�er hi�bir kontrol girdisi yoksa u�a�� yava��a d�z konuma getir
        if (controlInput.RollInput == 0 && controlInput.PitchInput == 0 && controlInput.YawInput == 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * levelingSpeed);
        }
    }
}