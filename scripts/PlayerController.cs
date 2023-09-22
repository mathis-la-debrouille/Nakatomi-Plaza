using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


    public enum PlayState
    {
        Preparing,
        Played,
        Waiting,
    }

public class PlayerController : MonoBehaviour
{

    // relatives to movment
    public float moveSpeed;

    public bool isMoving;

    public Vector2 input;

    // relatives to player states

    public PlayState playerState;
    private Rigidbody2D rb;
    public bool played;
    public bool confirmedPlay;
    public TextMeshProUGUI confirmText;



    // relatives to actions
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool isDragging = false;

    private float maxSpeed = 3f; 
    private float maxTime = 3f;
    private float minTime = 2f; 
    public float speedIncreaser = 1f;
    public float bounceForce = 1f;
    public GameObject movArrow;
    public GameObject attackArrow;
    public GameObject specialArrow;

    public Vector2 currentVelocity;
    private Vector3 arrowBasePosition; 
    private float arrowMaxLength = 2f; 
    private float arrowMinLength = 0.5f; 


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        movArrow.SetActive(false);
        confirmText.enabled = false;
        played = false;
        confirmedPlay = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && played)
        {
            confirmedPlay = true;
            confirmText.enabled = false;
            played = false;
        }
        if (isMoving)
        {
            if (rb.velocity.magnitude < 0.1f)
            {
                rb.velocity = Vector2.zero;
                isMoving = false;
            } else if (rb.velocity.magnitude > 0) {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.1f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Calculez la direction de la collision
            Vector2 collisionDirection = (transform.position - collision.transform.position).normalized;

            // Appliquez une force opposée à la direction de la collision pour le rebond
            rb.AddForce(collisionDirection * bounceForce, ForceMode2D.Impulse);
        }
    }

    private void OnMouseDown()
    {
        startPosition = transform.position;
        isDragging = true;
        movArrow.SetActive(true);
        confirmText.enabled = false;
        played = false;
    }

        private void OnMouseDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        movArrow.transform.position = startPosition;

        // Calcul la direction et la magnitude du déplacement
        Vector3 direction = mousePosition - startPosition;
        float distance = direction.magnitude;

        // Calcul le temps nécessaire pour atteindre la vitesse maximale
        float requiredTime = Mathf.Clamp(distance / maxSpeed, minTime, maxTime);

        // Calcul la vitesse en fonction du temps
        float speed = distance / requiredTime;

        // Calcul la force en fonction de la vitesse
        float force = rb.mass * speed / Time.fixedDeltaTime;

        // Calculs pour le render de la flèche
        float arrowlength = Mathf.Sqrt(Mathf.Pow((mousePosition.x - startPosition.x), 2) + Mathf.Pow((mousePosition.y - startPosition.y), 2));
        movArrow.transform.localScale = new Vector3(arrowlength*3, arrowlength *3   * 0.6f, 1f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        movArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        movArrow.transform.position = startPosition;

        // Calcul la position finale
        endPosition = startPosition + direction.normalized * distance;
        
    }


    private void OnMouseUp()
    {
        isDragging = false;
        movArrow.SetActive(false);
        confirmText.enabled = true;
        played = true;

        
        rb.isKinematic = false;

        // Calcule la vitesse requise pour atteindre la destination en 3 secondes maximum
        float requiredSpeed = Vector2.Distance(transform.position, endPosition) / 3f;
        float speed = Mathf.Min(requiredSpeed, maxSpeed);

        Vector2 direction = ((Vector2)endPosition - (Vector2) transform.position).normalized;

        // Applique la vitesse dans la direction calculée
        currentVelocity = direction * speed * speedIncreaser;
        Debug.Log(currentVelocity.ToString());

        isMoving = true;
    }


    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }
}
