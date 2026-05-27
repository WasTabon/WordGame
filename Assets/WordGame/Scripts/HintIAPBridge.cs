using UnityEngine;

public class HintIAPBridge : MonoBehaviour
{
    public void OnPurchaseComplete()
    {
        HintManager.GrantTenHints();
    }

    public void OnPurchaseFailed()
    {
        HintManager.OnPurchaseFailed();
    }

    public void OnProductFetched()
    {
        HintManager.OnProductFetched();
    }
}
