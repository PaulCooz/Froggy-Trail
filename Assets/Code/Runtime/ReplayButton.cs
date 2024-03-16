using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public sealed class ReplayButton : MonoBehaviour, IStartListener
    {
        [SerializeField]
        private GameLoop gameLoop;
        [SerializeField]
        private Button button;

        public void GameStart(LevelState state)
        {
            gameLoop.OnGameOver += Show;
        }

        public void Show()
        {
            button.gameObject.SetActive(true);
        }

        public void Restart()
        {
            button.gameObject.SetActive(false);
            gameLoop.Restart();
        }
    }
}