using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class Puzzle2Manager : NetworkBehaviour
{
    [SerializeField] private int[] correctSequence = { 1, 3, 0, 2 };
    [SerializeField] private Puzzle2Plate[] plates;
    [SerializeField] private GameObject solvedTextObject;
    public GameObject winLights;
    public bool puzzleActive = true;
    private int currentStep = 0;
    private void Start()
    {
        solvedTextObject.SetActive(false);
    }

    public void StepOnPlate(int plateIndex)
    {
        if (!IsServer) return;
        if (!puzzleActive) return;
        if (currentStep >= correctSequence.Length) return;
        if (plateIndex == correctSequence[currentStep])
        {
            currentStep++;
            Debug.Log("Correct!");
            CorrectPlateClientRpc(plateIndex);
            if (currentStep >= correctSequence.Length)
            {
                PuzzleSolved();
            }
        }
        else
        {
            Debug.Log("WRONG!");
            WrongPlateClientRpc();
            ResetPuzzle();
        }
    }

    private void PuzzleSolved()
    {
        Debug.Log("PUZZLE SOLVED");
        puzzleActive = false;
        currentStep = 0;
        AllPlatesGreenClientRpc();
        ShowSolvedTextClientRpc();
    }

    private void ResetPuzzle()
    {
        currentStep = 0;
    }

    [ClientRpc]
    private void ShowSolvedTextClientRpc()
    {
        solvedTextObject.SetActive(true);
        winLights.SetActive(true);
    }

    [ClientRpc]
    private void CorrectPlateClientRpc(int plateIndex)
    {
        plates[plateIndex].SetGreen();
    }

    [ClientRpc]
    private void AllPlatesGreenClientRpc()
    {
        foreach (var plate in plates)
        {
            plate.SetGreen();
        }
    }

    [ClientRpc]
    private void WrongPlateClientRpc()
    {
        StartCoroutine(WrongFlash());
    }

    private IEnumerator WrongFlash()
    {
        foreach (var plate in plates)
            plate.SetRed();

        yield return new WaitForSeconds(1f);

        foreach (var plate in plates)
            plate.ResetColor();
    }
}
