using UnityEngine;
using System.Collections;

public class BuildCubemap : MonoBehaviour
{
    public Camera cam;
    public Cubemap cubeMap;

	void OnEnable()
    {
        cam.RenderToCubemap(cubeMap);
    }
}
