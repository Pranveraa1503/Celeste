using UnityEngine;

namespace SoftLight.Core.App
{
    /// <summary>
    /// Root object of the application.
    /// Responsible for bootstrapping all game services.
    /// </summary>
    public sealed class App : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}