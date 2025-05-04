using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button menuButton;
    public Button quitButton;
    public Slider volumeSlider;
    public Toggle muteToggle;

    [Header("Audio")]
    public AudioMixer gameAudioMixer;
    public AudioClip buttonSound;

    private AudioSource audioSource;
    private bool isPaused = false;
    private float previousTimeScale;

    void Start()
    {
        // Configuração inicial
        pauseMenuPanel.SetActive(false);
        
        // Configura o AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // Configura os listeners dos botões
        resumeButton.onClick.AddListener(ResumeGame);
        menuButton.onClick.AddListener(ReturnToMenu);
        quitButton.onClick.AddListener(QuitGame);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
        
        // Carrega as preferências salvas
        LoadPlayerPrefs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f; // Pausa o jogo
        
        pauseMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        PlayButtonSound();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = previousTimeScale; // Restaura o tempo
        
        pauseMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        PlayButtonSound();
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Garante que o tempo volte ao normal
        SceneManager.LoadScene("Menu"); // Substitua pelo nome da sua cena de menu
        PlayButtonSound();
    }

    public void QuitGame()
    {
        PlayButtonSound();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void SetVolume(float volume)
    {
        // Converte o valor linear (0-1) para logarítmico (dB)
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        gameAudioMixer.SetFloat("MasterVolume", dB);
        
        // Salva a preferência
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void ToggleMute(bool isMuted)
    {
        gameAudioMixer.SetFloat("MasterVolume", isMuted ? -80f : 0f);
        volumeSlider.interactable = !isMuted;
        
        // Salva o estado do mute
        PlayerPrefs.SetInt("GameMuted", isMuted ? 1 : 0);
    }

    private void LoadPlayerPrefs()
    {
        // Volume
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 0.7f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
        
        // Mute
        bool savedMute = PlayerPrefs.GetInt("GameMuted", 0) == 1;
        muteToggle.isOn = savedMute;
        ToggleMute(savedMute);
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }
}