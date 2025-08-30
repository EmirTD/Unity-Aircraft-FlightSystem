// AircraftController_Simple.cs
using UnityEngine;

public class AircraftController_Simple : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 50f; // Ýleri hareket hýzý
    [SerializeField] private float rotationSpeed = 100f; // Dönüþ hýzý

    [Header("Stability")]
    [SerializeField] private float levelingSpeed = 5f; // Düzleþme hýzý

    private ControlInput controlInput;
    private Rigidbody rb;

    private void Awake()
    {
        controlInput = GetComponent<ControlInput>();
        rb = GetComponent<Rigidbody>();

        // Rigidbody'nin fiziksel hareketini devredýþý býrak
        // Tüm hareketleri doðrudan transform ile kontrol edeceðiz
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        // 1. Ýleri Hareket
        // Uçak her zaman ileri yönde sabit bir hýzla hareket etsin
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 2. Kontrol Giriþlerine Göre Dönüþ
        // Kullanýcýnýn girdi deðerlerine göre uçaðý döndür
        float roll = -controlInput.SmoothedRollInput * rotationSpeed * Time.deltaTime;
        float pitch = -controlInput.SmoothedPitchInput * rotationSpeed * Time.deltaTime;
        float yaw = controlInput.SmoothedYawInput * rotationSpeed * Time.deltaTime;

        transform.Rotate(pitch, yaw, roll, Space.Self);

        // 3. Otomatik Düzleþme
        // Eðer hiçbir kontrol girdisi yoksa uçaðý yavaþça düz konuma getir
        if (controlInput.RollInput == 0 && controlInput.PitchInput == 0 && controlInput.YawInput == 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * levelingSpeed);
        }
    }
}