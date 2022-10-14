using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private float timer;

    public void Init(int number)
    {
        coinText.text = number.ToString();
    }

    public void Update()
    {
        if (timer <= 0)
        {
            Destroy(gameObject);
            return;
        }
        timer -= Time.deltaTime;
        transform.Translate(Vector3.up * Time.deltaTime);
    }
}
