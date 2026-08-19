using UnityEngine;

public enum ColorMode
{
    Normal = 0,
    Protanopia = 1,
    Deuteranopia = 2,
    Tritanopia = 3
}

public class ColorAccessibilityManager : MonoBehaviour
{
    public static ColorAccessibilityManager Instance;

    [Header("Material do filtro de daltonismo")]
    public Material colorBlindnessMaterial;

    
    private const string ShaderProperty = "_ColorMode";

    public ColorMode CurrentMode { get; private set; }

    private const string SaveKey = "ColorAccessibilityMode";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public void SetColorMode(ColorMode mode)
    {
        CurrentMode = mode;

        
        PlayerPrefs.SetInt(SaveKey, (int)mode);
        PlayerPrefs.Save();

        ApplyColorMode();

        Debug.Log("Modo de cores alterado para: " + CurrentMode);
    }

    private void LoadSettings()
    {
        int savedMode = PlayerPrefs.GetInt(SaveKey, 0);

        CurrentMode = (ColorMode)savedMode;

        ApplyColorMode();

        Debug.Log("Modo de cores carregado: " + CurrentMode);
    }

    private void ApplyColorMode()
    {
        if (colorBlindnessMaterial != null)
        {
            colorBlindnessMaterial.SetFloat(
                ShaderProperty,
                (float)CurrentMode
            );
        }
        else
        {
            Debug.LogWarning(
                "ColorBlindnessMaterial não foi colocado no ColorAccessibilityManager."
            );
        }
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetColorMode(ColorMode.Normal);
        }

        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetColorMode(ColorMode.Protanopia);
        }

       
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetColorMode(ColorMode.Deuteranopia);
        }

       
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetColorMode(ColorMode.Tritanopia);
        }
    }
}