using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class BlurSpriteRenderer : MonoBehaviour

{

  public SpriteRenderer spriteRenderer; // Assign in Inspector



 public Material blurMaterial; // Assign in Inspector



 public Material originalMaterial; // To store the original material







 void Start()



 {



  if (spriteRenderer != null)



  {



   originalMaterial = spriteRenderer.material;



  }



 }







 public void ApplyBlur()



 {



  if (spriteRenderer != null && blurMaterial != null)



  {



   spriteRenderer.material = blurMaterial;



  }



 }







 public void RemoveBlur()



 {



  if (spriteRenderer != null && originalMaterial != null)



  {



   spriteRenderer.material = originalMaterial;



  }



 }

}