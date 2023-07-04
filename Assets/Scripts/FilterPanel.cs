using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanel : MonoBehaviour
{
   private ArmyController _armyController;
   [SerializeField] private TMP_Dropdown roleDropdown;
   [SerializeField] private TMP_Dropdown classDropdown;
   [SerializeField] private TMP_Dropdown weaponDropdown;
   [SerializeField] private RangeSlider rangeSlider;

   private void Start()
   {
      _armyController = FindObjectOfType<ArmyController>();
      List<string> roleNames=new List<string>();
      foreach (Role role in Enum.GetValues(typeof(Role)))
      {
         roleNames.Add(role.ToString().ToUpper());
      }
      roleDropdown.AddOptions(roleNames);      
      
      List<string> classNames=new List<string>();
      foreach (Class newClass in Enum.GetValues(typeof(Class)))
      {
         classNames.Add(ClassNameParser.GetDescriptionFromEnum(newClass).ToUpper());
      }
      classDropdown.AddOptions(classNames);      
      
      List<string> weaponNames=new List<string>();
      foreach (Weapon weapon in Enum.GetValues(typeof(Weapon)))
      {
         weaponNames.Add(weapon.ToString().ToUpper());
      }
      weaponDropdown.AddOptions(weaponNames);
   }

   public void ClearAll()
   {
      roleDropdown.value = 0;
      classDropdown.value = 0;
      weaponDropdown.value = 0;
      rangeSlider.LowValue=rangeSlider.MinValue;
      rangeSlider.HighValue=rangeSlider.MaxValue;
   }
   public void ApplyFilter()
   {
      _armyController.ApplyFreeCharactersFilter(roleDropdown.value,classDropdown.value,weaponDropdown.value,(int)rangeSlider.LowValue,(int)rangeSlider.HighValue);
   }   
   public void ApplyBuyFilter()
   {
      _armyController.ApplyCharactersToBuyFilter(roleDropdown.value,classDropdown.value,weaponDropdown.value,(int)rangeSlider.LowValue,(int)rangeSlider.HighValue);
   }
}
