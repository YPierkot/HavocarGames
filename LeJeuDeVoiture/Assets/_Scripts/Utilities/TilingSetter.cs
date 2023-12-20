using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TilingSetter : MonoBehaviour {
    public bool setRandomRot = true;
    public float maxAngle;
    public int maxQuarter;
    public GameObject[] possibleTiles;
    public Material[] possibleMaterials;
    public Vector2 tilingSize;
    public List<GameObject> tiles;
    public RectTransform rectTransform;
    public BoxCollider collider;
    
    [ContextMenu("SetRandomAngle")]
    public void SetRandomAngles() {
        if (!setRandomRot) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).rotation = Quaternion.Euler(-90,Random.Range(0,maxQuarter)*90 + Random.Range(-maxAngle,maxAngle),0);
        }
    }
    
    [ContextMenu("SetTiling")]
    public void SetTiling()
    {
        int tileXAmount = Mathf.RoundToInt(rectTransform.rect.width / tilingSize.x);
        int tileYAmount = Mathf.RoundToInt(rectTransform.rect.height / tilingSize.y);

        for (int i = 0; i < tiles.Count; i++)
        {
            DestroyImmediate(tiles[i]);
        }
        tiles.Clear();
        
        for (int x = 0; x < tileXAmount; x++)
        {
            for (int y = 0; y < tileYAmount; y++)
            {
                Vector3 pos = new Vector3(-rectTransform.rect.width / 2f + (tilingSize.x / 2) + (x * tilingSize.x),-rectTransform.rect.height / 2f + (tilingSize.y / 2) + (y * tilingSize.y),0);
                tiles.Add(Instantiate(possibleTiles[Random.Range(0, possibleTiles.Length)], Vector3.zero, Quaternion.identity, transform));
                tiles[tiles.Count - 1].transform.localPosition = pos;
                tiles[tiles.Count - 1].GetComponent<MeshRenderer>().material = possibleMaterials[Random.Range(0, possibleMaterials.Length)];
            }
        }

        if (collider)
        {
            var colliderSize = collider.size;
            colliderSize.x = rectTransform.rect.width;
            colliderSize.y = rectTransform.rect.height;
            collider.size = colliderSize;
        }
        
        SetRandomAngles();
    }
}
