using UnityEngine;

namespace UI
{
    public class ButtonInput : MonoBehaviour
    {
        public void Charge(float input)
        {
            GameManager.Instance.turnManager.TurnTrigger(TurnManager.TurnAction.CHARGE, input);
        }
    }
}