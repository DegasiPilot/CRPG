using UnityEngine;

public class SkinPart : MonoBehaviour
{
    public bool HasDefaultSkin;
    public Mesh _defaultMesh;
    public Material _defaultMaterial;
    public Mesh _specialMesh;

    public bool IsEmpty { get; private set; } = true;

    private MeshFilter _meshFilter;
    private Renderer _renderer;
    private bool _isSkinedRender;

    private void Awake()
    {
        _isSkinedRender = !TryGetComponent(out _meshFilter);
        _renderer = GetComponent<Renderer>();
    }

    public void SetSkin(GameObject skin)
    {
        Mesh mesh = skin.GetComponentInChildren<MeshFilter>().mesh;
        Material material = skin.GetComponentInChildren<Renderer>().material;

        transform.localPosition = skin.transform.localPosition;
        transform.localScale = transform.localScale;
        if (_isSkinedRender) (_renderer as SkinnedMeshRenderer).sharedMesh = mesh;
        else _meshFilter.mesh = mesh;
        _renderer.enabled = true;
        _renderer.material = material;
        IsEmpty = false;
    }

    public void ResetSkin(bool toSpecial = false)
    {
        if (!HasDefaultSkin)
        {
            _renderer.enabled = false;
        }
        else
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            Mesh mesh = toSpecial && _specialMesh != null ? _specialMesh : _defaultMesh;
            if (_isSkinedRender) (_renderer as SkinnedMeshRenderer).sharedMesh = mesh;
            else _meshFilter.mesh = mesh;
            _renderer.material = _defaultMaterial;
        }
        IsEmpty = true;
    }
}
