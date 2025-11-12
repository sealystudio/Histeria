using UnityEngine;

public class FlashLightAttack : MonoBehaviour
{
    public float lifeTime = 0.2f;
    public float fadeTime = 0.2f;
    private SpriteRenderer sr;
    private float timer = 0f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {
        Destroy(gameObject, lifeTime + fadeTime);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float fade = 1f;
        if (timer > lifeTime)
            fade = 1f - (timer - lifeTime) / fadeTime;

        Color c = sr.color;
        c.a = Mathf.Clamp01(fade);
        sr.color = c;
    }
}
