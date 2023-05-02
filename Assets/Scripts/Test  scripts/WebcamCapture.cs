using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebcamCapture : MonoBehaviour
{
    public RawImage rawImage; // посилання на RawImage компонент, в який буде виведено зображення з веб-камери

    private WebCamTexture webcamTexture; // текстура веб-камери

    void Start()
    {
        // Отримання доступних веб-камер
        WebCamDevice[] devices = WebCamTexture.devices;
        
        if (devices.Length > 0)
        {
            // Встановлення веб-камери за замовчуванням як джерела текстури
            webcamTexture = new WebCamTexture(devices[0].name);
            rawImage.texture = webcamTexture;

            // Запуск веб-камери
            webcamTexture.Play();
        }
        else
        {
            Debug.Log("Веб-камера не знайдена.");
        }
    }

    void Update()
    {
        // Переміщення отриманого зображення в текстуру
        if (webcamTexture.isPlaying)
        {
            // Оновлення текстури зображення з веб-камери
            //webcamTexture.Update();

            // Відображення зображення в RawImage компоненті
            rawImage.texture = webcamTexture;
        }
    }

    void OnDestroy()
    {
        // Зупинка веб-камери при виході з додатку
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}