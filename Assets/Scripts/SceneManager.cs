using UnityEngine;
using UnityEngine.SceneManagement;

public enum ÉtatsScène { ScèneMenu, ScèneJeu, ScèneContreLaMontre, NbÉtatsScène }

public class GameMasterFSM : MonoBehaviour
{
    readonly string[] NomsScènes = { "ScèneMenu", "ScèneJeu", "ScèneContreLaMontre" };

    public string MessageFinal { get; private set; }
    private static bool IsCreated;
    private static float DélaiTouche { get; set; }

    private static ÉtatsScène SceneState;

    private static ÉtatsScène nextSceneState;
    public static ÉtatsScène NextSceneState
    {
        get { return nextSceneState; }
        set
        {
            if (value >= ÉtatsScène.ScèneMenu)
                nextSceneState = value;
            else
                nextSceneState = ÉtatsScène.ScèneMenu;
        }
    }

    static GameMasterFSM()
    {
        IsCreated = false;
        DélaiTouche = 0;
        SceneState = ÉtatsScène.ScèneMenu;
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
        //DélaiTouche += Time.deltaTime;
        //if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && DélaiTouche > 0.5f)
        //{
        //   if (Input.GetKey(KeyCode.RightArrow))
        //   {
        //      DélaiTouche = 0;
        //      ProchaineScène();
        //   }
        //   else
        //   {
        //      if (Input.GetKey(KeyCode.LeftArrow))
        //      {
        //         DélaiTouche = 0;
        //         ScènePrécédente();
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
        SceneManager.LoadScene(NomsScènes[(int)NextSceneState], LoadSceneMode.Single);
        SceneState = NextSceneState;
    }

    public void ProchaineScène()
    {
        //NextSceneState = (ÉtatsScène)(((int)SceneState+1)%(int)ÉtatsScène.NbÉtatsScène);
    }

    public void ScènePrécédente()
    {
        NextSceneState = ÉtatsScène.ScèneMenu;
    }

    public void CréerMessageFinal(float pointage)
    {
        MessageFinal = $"{pointage} points Damn GG!wtf";
    }

}
