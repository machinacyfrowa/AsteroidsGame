using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float flySpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //dodaj do współrzędnych wartość x=1, y=0, z=0 pomnożone przez czas
        //mierzony w sekundach od ostatniej klatki
        //transform.position += new Vector3(1, 0, 0) * Time.deltaTime;

        //prezentacja działania wygładzonego sterowania (emualcja joystika)
        //Debug.Log(Input.GetAxis("Vertical"));

        //sterowanie prędkością
        //stworz nowy wektor przesunięcia o wartości 1 do przodu
        Vector3 movement = transform.forward;
        //pomnóż go przez czas od ostatniej klatki
        movement *= Time.deltaTime;
        //pomnóż go przez "wychylenie joystika"
        movement *= Input.GetAxis("Vertical");
        //pomnóż przez prędkość lotu
        movement *= flySpeed;
        //dodaj ruch do obiektu
        //zmiana na fizyke
        // --- transform.position += movement;

        //komponent fizyki wewnątrz gracza
        Rigidbody rb = GetComponent<Rigidbody>();
        //dodaj siłe - do przodu statku w trybie zmiany prędkości
        rb.AddForce(movement, ForceMode.VelocityChange);


        //obrót
        //modyfikuj oś "Y" obiektu player
        Vector3 rotation = Vector3.up;
        //przemnóż przez czas
        rotation *= Time.deltaTime;
        //przemnóż przez klawiaturę
        rotation *= Input.GetAxis("Horizontal");
        //pomnóż przez prędkość obrotu
        rotation *= rotationSpeed;
        //dodaj obrót do obiektu
        //nie możemy użyć += ponieważ unity używa Quaternionów do zapisu rotacji
        transform.Rotate(rotation);

    }

    private void OnCollisionEnter(Collision collision)
    {
        //uruchamia się automatycznie jeśli zetkniemy sie z innym coliderem

        //sprawdz czy dotknęliśmy asteroidy
        if(collision.collider.transform.CompareTag("Asteroid"))
        {
            Debug.Log("Boom!");
            //pauza
            Time.timeScale = 0;
        }
    }
}
