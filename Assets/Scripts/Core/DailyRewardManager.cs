using UnityEngine;
using System;

public class DailyRewardManager : MonoBehaviour
{
    // Récompenses pour 7 jours : exemple avec argent, matériaux, objets
    [System.Serializable]
    public class Reward
    {
        public int coins;
        public string material; // nom du matériau, vide si aucun
        public string item;     // nom de l'objet fabriqué, vide si aucun
    }

    public Reward[] dailyRewards = new Reward[7]
    {
        // Jour 1 : 300 euros
        new Reward { coins = 300, material = "", item = "" },
        // Jour 2 : 500 euros, 12x bois de chene, 4x vernis
        new Reward { coins = 500, material = "12x bois de chene, 4x vernis", item = "" },
        // Jour 3 : 800 euros, 8x bois de pin, 4x vernis, 8x bois de chene
        new Reward { coins = 800, material = "8x bois de pin, 4x vernis, 8x bois de chene", item = "" },
        // Jour 4 : 1200 euros, 10x metal, 10x tissu
        new Reward { coins = 1200, material = "10x metal, 10x tissu", item = "" },
        // Jour 5 : 1800 euros, 5x chaise en pin
        new Reward { coins = 1800, material = "", item = "5x chaise en pin" },
        // Jour 6 : 2200 euros, 6x étagère mixe
        new Reward { coins = 2200, material = "", item = "6x étagère mixe" },
        // Jour 7 : 3000 euros, 3 tables en chene, 5 lampes, 3 etagere mixes, 5 chaises en pin
        new Reward { coins = 3000, material = "", item = "3x table en chene, 5x lampe, 3x étagère mixe, 5x chaise en pin" }
    };

    private int currentDay = 0;
    private const string LastClaimDateKey = "LastClaimDate";
    private const string CurrentDayKey = "CurrentDay";

    void Start()
    {
        CheckDailyReward();
    }

    // Rends la méthode publique pour l'accès UI
    public void CheckDailyReward()
    {
        string lastClaimDate = PlayerPrefs.GetString(LastClaimDateKey, "");
        string today = DateTime.Now.ToString("yyyyMMdd");

        if (lastClaimDate != today)
        {
            currentDay = PlayerPrefs.GetInt(CurrentDayKey, 0);
            if (currentDay >= dailyRewards.Length)
                currentDay = 0;
            GiveReward(currentDay);
            PlayerPrefs.SetString(LastClaimDateKey, today);
            PlayerPrefs.SetInt(CurrentDayKey, currentDay + 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Récompense déjà récupérée aujourd'hui !");
        }
    }

    void GiveReward(int day)
    {
        Reward reward = dailyRewards[day];
        if (reward == null) {
            Debug.LogWarning("Aucune récompense définie pour ce jour.");
            return;
        }
        // Récupère le GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) {
            Debug.LogError("GameManager non trouvé !");
            return;
        }
        if (reward.coins > 0)
        {
            gm.AddMoney(reward.coins);
            Debug.Log($"+{reward.coins} pièces !");
        }
        if (!string.IsNullOrEmpty(reward.material))
        {
            // Format attendu : "12x bois de chene, 4x vernis"
            string[] mats = reward.material.Split(',');
            foreach (string mat in mats)
            {
                string trimmed = mat.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                string[] parts = trimmed.Split('x');
                if (parts.Length == 2)
                {
                    int qty;
                    if (int.TryParse(parts[0].Trim(), out qty))
                    {
                        string matName = parts[1].Trim();
                        CraftingMaterial found = gm.craftingMaterials.Find(m => m.materialName.ToLower() == matName.ToLower());
                        if (found != null)
                        {
                            found.AddQuantity(qty);
                            Debug.Log($"+{qty} {matName} ajouté au stock");
                        }
                        else
                        {
                            Debug.LogWarning($"Matériau '{matName}' non trouvé dans GameManager");
                        }
                    }
                }
            }
        }
        if (!string.IsNullOrEmpty(reward.item))
        {
            // Format attendu : "5x chaise en pin, 3x table en chene"
            string[] items = reward.item.Split(',');
            foreach (string it in items)
            {
                string trimmed = it.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                string[] parts = trimmed.Split('x');
                if (parts.Length == 2)
                {
                    int qty;
                    if (int.TryParse(parts[0].Trim(), out qty))
                    {
                        string prodName = parts[1].Trim();
                        Product found = gm.products.Find(p => p.productName.ToLower() == prodName.ToLower());
                        if (found != null)
                        {
                            found.AddQuantity(qty);
                            Debug.Log($"+{qty} {prodName} ajouté au stock de produits finis");
                        }
                        else
                        {
                            Debug.LogWarning($"Produit '{prodName}' non trouvé dans GameManager");
                        }
                    }
                }
            }
        }
        Debug.Log($"Récompense journalière du jour {day + 1} attribuée.");
    }
}
