using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private const float duree = 0.8f;
    private const float vitesseMontee = 1.5f;

    private TextMeshPro texte;
    private float timer;
    private Color couleur;

    public static void Creer(Vector3 position, float montant, Color couleur)
    {
        GameObject go = new GameObject("DamagePopup");
        go.transform.position = position;

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = montant.ToString("0.#");
        tmp.fontSize = 6f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = couleur;
        tmp.sortingOrder = 100; // passe devant les sprites

        DamagePopup popup = go.AddComponent<DamagePopup>();
        popup.texte = tmp;
        popup.couleur = couleur;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * vitesseMontee * Time.deltaTime;

        couleur.a = 1f - timer / duree;
        texte.color = couleur;

        if (timer >= duree) Destroy(gameObject);
    }
}