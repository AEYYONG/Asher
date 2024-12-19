using UnityEngine;

[CreateAssetMenu(fileName = "VFXData", menuName = "VFX/Create VFX Data")]
public class VFXData : ScriptableObject
{
    public string vfxName; //vfx 이름
    public GameObject vfxPrefab;  // VFX Prefab
    public float duration;        // VFX 지속시간
    public Vector3 scale = Vector3.one; // 스케일
    public Color vfxColor = Color.white; // 색상
}
