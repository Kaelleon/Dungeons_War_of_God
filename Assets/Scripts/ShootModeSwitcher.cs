using UnityEngine;
using UnityEngine.InputSystem;

public class ShootModeSwitcher : MonoBehaviour
{
    [Header("Modos (arrastra los scripts del jugador)")]
    [SerializeField] private ShootOnClick clickMode;      // tecla 1
    [SerializeField] private ShootAimMouse aimMode;       // tecla 2
    [SerializeField] private ShootProximity proximityMode; // tecla 3

    [SerializeField] private int startMode = 1;

    private ShooterBase[] modes;
    private int currentMode;

    private void Awake()
    {
        modes = new ShooterBase[] { clickMode, aimMode, proximityMode };
    }

    private void Start()
    {
        SetMode(startMode);
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
            SetMode(1);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
            SetMode(2);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
            SetMode(3);
    }

    private void SetMode(int mode)
    {
        currentMode = mode;

        for (int i = 0; i < modes.Length; i++)
        {
            if (modes[i] != null)
                modes[i].enabled = (i == mode - 1);
        }

        Debug.Log("Modo de disparo: " + mode);
    }
}