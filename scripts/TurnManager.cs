using UnityEngine;
using TMPro;
using System.Collections;


public class TurnManager : MonoBehaviour
{
    public TextMeshProUGUI turnText; // Texte pour afficher le tour actuel
    public TextMeshProUGUI phaseText; // Texte pour afficher la phase actuelle
    private int currentTurn = 1;
    private enum TurnPhase { Movement, Waiting, Finished };
    private TurnPhase currentPhase = TurnPhase.Movement;
    public GameObject player1;
    private PlayerController Player1;

    private void Start()
    {
        Player1 = player1.GetComponent<PlayerController>();
        UpdateUIText();
        // StartCoroutine(TransitionToWaitingPhase());
    }

    private void LateUpdate()
    {
        if (Player1.confirmedPlay && currentPhase == TurnPhase.Movement) {
            StartCoroutine(TransitionToWaitingPhase());
            Player1.confirmedPlay = false;
        }
    }

    private IEnumerator TransitionToWaitingPhase()
    {
        yield return new WaitForSeconds(0.5f);
        currentPhase = TurnPhase.Waiting;
        UpdateUIText();
        StartCoroutine(TransitionToFinishedPhase());
    }

    private IEnumerator TransitionToFinishedPhase()
    {
        yield return new WaitForSeconds(3f);
        currentPhase = TurnPhase.Finished;
        UpdateUIText();
        yield return new WaitForSeconds(3f);

        currentPhase = TurnPhase.Movement;
        currentTurn++;
        UpdateUIText();

        if (player1 != null)
        {
            Rigidbody2D rb = player1.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Player1.currentVelocity;
            }
        }
    }

    private void UpdateUIText()
    {
        turnText.text = "Tour " + currentTurn;
        switch (currentPhase)
        {
            case TurnPhase.Movement:
                phaseText.text = "Déplacement";
                break;
            case TurnPhase.Waiting:
                phaseText.text = "En attente de l'adversaire";
                break;
            case TurnPhase.Finished:
                phaseText.text = "Tour fini";
                break;
        }
    }
}
