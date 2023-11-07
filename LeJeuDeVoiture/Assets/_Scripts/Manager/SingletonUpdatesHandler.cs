using UnityEngine;

namespace Helper {
    public class SingletonUpdatesHandler<T> : UpdatesHandler where T : MonoBehaviour{
        private static T instance = null;
        public static T Instance => instance;

        private void Awake() {
            if (instance != null) return;

            instance = this as T;
            AwakeContinue();
        }

        /// <summary>
        /// Method which is called inside the awake method
        /// </summary>
        protected virtual void AwakeContinue() {}
    }
}
