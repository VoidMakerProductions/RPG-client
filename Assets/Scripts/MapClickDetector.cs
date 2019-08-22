using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MapClickDetector : MonoBehaviour, IPointerDownHandler

{
    public Tilemap tilemap;

    public void OnPointerDown(PointerEventData eventData)
    {
        
        Vector3Int grid = tilemap.WorldToCell(eventData.pointerCurrentRaycast.worldPosition);
        Debug.Log(grid);
    }

    

    
}
