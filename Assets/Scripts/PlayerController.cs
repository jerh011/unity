using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    private int count;
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public AudioSource audioSource;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoPickUp;

    // Asegúrate de asignar esto en el Inspector (arrastrar el TextMeshProUGUI)
    public TextMeshProUGUI countText;
    // Objeto que contiene el contador (si lo ocultas al ganar)
    public GameObject countTextObject;
    // Objeto que muestra el texto de victoria/derrota (debe contener TextMeshProUGUI o ser manejado como GameObject)
    public GameObject winTextObject;

    void Awake()
    {
        // Intentar obtener Rigidbody si no lo asignaste en Inspector
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rb == null)
            Debug.LogError($"[{name}] Rigidbody no encontrado. Agrega un Rigidbody al GameObject.");

        // Comprobar referencias públicas para evitar NullReference más adelante
        if (countText == null)
            Debug.LogError($"[{name}] countText (TextMeshProUGUI) NO está asignado en el Inspector.");
        if (countTextObject == null)
            Debug.LogWarning($"[{name}] countTextObject no está asignado. Si piensas ocultarlo al ganar, asígnalo.");
        if (winTextObject == null)
            Debug.LogWarning($"[{name}] winTextObject no está asignado. Si piensas mostrar texto al ganar/perder, asígnalo.");

        count = 0;
        SetCountText();

        if (winTextObject != null)
            winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * Speed);

        // Limitar la posición después de aplicar física
        float x = Mathf.Clamp(rb.position.x, -9f, 9f);
        float z = Mathf.Clamp(rb.position.z, -9f, 9f);
        rb.position = new Vector3(x, rb.position.y, z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;

            // 🎵 reproducir sonido de recoger
            if (audioSource != null && sonidoPickUp != null)
                audioSource.PlayOneShot(sonidoPickUp);

            SetCountText();
        }
    }


    private void SetCountText()
    {
        // Protecciones: si no hay countText, no intentes acceder a .text
        if (countText != null)
        {
            if (count < 10)
                countText.text = "Count: 0" + count.ToString() + "/16";
            else
                countText.text = "Count: " + count.ToString() + "/16";
        }

        if (count >= 16)
        {
            if (winTextObject != null) winTextObject.SetActive(true);
            if (countTextObject != null) countTextObject.SetActive(false);

            // Llamar corrutina solo si el script está activo
            StartCoroutine(CambiarEscenaConRetraso());
        }
    }
    IEnumerator CambiarEscenaConRetraso()
    {
        if (audioSource != null && sonidoVictoria != null)
            audioSource.PlayOneShot(sonidoVictoria);

        yield return new WaitForSeconds(5f); // Espera 5 segundos

        SceneManager.LoadScene("Menu");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destruye el jugador (o podrías desactivarlo para hacer respawn)
            Destroy(gameObject);

            if (winTextObject != null)
            {
                winTextObject.SetActive(true);

                // Intentar cambiar el texto si existe un TextMeshProUGUI en el objeto
                var tmp = winTextObject.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = "You Lose!";
                }
                else
                {
                    // Si winTextObject no tiene TextMeshProUGUI, intentar buscar en hijos
                    var childTmp = winTextObject.GetComponentInChildren<TextMeshProUGUI>();
                    if (childTmp != null) childTmp.text = "You Lose!";
                    else Debug.LogWarning($"[{name}] winTextObject no contiene TextMeshProUGUI para mostrar 'You Lose!'.");
                }
            }
            else
            {
                Debug.LogWarning($"[{name}] winTextObject no asignado: no se mostrará 'You Lose!'.");
            }
        }
    }
}
