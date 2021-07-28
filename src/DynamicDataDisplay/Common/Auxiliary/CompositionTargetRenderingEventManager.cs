using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.Common.Auxiliary
{
    /// <summary>
    /// WeakEventManager for the System.Windows.Media.CompositionTarget.Rendering event, which is a static event
    /// </summary>
    public class CompositionTargetRenderingEventManager : WeakEventManager
    {
        public static void AddHandler(EventHandler handler) => 
            CurrentManager.ProtectedAddHandler(null, handler);

        public static void RemoveHandler(EventHandler handler) => 
            CurrentManager.ProtectedRemoveHandler(null, handler);

        public static void AddListener(IWeakEventListener listener) => 
            CurrentManager.ProtectedAddListener(null, listener);

        public static void RemoveListener(IWeakEventListener listener) => 
            CurrentManager.ProtectedRemoveListener(null, listener);

        protected sealed override void StartListening(object source) =>
            CompositionTarget.Rendering += this.OnCompositionTargetRendering;

        protected sealed override void StopListening(object source) => 
            CompositionTarget.Rendering -= this.OnCompositionTargetRendering;

        private void OnCompositionTargetRendering(object sender, EventArgs e) => 
            this.DeliverEvent(null, e);

        private static CompositionTargetRenderingEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(CompositionTargetRenderingEventManager);
                if (!(GetCurrentManager(managerType) is CompositionTargetRenderingEventManager manager))
                {
                    manager = new CompositionTargetRenderingEventManager();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }
    }
}
