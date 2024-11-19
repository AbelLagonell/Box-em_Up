using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemChange : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private Sprite[] abilitySprites;
    private int amount = 1;

    public void Init(AbilityType ability) {
        image.sprite = abilitySprites[(int)ability];
        amountText.text = "";
    }

    public void Init(UpgradeType upgrade) {
        image.sprite = itemSprites[(int)upgrade];
        amountText.text = amount.ToString();
    }

    public void Init(UpgradeType upgrade, int newAmount) {
        image.sprite = itemSprites[(int)upgrade];
        amountText.text = newAmount.ToString();
    }

    public void IncrementAmount() {
        amount++;
        amountText.text = amount.ToString();
    }
}