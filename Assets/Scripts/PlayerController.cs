using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float flySpeed = 5f;
    //odniesienie do menadzera poziomu
    GameObject levelManagerObject;
    //stan osłon w procentach (1=100%)
    float shieldCapacity = 1;
    //płomień silnika
    GameObject engineFlame;
    //odgłos silnika
    GameObject engineSound;
    //wizualna osłona
    GameObject shieldSphere;

    // Start is called before the first frame update
    void Start()
    {
        levelManagerObject = GameObject.Find("LevelManager");
        engineFlame = transform.Find("EngineFlame").gameObject;
        engineSound = transform.Find("EngineSound").gameObject;
        shieldSphere = transform.Find("ShieldSphere").gameObject;
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

        //dostosuj wielkość płomienia silnika do ilości dodanego "gazu", tylko dla dodatnich
        engineFlame.transform.localScale = Vector3.one * Mathf.Max(Input.GetAxis("Vertical"), 0);

        //dostosuj głośność odłosu silnika j.w.
        engineSound.GetComponent<AudioSource>().volume = Mathf.Max(Input.GetAxis("Vertical"), 0);

        //pasywna regeneracja osłon
        if(shieldCapacity < 1)
            shieldCapacity += Time.deltaTime / 100;

        //zaktualizuj interfejs
        UpdateUI();
    }

    private void UpdateUI()
    {
        //metoda wykonuje wszystko związane z aktualizacją interfejsu użytkownika

        //wyciagnij z menadzera poziomu pozycje wyjscia
        Vector3 target = levelManagerObject.GetComponent<LevelManager>().exitPosition;
        //obroc znacznik w strone wyjscia
        transform.Find("NavUI").Find("TargetMarker").LookAt(target);
        //zmien ilosc procentwo widoczna w interfejsie
        //TODO: poprawić wyświetlanie stanu osłon!
        TextMeshProUGUI shieldText = 
            GameObject.Find("Canvas").transform.Find("ShieldCapacityText").GetComponent<TextMeshProUGUI>();
        shieldText.text = " Shield: " + (shieldCapacity*100).ToString("F0") + "%";

        //sprawdzamy czy poziom się zakończył i czy musimy wyświetlić ekran końcowy
        if(levelManagerObject.GetComponent<LevelManager>().levelComplete) 
        {
            //znajdz canvas (interfejs), znajdz w nim ekran konca poziomu i go włącz
            GameObject.Find("Canvas").transform.Find("LevelCompleteScreen").gameObject.SetActive(true);
        }
        //sprawdzamy czy poziom się zakończył i czy musimy wyświetlić ekran końcowy
        if (levelManagerObject.GetComponent<LevelManager>().levelFailed)
        {
            //znajdz canvas (interfejs), znajdz w nim ekran konca poziomu i go włącz
            GameObject.Find("Canvas").transform.Find("GameOverScreen").gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //uruchamia się automatycznie jeśli zetkniemy sie z innym coliderem

        //sprawdz czy dotknęliśmy asteroidy
        if(collision.collider.transform.CompareTag("Asteroid"))
        {
            //transform asteroidy
            Transform asteroid = collision.collider.transform;
            //policz wektor według którego odepchniemy asteroide
            Vector3 shieldForce = asteroid.position - transform.position;
            //popchnij asteroide
            asteroid.GetComponent<Rigidbody>().AddForce(shieldForce * 5, ForceMode.Impulse);
            shieldCapacity -= 0.25f;
            //błyśnij osłonami
            ShieldFlash();
            if(shieldCapacity <= 0)
            {
                //poinformuj level manager, że gra się skończyła bo nie mamy osłon
                levelManagerObject.GetComponent<LevelManager>().OnFailure();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //jeżeli dotkniemy znacnzika końca poziomu to ustaw w levelmanager flagę,
        //że poziom jest ukończony
        if (other.transform.CompareTag("LevelExit"))
        {
            //wywołaj dla LevelManager metodę zakończenia poziomu
            levelManagerObject.GetComponent<LevelManager>().OnSuccess();
        }
    }
    private void ShieldFlash()
    {
        shieldSphere.SetActive(true);
        Invoke("ShieldOff", 1);
    }
    void ShieldOff()
    {
        shieldSphere.SetActive(false);
    }
}
