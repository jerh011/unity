using UnityEngine;

public class Rotator : MonoBehaviour
{
  
    void Update()
    {
        transform.Rotate(new Vector3(20, 0, 45) * Time.deltaTime);
    }
}
