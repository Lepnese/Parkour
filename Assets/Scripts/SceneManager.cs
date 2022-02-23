using UnityEngine;
using UnityEngine.SceneManagement;

public enum �tatsSc�ne { Sc�neMenu, Sc�neJeu, Sc�neContreLaMontre, Nb�tatsSc�ne }

public class GameMasterFSM : MonoBehaviour
{
    readonly string[] NomsSc�nes = { "Sc�neMenu", "Sc�neJeu", "Sc�neContreLaMontre" };

    public string MessageFinal { get; private set; }
    private static bool IsCreated;
    private static float D�laiTouche { get; set; }

    private static �tatsSc�ne SceneState;

    private static �tatsSc�ne nextSceneState;
    public static �tatsSc�ne NextSceneState
    {
        get { return nextSceneState; }
        set
        {
            if (value >= �tatsSc�ne.Sc�neMenu)
                nextSceneState = value;
            else
                nextSceneState = �tatsSc�ne.Sc�neMenu;
        }
    }

    static GameMasterFSM()
    {
        IsCreated = false;
        D�laiTouche = 0;
        SceneState = �tatsSc�ne.Sc�neMenu;
    }

    void Awake()
    {
        if (!IsCreated)
        {
            DontDestroyOnLoad(this.gameObject);
            IsCreated = true;
        }
    }


    void Update()
    {
        //D�laiTouche += Time.deltaTime;
        //if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && D�laiTouche > 0.5f)
        //{
        //   if (Input.GetKey(KeyCode.RightArrow))
        //   {
        //      D�laiTouche = 0;
        //      ProchaineSc�ne();
        //   }
        //   else
        //   {
        //      if (Input.GetKey(KeyCode.LeftArrow))
        //      {
        //         D�laiTouche = 0;
        //         Sc�nePr�c�dente();
        //      }
        //   }
        //}
        if (SceneState != NextSceneState)
        {
            EffectuerTransition();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Quitter();
        }
    }

    public void Quitter()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

    }

    private void EffectuerTransition()
    {
        SceneManager.LoadScene(NomsSc�nes[(int)NextSceneState], LoadSceneMode.Single);
        SceneState = NextSceneState;
    }

    public void ProchaineSc�ne()
    {
        //NextSceneState = (�tatsSc�ne)(((int)SceneState+1)%(int)�tatsSc�ne.Nb�tatsSc�ne);
    }

    public void Sc�nePr�c�dente()
    {
        NextSceneState = �tatsSc�ne.Sc�neMenu;
    }

    public void Cr�erMessageFinal(float pointage)
    {
        MessageFinal = $"{pointage} points Damn GG!wtf";
    }

}
