using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NameInputManager : MonoBehaviour
{
    public GameObject nameInputPanel;
    public TMP_InputField inputField;
    public Button validateButton;

    void Start()
    {
        nameInputPanel.SetActive(false);
        validateButton.onClick.AddListener(OnValidate);
    }

    // Appelée à la fin du tutoriel
    public void ShowNameInputIfNewGame()
    {
        // Affiche le panneau uniquement si aucune sauvegarde n'existe
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            nameInputPanel.SetActive(true);
        }
    }

    void OnValidate()
    {
        string name = inputField.text;
        if (!string.IsNullOrEmpty(name))
        {
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.Save();
            nameInputPanel.SetActive(false);
            Debug.Log($"Nom sauvegardé: {name}");
        }
    }
}

