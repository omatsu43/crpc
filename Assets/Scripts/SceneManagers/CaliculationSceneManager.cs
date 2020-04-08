using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaliculationSceneManager : MonoBehaviour
{
    public GameObject inputScreen;
    public GameObject resultScreen;
    public GameObject panelAlert;
    public InputField initialPSA;
    public ToggleGroup initialMetastasis;
    public ToggleGroup gleasonScore;
    public InputField timeToCRPC;
    public ToggleGroup chemotherapy;
    public InputField currentAge;
    public InputField currentPSA;
    public InputField currentALP;
    public InputField currentLDH;
    public Text oneYearSurvivalProbability;
    public Text twoYearSurvivalProbability;

    void Start()
    {
    }

    void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyUp (KeyCode.Escape)) {
            if (inputScreen.activeSelf) {
                SceneManager.LoadScene("TitleScene");
            } else {
                SwitchToInputScreen();
            }
        }
#endif
    }

    public void SwitchToResultScreen() {
        if (!ValidateSubmitAllOfFactors()) {
            panelAlert.SetActive(true);
            return;
        }

        oneYearSurvivalProbability.text = (CaliculateOneYearSurvivalProbability(
            int.Parse(initialPSA.text),
            initialMetastasis.ActiveToggles().First().GetComponentsInChildren<Localization.LocalizedText>().First(t => t.name == "Text").GetKey == "m1",
            int.Parse(gleasonScore.ActiveToggles().First().GetComponentsInChildren<Localization.LocalizedText>().First(t => t.name == "Text").GetKey),
            int.Parse(timeToCRPC.text),
            chemotherapy.ActiveToggles().First().GetComponentsInChildren<Localization.LocalizedText>().First(t => t.name == "Text").GetKey == "yes",
            int.Parse(currentAge.text),
            int.Parse(currentPSA.text),
            int.Parse(currentALP.text),
            int.Parse(currentLDH.text)
        ) * 100).ToString("f1");
        twoYearSurvivalProbability.text = (CaliculateTwoYearSurvivalProbability(
            int.Parse(initialPSA.text),
            initialMetastasis.ActiveToggles().FirstOrDefault().GetComponentsInChildren<Localization.LocalizedText>().First(t => t.name == "Text").GetKey == "m1",
            int.Parse(gleasonScore.ActiveToggles().FirstOrDefault().GetComponentsInChildren<Text>().First(t => t.name == "Text").text),
            int.Parse(timeToCRPC.text),
            chemotherapy.ActiveToggles().FirstOrDefault().GetComponentsInChildren<Localization.LocalizedText>().First(t => t.name == "Text").GetKey == "yes",
            int.Parse(currentAge.text),
            int.Parse(currentPSA.text),
            int.Parse(currentALP.text),
            int.Parse(currentLDH.text)
        ) * 100).ToString("f1");

        inputScreen.SetActive(false);
        resultScreen.SetActive(true);
    }
    public void SwitchToInputScreen() {
        resultScreen.SetActive(false);
        inputScreen.SetActive(true);
    }
    public void HideAlertPanel() {
        panelAlert.SetActive(false);
    }
    public void LoadTitleScene() {
        SceneManager.LoadScene("TitleScene");
    }

    private double CaliculateOneYearSurvivalProbability(
        int initialPSA,
        bool initialMetastasis,
        int gleasonScore,
        int timeToCRPC,
        bool chemotherapy,
        int currentAge,
        int currentPSA,
        int currentALP,
        int currentLDH
    ) {
        float intermediateResult = CaliculateIntermediateResult(initialPSA, initialMetastasis, gleasonScore, timeToCRPC, chemotherapy, currentAge, currentPSA, currentALP, currentLDH);
        double probability = Mathf.Exp((float) -0.000032104100556251 * Mathf.Exp(intermediateResult));
        return probability;
    }
    private double CaliculateTwoYearSurvivalProbability(
        int initialPSA,
        bool initialMetastasis,
        int gleasonScore,
        int timeToCRPC,
        bool chemotherapy,
        int currentAge,
        int currentPSA,
        int currentALP,
        int currentLDH
    ) {
        float intermediateResult = CaliculateIntermediateResult(initialPSA, initialMetastasis, gleasonScore, timeToCRPC, chemotherapy, currentAge, currentPSA, currentALP, currentLDH);
        double probability = Mathf.Exp((float) -0.0000915264669636689 * Mathf.Exp(intermediateResult));
        return probability;
    }
    private float CaliculateIntermediateResult(
        int initialPSA,
        bool initialMetastasis,
        int gleasonScore,
        int timeToCRPC,
        bool chemotherapy,
        int currentAge,
        int currentPSA,
        int currentALP,
        int currentLDH
    ) {
        double result = -0.053803056957926 * Mathf.Log(initialPSA) +
            0.232617660987787 * (initialMetastasis ? 1 : 0) +
            -0.0213650418712073 * gleasonScore +
            -0.0855340732399733 * Mathf.Log(timeToCRPC) +
            0.527923125838508 * (chemotherapy ? 1 : 0) +
            0.01182 * currentAge +
            0.146286383326766 * Mathf.Log(currentPSA) +
            0.265177952620299 * Mathf.Log(currentALP) +
            1.14730030641578 * Mathf.Log(currentLDH);
        return (float) result;
    }

    private bool ValidateSubmitAllOfFactors() {
        int _;
        return int.TryParse(initialPSA.text, out _) && 
            initialMetastasis.AnyTogglesOn() &&
            gleasonScore.AnyTogglesOn() &&
            int.TryParse(timeToCRPC.text, out _) &&
            chemotherapy.AnyTogglesOn() &&
            int.TryParse(currentAge.text, out _) &&
            int.TryParse(currentPSA.text, out _) &&
            int.TryParse(currentALP.text, out _) &&
            int.TryParse(currentLDH.text, out _);
    }
}