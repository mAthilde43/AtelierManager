using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyRewardUI : MonoBehaviour
{
    [Header("Références UI")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI rewardDescriptionText;
    public Button claimButton;
    [Header("Référence au script de récompense")]
    public DailyRewardManager rewardManager;

    void Start()
    {
        // Toujours afficher la bonne description, même si le panneau est masqué
        ShowTodayReward();
        if (IsRewardAvailableToday())
        {
            panel.SetActive(true);
            claimButton.interactable = true;
        }
        else
        {
            panel.SetActive(false);
            claimButton.interactable = false;
        }
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(ClaimReward);
    }

    bool IsRewardAvailableToday()
    {
        string lastClaimDate = PlayerPrefs.GetString("LastClaimDate", "");
        string today = System.DateTime.Now.ToString("yyyyMMdd");
        return lastClaimDate != today;
    }

    void ShowTodayReward()
    {
        // Affiche la récompense du jour
        int day = PlayerPrefs.GetInt("CurrentDay", 0);
        if (day >= rewardManager.dailyRewards.Length)
            day = 0;
        var reward = rewardManager.dailyRewards[day];
        string desc = "";
        if (reward.coins > 0) desc += reward.coins + " €\n";
        if (!string.IsNullOrEmpty(reward.material)) desc += reward.material + "\n";
        if (!string.IsNullOrEmpty(reward.item)) desc += reward.item + "\n";
        rewardDescriptionText.text = desc.Trim();
    }

    void ClaimReward()
    {
        if (!IsRewardAvailableToday())
        {
            panel.SetActive(false);
            return;
        }
        rewardManager.CheckDailyReward();
        panel.SetActive(false);
        claimButton.interactable = false;
    }
}