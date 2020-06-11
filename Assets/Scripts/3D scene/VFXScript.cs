using UnityEngine;

[ExecuteInEditMode]
public class VfxScript : MonoBehaviour
{
    [SerializeField]private Material _mat;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, _mat);
    }
}
