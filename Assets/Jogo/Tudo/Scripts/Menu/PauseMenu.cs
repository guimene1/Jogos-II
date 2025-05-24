using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

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
    public string backgroundMusicName = "Background";
    public AudioClip buttonSound;

    private AudioSource audioSource;
    private bool isPaused = false;
    private float previousTimeScale;
    private float lastVolumeBeforeMute; // Armazena o volume antes de mutar

    void Start()
    {
        // Configuração inicial
        pauseMenuPanel.SetActive(false);

        // Configura o AudioSource para efeitos de UI
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

        // Atalho para mutar/desmutar com a tecla M
        if (Input.GetKeyDown(KeyCode.M))
        {
            muteToggle.isOn = !muteToggle.isOn;
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

        // Pausa a música de fundo (opcional)
        AudioManager.instance.Stop(backgroundMusicName);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = previousTimeScale; // Restaura o tempo

        pauseMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayButtonSound();

        // Retoma a música de fundo se não estiver mutada
        if (!muteToggle.isOn)
        {
            AudioManager.instance.Play(backgroundMusicName);
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Garante que o tempo volte ao normal
        AudioManager.instance.Stop(backgroundMusicName); // Para a música atual
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
        AudioManager.instance.SetMasterVolume(volume);
        if (!muteToggle.isOn)
            lastVolumeBeforeMute = volume;

        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void ToggleMute(bool isMuted)
    {
        AudioManager.instance.MuteMaster(isMuted);
        volumeSlider.interactable = !isMuted;
        PlayerPrefs.SetInt("GameMuted", isMuted ? 1 : 0);
    }

    private void LoadPlayerPrefs()
    {
        // Volume
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 0.7f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
        lastVolumeBeforeMute = savedVolume; // Inicializa com o volume salvo

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