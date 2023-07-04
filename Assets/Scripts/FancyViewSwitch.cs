using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FancyViewSwitch : MonoBehaviour
{
   [SerializeField] private GridLayoutGroup gridLayout;

   [SerializeField] private RectTransform[] roles;
   private static int _offset=30;
   private static int _secondOffset=36;
    private int[] heights=new []{-_offset,0,_offset,_offset*2,_offset*3,_offset*3-_secondOffset,_offset*2-_secondOffset,_offset*1-_secondOffset};
    private bool _inFancyView;

    public void SwitchView()
    {
       if (_inFancyView)
       {
          _inFancyView = false;
          ChangeToClassicView();
       }
       else
       {
          _inFancyView = true;
          ChangeToFancyView();
       }
    }
    private void ChangeToFancyView()
   {
      gridLayout.enabled = false;

      for (int i = 0; i < roles.Length; i++)
      {
         roles[i].anchoredPosition =
            new Vector2(roles[i].anchoredPosition.x, roles[i].anchoredPosition.y + heights[i / 2]);
      }
      
   }

    private void ChangeToClassicView()
   {
      gridLayout.enabled = true;

   }
}
