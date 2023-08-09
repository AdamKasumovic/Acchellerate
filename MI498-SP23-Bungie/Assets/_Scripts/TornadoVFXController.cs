using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TornadoVFXController : MonoBehaviour
{
    private Transform car;
    private CarManager cm;
    private VisualEffect tornadoVFX;
    private bool wasTornado = false;
    // Start is called before the first frame update
    void Start()
    {
        car = CarManager.Instance.transform;
        cm = CarManager.Instance;
        tornadoVFX = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!wasTornado && cm.tornado)
        {
            tornadoVFX.enabled = true;
            tornadoVFX.Play();
        }
        else if (!cm.tornado && wasTornado)
        {
            tornadoVFX.enabled = false;
        }
        if (cm.tornado)
        {
            int layerMask = 1 << 0;
            RaycastHit hit;
            Physics.Raycast(car.position, Vector3.down, out hit, Mathf.Infinity, layerMask);
            //Debug.Log(hit.point);
            transform.position = hit.point;
            transform.localScale = new Vector3(2, (car.position.y - hit.point.y) / 3, 2);
        }

        wasTornado = cm.tornado;
    }
}
