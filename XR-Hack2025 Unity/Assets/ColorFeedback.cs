using System.Collections;
using UnityEngine;

public class ColorFeedback : MonoBehaviour
{
      [SerializeField] private Material envMaterial;
      private float coolDownTime = 2;
      private bool canChangeColor = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerExit(Collider other)
    {
        if (canChangeColor)
        {
            StartCoroutine(ColorChangeCooldown());
        }

    }
    private void ColorChange()
    {
        Color[] colors = {Color.red, Color.blue, Color.yellow};
        int randomIndex = UnityEngine.Random.Range(0, colors.Length);
        while (colors[randomIndex] == envMaterial.color)
        {
            randomIndex = UnityEngine.Random.Range(0, colors.Length);

        }
         envMaterial.color = colors[randomIndex];

    }

    private IEnumerator ColorChangeCooldown()
    {
      
            ColorChange();
            canChangeColor = false;
            yield return new WaitForSeconds(coolDownTime);
            canChangeColor = true;
            
       
    }


}
