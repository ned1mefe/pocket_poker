using UnityEngine;

namespace Screens
{
    public interface IScreen 
    {
        public void HandleShow();
        public void HandleClose();
    }
}