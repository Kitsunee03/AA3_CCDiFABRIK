using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI currentTaskText;
    [SerializeField] private TextMeshProUGUI algorithmText;
    [SerializeField] private TextMeshProUGUI iterationsText;
    [SerializeField] private TextMeshProUGUI distanceText;

    private string lastIterations = "0";
    private string lastDistances = "-1";

    private DronController dronController;

    private void Awake()
    {
        dronController = FindFirstObjectByType<DronController>();
        dronController.onTaskChanged.AddListener(SetTaskDisplay);
    }

    private void SetTaskDisplay(string p_info) { currentTaskText.text = p_info; }

    private void Update()
    {
        UpdateInfoDisplay();
    }

    private void UpdateInfoDisplay()
    {
        algorithmText.text = dronController.ActiveAlgorithm();

        string iterations = "";
        if (dronController.IsCCDActive())
        {
            string ccd = dronController.CCDIterationsThisFrame();
            iterations += ccd;
        }
        if (dronController.IsFABRIKActive())
        {
            if (iterations != "") iterations += " | ";
            iterations += dronController.FABRIKIterationsThisFrame();
        }

        if (iterations != lastIterations)
        {
            lastIterations = iterations;
            iterationsText.text = "Iterations: " + iterations;
        }

        string distances = "";
        if (dronController.IsCCDActive()) distances += dronController.CCDDistanceToTarget();
        if (dronController.IsFABRIKActive())
        {
            if (distances != "") distances += " | ";
            distances += dronController.FABRIKDistanceToTarget();
        }

        if (distances != lastDistances)
        {
            lastDistances = distances;
            distanceText.text = "Distance: " + distances;
        }
    }

    private void OnDestroy()
    {
        if (dronController != null)
        {
            dronController.onTaskChanged.RemoveListener(SetTaskDisplay);
        }
    }
}