using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOrderScript : MonoBehaviour {
    static int                  mostFrontOrder = 100;

    [SerializeField] Renderer[] backRenderers;
    [SerializeField] Renderer[] middleRenderers;
    [SerializeField] string     sortingLayerName;

    List<GameObject>            grids = new List<GameObject>();
    int                         originOrder;

	public void SetOrder(int _order) {
        int mulOrder = _order * 10;

        foreach(var renderer in backRenderers) {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder;
        }

        foreach (var renderer in middleRenderers) {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder + 1;
        }

        foreach (var obj in grids) {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
			renderer.sortingLayerName = sortingLayerName;
			renderer.sortingOrder = mulOrder + 2;
		}
    }

    public void SetOriginOrder() {
        SetOrder(originOrder);
    }

    public void SetOriginOrder(int _originOrder) {
        originOrder = _originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool _mostFront) {
        SetOrder(_mostFront ? mostFrontOrder : originOrder);
    }
}
