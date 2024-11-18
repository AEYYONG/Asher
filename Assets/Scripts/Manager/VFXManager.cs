using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    // 딕셔너리로 VFX 관리
    private Dictionary<string, VFXData> vfxDictionary = new Dictionary<string, VFXData>();

    [SerializeField]
    private List<VFXData> vfxDataList;

    void Start()
    {
        // VFX 데이터 초기화
        foreach (var vfxData in vfxDataList)
        {
            if (vfxData != null && vfxData.vfxPrefab != null)
            {
                vfxDictionary[vfxData.vfxName] = vfxData;
            }
        }
    }

    public void PlayVFX(string vfxName, Vector3 position, Quaternion rotation)
    {
        if (vfxDictionary.TryGetValue(vfxName, out VFXData vfxData))
        {
            // VFX 생성
            GameObject vfxInstance = Instantiate(vfxData.vfxPrefab, position, rotation);

            // 스케일 적용
            vfxInstance.transform.localScale = vfxData.scale;

            // 색상 적용 (Material 변경)
            var renderer = vfxInstance.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = vfxData.vfxColor;
            }

            // 일정 시간 후 삭제
            Destroy(vfxInstance, vfxData.duration);
        }
        else
        {
            Debug.LogWarning($"VFX {vfxName} not found in dictionary!");
        }
    }
}
