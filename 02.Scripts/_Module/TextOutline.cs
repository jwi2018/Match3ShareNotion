﻿using UnityEngine;
using UnityEngine.Rendering;

public class TextOutline : MonoBehaviour
{
    //두께 설정
    public float pixelSize = 0.2f;

    public Color outlineColor = Color.black;

    //해상도에 따라 pixel size를 조정할지 결정
    public bool resolutionDependant;

    //설정된 Resolution보다 클 경우 pixel size 두 배로 설정
    public int doubleResolution = 1024;
    private bool isDead;
    private MeshRenderer meshRenderer;

    private TextMesh textMesh;

    private void Start()
    {
        isDead = false;
        textMesh = GetComponent<TextMesh>();
        meshRenderer = GetComponent<MeshRenderer>();

        for (var i = 0; i < 8; i++)
        {
            var outline = new GameObject("outline", typeof(TextMesh));
            outline.transform.parent = transform;
            outline.transform.localScale = new Vector3(1, 1, 1);
            var otherMeshRenderer = outline.GetComponent<MeshRenderer>();
            otherMeshRenderer.material = new Material(meshRenderer.material);
            otherMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            otherMeshRenderer.receiveShadows = false;
            otherMeshRenderer.sortingLayerID = meshRenderer.sortingLayerID;
            otherMeshRenderer.sortingLayerName = meshRenderer.sortingLayerName;
            otherMeshRenderer.sortingOrder = 9;
        }
    }

    private void LateUpdate()
    {
        if (isDead || Camera.main == null) return;
        //현재 원본 Text의 월드 좌표를 스크린 포인트로 맵핑합니다.
        var screenPoint = Vector3.zero;
        if (Camera.main != null)
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        outlineColor.a = textMesh.color.a * textMesh.color.a;

        //복제된 TextMesh 옵션 설정
        for (var i = 0; i < transform.childCount; i++)
        {
            //원본으로부터 복제된 자식(child)들을 불러옵니다.
            var other = transform.GetChild(i).GetComponent<TextMesh>();
            other.color = outlineColor;
            other.text = textMesh.text;
            other.alignment = textMesh.alignment;
            other.anchor = textMesh.anchor;
            other.characterSize = textMesh.characterSize;
            other.font = textMesh.font;
            other.fontSize = textMesh.fontSize;
            other.fontStyle = textMesh.fontStyle;
            other.richText = textMesh.richText;
            other.tabSize = textMesh.tabSize;
            other.lineSpacing = textMesh.lineSpacing;
            other.offsetZ = textMesh.offsetZ;

            //설정된 해상도(doubleResolution)보다 큰 디바이스에서 실행될 경우
            //pixelSize를 두배로 합니다.
            var doublePixel = resolutionDependant &&
                              (Screen.width > doubleResolution || Screen.height > doubleResolution);
            var pixelOffset = GetOffset(i) * (doublePixel ? 2.0f * pixelSize : pixelSize);
            var worldPoint = Camera.main.ScreenToWorldPoint(screenPoint + pixelOffset);
            other.transform.position = worldPoint;

            //레이어 오더
            var otherMeshRenderer = transform.GetChild(i).GetComponent<MeshRenderer>();
            otherMeshRenderer.sortingLayerID = meshRenderer.sortingLayerID;
            otherMeshRenderer.sortingLayerName = meshRenderer.sortingLayerName;
            otherMeshRenderer.sortingOrder = 9;
        }
    }

    public void Dead()
    {
        //원할 경우 복제들을 파괴합니다.
        isDead = true;
        for (var i = 0; i < transform.childCount; i++)
        {
            var other = transform.GetChild(i).gameObject;
            Destroy(other);
        }

        Destroy(this);
    }

    //복제된 TextMesh들의 배치정보
    private Vector3 GetOffset(int i)
    {
        switch (i % 8)
        {
            case 0: return new Vector3(0, 1, 0);
            case 1: return new Vector3(1, 1, 0);
            case 2: return new Vector3(1, 0, 0);
            case 3: return new Vector3(1, -1, 0);
            case 4: return new Vector3(0, -1, 0);
            case 5: return new Vector3(-1, -1, 0);
            case 6: return new Vector3(-1, 0, 0);
            case 7: return new Vector3(-1, 1, 0);
            default: return Vector3.zero;
        }
    }
}