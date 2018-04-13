using UnityEngine;
using System.Collections;

public class InspectorGrid : MonoBehaviour {

    public GameObject inspectObj;
    public int columns = 5, rows = 5;
    float spaceSize = 0.5f;
    [Range(0, 1)]
    public float objSize = 0.75f;
    BoxCollider2D box;

	void Start () {
        SpawnGrid(); 
    }
	
	void Update () {
	
	}

    void SpawnGrid()
    {
        box = GetComponent<BoxCollider2D>();
        spaceSize = box.size.x / columns;
        //float boxHeight = box.size.y;
        float scale = spaceSize * objSize;

        int i = 0;
        for (int c = 0; c < columns; ++c)
        {
            for (int r = 0; r < rows; ++r)
            {
                Vector2 pos;
                pos.x = transform.position.x -((spaceSize * columns) / 2) + (spaceSize * c) + spaceSize / 2;
                pos.y = transform.position.y + ((spaceSize * rows) / 2) - (spaceSize * r) + spaceSize / 2 - spaceSize;

                GameObject g = Instantiate(inspectObj) as GameObject;
                g.transform.localScale = new Vector3(scale, scale, g.transform.localScale.z);
                g.transform.position = pos;

                g.transform.parent = transform;

                ++i;
            }
        }
    }
}
