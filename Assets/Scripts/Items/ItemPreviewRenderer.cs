
using UnityEngine;
using System.Collections.Generic;

public class ItemPreviewRenderer : MonoBehaviour
{
    [SerializeField] private Camera previewCamera;
    [SerializeField] private int textureSize = 256;

    private Dictionary<ItemData, RenderTexture> previews = new Dictionary<ItemData, RenderTexture>();
    private int previewLayer;
    private Transform previewRoot;

    void Awake()
    {
        previewLayer = LayerMask.NameToLayer("ItemPreview");

        GameObject root = new GameObject("InventoryPreviewModels");
        previewRoot = root.transform;
        previewRoot.position = new Vector3(0, -1000, 0);

        if (previewCamera != null)
        {
            previewCamera.enabled = false;
            previewCamera.orthographic = true;
            previewCamera.cullingMask = 1 << previewLayer;
        }
    }

    public RenderTexture GetPreview(ItemData item)
    {
        if (item == null || item.worldModel == null || previewCamera == null || previewLayer < 0)
        {
            return null;
        }

        if (previews.TryGetValue(item, out RenderTexture existing))
        {
            return existing;
        }

        GameObject model = Instantiate(item.worldModel, previewRoot);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.SetActive(true);

        foreach (MonoBehaviour script in model.GetComponentsInChildren<MonoBehaviour>())
        {
            script.enabled = false;
        }

        foreach (Collider collider in model.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        foreach (Transform child in model.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = previewLayer;
        }

        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(model.transform.position, Vector3.zero);
        bool foundRenderer = false;

        foreach (Renderer renderer in renderers)
        {
            if (!foundRenderer)
            {
                bounds = renderer.bounds;
                foundRenderer = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (!foundRenderer)
        {
            Destroy(model);
            return null;
        }

        RenderTexture texture = new RenderTexture(textureSize, textureSize, 16, RenderTextureFormat.ARGB32);
        texture.Create();

        previewCamera.transform.position = bounds.center + new Vector3(0, 0, -10);
        previewCamera.transform.rotation = Quaternion.identity;
        previewCamera.orthographicSize = Mathf.Max(bounds.extents.x, bounds.extents.y) * 1.35f;
        previewCamera.targetTexture = texture;

        previewCamera.Render();

        previewCamera.targetTexture = null;
        Destroy(model);

        previews.Add(item, texture);
        return texture;
    }

    void OnDestroy()
    {
        foreach (RenderTexture texture in previews.Values)
        {
            if (texture != null)
            {
                texture.Release();
                Destroy(texture);
            }
        }

        if (previewRoot != null)
        {
            Destroy(previewRoot.gameObject);
        }
    }
}
