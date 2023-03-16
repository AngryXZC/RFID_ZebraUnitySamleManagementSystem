using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour,IPointerClickHandler
{
    [HideInInspector]
    private System.Diagnostics.Process osk;
    public void OnPointerClick(PointerEventData eventData)
    {
        this.gameObject.GetComponent<InputField>().keyboardType = TouchScreenKeyboardType.DecimalPad;
        OpenCloseKeyboard(true);
    }

    /// <summary>
    ///目的：打开或者关闭 window软键盘
    /// </summary>
    /// <param name="_bIsOpen">是否打开软键盘</param>
    public void OpenCloseKeyboard(bool _bIsOpen)
    {
        if (_bIsOpen)
        {
            if (osk == null)
            {
                osk = System.Diagnostics.Process.Start("C:\\Windows\\System32\\osk.exe");
            }
            else
            {
                if (osk.HasExited)
                {
                    osk = System.Diagnostics.Process.Start("C:\\Windows\\System32\\osk.exe");
                }
            }
        }
        else
        {
            if (osk == null)
            {
            }
            else
            {
                if (!osk.HasExited)
                {
                    osk.Kill();
                    osk = null;
                }
            }
        }


    }
}
