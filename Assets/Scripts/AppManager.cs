namespace Oatsbarley.GameJams.LD52
{
    using System.Collections.Generic;
    using Oatsbarley.GameJams.LD52.UI;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public enum AppStage
    {
        Startup,
        MainMenu,
        Tutorial,
        Gameplay,
        EndGame
    }
    
    // controls the flow of the overall application
    
    public class AppManager : MonoBehaviour
    {
        [SerializeField] private bool debugMode = true;
        [SerializeField] private MainMenuUI mainMenuUI;
        [SerializeField] private TutorialUI tutorialUI;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EndGameUI endGameUI;

        public AppStage CurrentStage { get; private set; } = AppStage.Startup;
        public bool HasShownTutorial { get; private set; } = false;
        
        public bool DebugMode => debugMode;

        private static AppManager instance;
        public static AppManager Instance
        {
            get
            {
                if (AppManager.instance == null)
                {
                    AppManager.instance = Object.FindObjectOfType<AppManager>();
                }

                return AppManager.instance;
            }
        }

        private void Awake()
        {
            tutorialUI.Hiding += OnTutorialHiding;
            endGameUI.Hiding += OnEndGameHiding;
        }

        private void Start()
        {
            if (DebugMode)
            {
                BeginGameplay();
                return;
            }
            
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            // todo check if we need to clear gameplay?
            
            mainMenuUI.Show();
            CurrentStage = AppStage.MainMenu;
        }
        
        public void BeginGameplay() 
        {
            if (!DebugMode && !HasShownTutorial)
            {
                ShowTutorial();
                return;
            }
            
            mainMenuUI.Hide();

            gameManager.RunLevel();
            CurrentStage = AppStage.Gameplay;
        }

        public void ShowTutorial()
        {
            if (CurrentStage != AppStage.MainMenu)
            {
                Debug.LogError("Can't show tutorial while not on main menu.");
                return;
            }
            
            mainMenuUI.Hide();
            
            tutorialUI.Show();
            CurrentStage = AppStage.Tutorial;
        }

        private void OnTutorialHiding()
        {
            if (!HasShownTutorial)
            {
                HasShownTutorial = true;
                BeginGameplay();
                return;
            }
            
            ShowMainMenu();
        }

        public void ShowEndGame(Dictionary<string, int> stats)
        {
            if (CurrentStage != AppStage.Gameplay)
            {
                Debug.LogError("Can't show end-game screen while not in gameplay.");
                return;
            }
            
            endGameUI.Show(stats);
            CurrentStage = AppStage.EndGame;
        }

        private void OnEndGameHiding()
        {
            ShowMainMenu();
        }
    }
}