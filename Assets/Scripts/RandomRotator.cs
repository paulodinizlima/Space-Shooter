using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    //velocidade de giro do asteroide
    public float tumble = 5.0f;
    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        //pega o componente rigidbody do asteroide
        rigidbody = GetComponent<Rigidbody>();
        //gera valores aleatórios em uma esfera, multiplica pela
        //velocidade de giro e atribui à velocidade angular do asteroide
        rigidbody.angularVelocity = Random.insideUnitSphere * tumble;
    }
}
