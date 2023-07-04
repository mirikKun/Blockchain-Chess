using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;
using UnityEngine.UI;

public class ArmyController : MonoBehaviour
{
    [SerializeField] private AllCharacters[] allCharacters;

    [Serializable]
    public class AllCharacters
    {
        public Character[] allAvailableCharacters;
        public Character[] charactersToBuy;
    }

    [SerializeField] private List<CharacterDisplay> freeCharactersDisplay;
    [SerializeField] private List<CharacterDisplay> charactersToBuyDisplay;
    [SerializeField] private RectTransform freeCharactersHolder;
    [SerializeField] private RectTransform charactersToBuyHolder;
    [SerializeField] private CharacterDisplay characterDisplayPrefab;
    [SerializeField] private CharacterDisplay characterToBuyDisplayPrefab;
    [SerializeField] private CharacterDropHolder allCharactersHolder;
    private Character[] _freeCharacters;
    [SerializeField] private ArmyRoleHolder[] curCharacters;
    private CharacterDragHandler _lastCharacter;
    private CharacterDropHolder _lastCharacterDropHolder;
    private bool _rolesWereSelected;
    [SerializeField] private MainMenuPanelSwitcher menuPanelSwitcher;
    [SerializeField] private GameObject notCompletedPopup;

    [SerializeField] private GameObject areYouWantToSaveChanges;
    [SerializeField] private GameObject areYouSureToBuyPopup;
    [SerializeField] private GameObject armySavedPopup;
    [SerializeField] private Accounts accounts;
    [SerializeField] private CharacterPanel characterPanel;
    [SerializeField] private CharacterShower characterShower;
    private CharacterDisplay _chosenCharacterToBuy;


    private Army _army;

    private void Awake()
    {
    }

    private void Start()
    {
        // foreach (var character in allCharacters[PlayerData.ColorIndex].allAvailableCharacters)
        // {
        //     CharacterDisplay newCharacterDisplay = Instantiate(characterDisplayPrefab, freeCharactersHolder);
        //     newCharacterDisplay.SetupCharacterDisplay(character);
        //     freeCharactersDisplay.Add(newCharacterDisplay);
        //     newCharacterDisplay.onGoToTheList += AddCharacterDisplay;
        //     newCharacterDisplay.onGoOutOfTheList += RemoveCharacterDisplay;
        //     CharacterDragHandler characterDragHandler = newCharacterDisplay.GetComponent<CharacterDragHandler>();
        //     characterDragHandler.GetButton().onClick.AddListener(delegate
        //     {
        //         ReactOnCharacterClick(characterDragHandler);
        //     });
        // }
        //
        // foreach (var character in allCharacters[PlayerData.ColorIndex].charactersToBuy)
        // {
        //     CharacterDisplay newCharacterDisplay = Instantiate(characterToBuyDisplayPrefab, charactersToBuyHolder);
        //     newCharacterDisplay.SetupCharacterDisplay(character);
        //     charactersToBuyDisplay.Add(newCharacterDisplay);
        //     newCharacterDisplay.GetComponent<Button>().onClick.AddListener(delegate
        //     {
        //         ChooseCharacterToBuy(newCharacterDisplay);
        //     });
        // }

        foreach (var curCharacter in curCharacters)
        {
            CharacterDropHolder characterDropHolder = curCharacter.GetComponent<CharacterDropHolder>();
            characterDropHolder.OnMoveToHolder += UnselectAllRoles;
            characterDropHolder.GetButton().onClick.AddListener(delegate
            {
                ReactOnRoleClick(characterDropHolder);
                characterDropHolder.OnDropEnd += ForgotLastCharacter;
            });
            // for (int j = 0; j < freeCharactersDisplay.Count; j++)
            // {
            //     CharacterDragHandler curCharacterDragHandler =
            //         freeCharactersDisplay[j].GetComponent<CharacterDragHandler>();
            //
            //     if (characterDropHolder.GetRole() ==
            //         (int) curCharacterDragHandler.GetCharacterDisplay().character.characterRole)
            //     {
            //         characterDropHolder.MoveCharacterToTheHolder(curCharacterDragHandler);
            //         break;
            //     }
            // }
        }

        allCharactersHolder.GetButton().onClick.AddListener(delegate
        {
            ReactOnRoleClick(allCharactersHolder);
            allCharactersHolder.OnDropEnd += ForgotLastCharacter;
        });
    }

    private void OnDisable()
    {
        foreach (var freeCharacterDisplay in freeCharactersDisplay)
        {
            freeCharacterDisplay.onGoToTheList = null;
            freeCharacterDisplay.onGoOutOfTheList = null;
            Destroy(freeCharacterDisplay.gameObject);
        }

        freeCharactersDisplay?.Clear();
        foreach (var characterToBuyDisplay in charactersToBuyDisplay)
        {
            Destroy(characterToBuyDisplay.gameObject);
        }

        charactersToBuyDisplay?.Clear();

        foreach (var curCharacter in curCharacters)
        {
            CharacterDropHolder characterDropHolder = curCharacter.GetComponent<CharacterDropHolder>();
            if (characterDropHolder.GetComponentInChildren<CharacterDragHandler>())
            {
                Destroy(characterDropHolder.GetComponentInChildren<CharacterDragHandler>().gameObject);
            }
        }
    }

    private void OnEnable()
    {
        if (freeCharactersDisplay != null)

            if (PlayerData.Account == null)
            {
                foreach (var character in allCharacters[PlayerData.ColorIndex].allAvailableCharacters)
                {
                    SetupCharacterButton(character);
                }
            }
            else if (PlayerData.Account.AvailableCharacters == null)
            {
                PlayerData.Account.AvailableCharacters = new List<Character>();
                PlayerData.Account.AvailableCharacters.AddRange(allCharacters[PlayerData.ColorIndex]
                    .allAvailableCharacters.ToList());
                foreach (var character in allCharacters[PlayerData.ColorIndex].allAvailableCharacters)
                {
                    SetupCharacterButton(character);
                }
            }
            else
            {
                foreach (var character in PlayerData.Account.AvailableCharacters)
                {
                    SetupCharacterButton(character);
                }
            }


        foreach (var character in allCharacters[PlayerData.ColorIndex].charactersToBuy)
        {
            if (PlayerData.Account != null && PlayerData.Account.AvailableCharacters.Contains(character))
            {
                continue;
            }

            CharacterDisplay newCharacterDisplay = Instantiate(characterToBuyDisplayPrefab, charactersToBuyHolder);
            newCharacterDisplay.SetupCharacterDisplay(character);
            charactersToBuyDisplay.Add(newCharacterDisplay);
            newCharacterDisplay.GetComponent<Button>().onClick.AddListener(delegate
            {
                ChooseCharacterToBuy(newCharacterDisplay);
            });
        }

        int i = -1;

        foreach (var curCharacter in curCharacters)
        {
            CharacterDropHolder characterDropHolder = curCharacter.GetComponent<CharacterDropHolder>();


            if (PlayerData.Army != null)
            {
                i++;

                for (int j = 0; j < freeCharactersDisplay.Count; j++)
                {
                    CharacterDragHandler curCharacterDragHandler =
                        freeCharactersDisplay[j].GetComponent<CharacterDragHandler>();
                    if (PlayerData.Army._characters[i].index ==
                        curCharacterDragHandler.GetCharacterDisplay().character.index)
                    {
                        characterDropHolder.MoveCharacterToTheHolder(curCharacterDragHandler);
                        break;
                    }
                }
            }
            else
            {
                for (int j = 0; j < freeCharactersDisplay.Count; j++)
                {
                    CharacterDragHandler curCharacterDragHandler =
                        freeCharactersDisplay[j].GetComponent<CharacterDragHandler>();

                    if (characterDropHolder.GetRole() ==
                        (int) curCharacterDragHandler.GetCharacterDisplay().character.characterRole)
                    {
                        characterDropHolder.MoveCharacterToTheHolder(curCharacterDragHandler);
                        break;
                    }
                }
            }
        }
    }

    public void SetupCharacterButton(Character character)
    {
        CharacterDisplay newCharacterDisplay = Instantiate(characterDisplayPrefab, freeCharactersHolder);
        newCharacterDisplay.SetupCharacterDisplay(character);
        freeCharactersDisplay.Add(newCharacterDisplay);
        newCharacterDisplay.onGoToTheList += AddCharacterDisplay;
        newCharacterDisplay.onGoOutOfTheList += RemoveCharacterDisplay;
        CharacterDragHandler characterDragHandler = newCharacterDisplay.GetComponent<CharacterDragHandler>();
        characterDragHandler.GetButton().onClick.AddListener(delegate { ReactOnCharacterClick(characterDragHandler); });
    }

    public Character[] GetAllCharacters(int colorIndex)
    {
        return allCharacters[colorIndex].allAvailableCharacters.Concat(allCharacters[colorIndex].charactersToBuy).ToArray();
    }
    public void ReactOnCharacterClick(CharacterDragHandler thisCharacter)
    {
        thisCharacter.ChangeLastParent(thisCharacter.transform.parent);

        UnselectAllRoles();

        if (_lastCharacter)
        {
            _lastCharacter.SetHighlightActive(false);
            if (_lastCharacter == thisCharacter)
            {
                if (thisCharacter.GetCharacterDisplay().WasInList)
                {
                    foreach (var curCharacter in curCharacters)
                    {
                        CharacterDropHolder newCurCharacter = curCharacter.GetComponent<CharacterDropHolder>();

                        if (!newCurCharacter.GetComponentInChildren<CharacterDragHandler>() &&
                            newCurCharacter.GetRole() ==
                            (int) thisCharacter.GetCharacterDisplay().character.characterRole)
                        {
                            newCurCharacter.MoveCharacterToTheHolder(thisCharacter);
                            _lastCharacter = null;
                            _rolesWereSelected = false;
                            return;
                        }
                    }
                }
                else
                {
                    _lastCharacter = null;
                    _rolesWereSelected = true;
                    thisCharacter.SetHighlightActive(false);
                    UnselectAllRoles();
                    return;
                }
            }
            else if (thisCharacter.GetCharacterDisplay().WasInList && _lastCharacter.GetCharacterDisplay().WasInList)
            {
                _lastCharacter = null;
                _rolesWereSelected = false;
            }
            else
            {
                if (!thisCharacter.GetCharacterDisplay().WasInList)
                {
                    thisCharacter.GetComponentInParent<CharacterDropHolder>().MoveCharacterToTheHolder(_lastCharacter);
                    _lastCharacter = null;
                    _rolesWereSelected = false;
                    return;
                }
            }
        }

        _lastCharacter = thisCharacter;
        _lastCharacter.SetHighlightActive(true);
        characterPanel.ChooseCharacter(thisCharacter.GetCharacterDisplay().character, PlayerType.Player);
        characterShower.ShowCurFigure(thisCharacter.GetCharacterDisplay().character);
        _rolesWereSelected = true;
        foreach (var curCharacter in curCharacters)
        {
            CharacterDropHolder newCurCharacter = curCharacter.GetComponent<CharacterDropHolder>();
            if (newCurCharacter.GetRole() == (int) thisCharacter.GetCharacterDisplay().character.characterRole)
            {
                newCurCharacter.ChangeSprite(true);
            }
        }
    }

    public void UnselectAllRoles()
    {
        if (_rolesWereSelected)
        {
            foreach (var curCharacter in curCharacters)
            {
                CharacterDropHolder newCurCharacter = curCharacter.GetComponent<CharacterDropHolder>();
                newCurCharacter.ChangeSprite(false);
            }
        }
    }

    public void ForgotLastCharacter()
    {
        _lastCharacter = null;
        _lastCharacterDropHolder = null;
    }

    public void ReactOnRoleClick(CharacterDropHolder thisCharacterDropHolder)
    {
        if (_lastCharacterDropHolder)
        {
            _lastCharacterDropHolder.ChangeSprite(false);
            ApplyFreeCharactersFilter(0, 0, 0);
            if (_lastCharacterDropHolder == thisCharacterDropHolder)
            {
                _lastCharacterDropHolder = null;
                if (!thisCharacterDropHolder.GetComponentInChildren<CharacterDragHandler>())
                {
                    foreach (var freeCharacterDisplay in freeCharactersDisplay)
                    {
                        if (thisCharacterDropHolder.GetRole() ==
                            (int) freeCharacterDisplay.character.characterRole)
                        {
                            CharacterDragHandler newCharacterDragHandler =
                                freeCharacterDisplay.GetComponent<CharacterDragHandler>();
                            thisCharacterDropHolder.MoveCharacterToTheHolder(newCharacterDragHandler);
                            return;
                        }
                    }
                }
            }
            else
            {
                _lastCharacterDropHolder = thisCharacterDropHolder;
            }
        }

        if (_lastCharacter)
        {
            if (!_lastCharacter.GetCharacterDisplay().WasInList || thisCharacterDropHolder.squareView)
            {
                thisCharacterDropHolder.MoveCharacterToTheHolder(_lastCharacter);
            }

            ApplyFreeCharactersFilter(0, 0, 0);
            UnselectAllRoles();
            _lastCharacter.SetHighlightActive(false);
            _lastCharacter = null;
            _lastCharacterDropHolder = null;
        }
        else
        {
            if (thisCharacterDropHolder.squareView)
            {
                if (_lastCharacterDropHolder == null)
                {
                    ApplyFreeCharactersFilter(thisCharacterDropHolder.GetRole() + 1, 0, 0);
                    _lastCharacterDropHolder = thisCharacterDropHolder;
                    thisCharacterDropHolder.ChangeSprite(true);
                }
                else
                {
                    _lastCharacterDropHolder.ChangeSprite(false);
                    ApplyFreeCharactersFilter(0, 0, 0);
                    _lastCharacterDropHolder = null;
                }
            }
        }
    }


    private void AddCharacterDisplay(CharacterDisplay characterDisplay)
    {
        if (characterDisplay && !freeCharactersDisplay.Contains(characterDisplay))
        {
            freeCharactersDisplay.Add(characterDisplay);
        }
    }

    private void RemoveCharacterDisplay(CharacterDisplay characterDisplay)
    {
        if (freeCharactersDisplay.Contains(characterDisplay))
        {
            freeCharactersDisplay.Remove(characterDisplay);
        }
    }

    public void ClearFilters()
    {
        ApplyFreeCharactersFilter(0, 0, 0);
        ApplyCharactersToBuyFilter(0, 0, 0);
        UnselectAllRoles();
        if (_lastCharacter)
            _lastCharacter.SetHighlightActive(false);
        if (_lastCharacterDropHolder)
            _lastCharacterDropHolder.ChangeSprite(false);

        _lastCharacter = null;
        _lastCharacterDropHolder = null;
    }

    public void ApplyFreeCharactersFilter(int roleIndex, int classIndex, int weaponIndex, int minLevel = 1,
        int maxLevel = 100)
    {
        ApplyFilter(roleIndex, classIndex, weaponIndex, freeCharactersDisplay, minLevel, maxLevel);
    }

    public void ApplyCharactersToBuyFilter(int roleIndex, int classIndex, int weaponIndex, int minLevel = 1,
        int maxLevel = 100)
    {
        ApplyFilter(roleIndex, classIndex, weaponIndex, charactersToBuyDisplay, minLevel, maxLevel);
    }

    public void ApplyFilter(int roleIndex, int classIndex, int weaponIndex, List<CharacterDisplay> characterDisplays,
        int minLevel, int maxLevel)
    {
        foreach (var freeCharacterDisplay in characterDisplays)
        {
            if (roleIndex != 0)
            {
                if (freeCharacterDisplay.GetRole() != (Role) (roleIndex - 1))
                {
                    freeCharacterDisplay.gameObject.SetActive(false);
                    continue;
                }
            }

            if (classIndex != 0)
            {
                if (freeCharacterDisplay.GetClass() != (Class) (classIndex - 1))
                {
                    freeCharacterDisplay.gameObject.SetActive(false);
                    continue;
                }
            }

            if (weaponIndex != 0)
            {
                if (freeCharacterDisplay.GetWeapon() != (Weapon) (weaponIndex - 1))
                {
                    freeCharacterDisplay.gameObject.SetActive(false);
                    continue;
                }
            }
            
            if (!(minLevel <= freeCharacterDisplay.character.characterProgress.Level 
                  && freeCharacterDisplay.character.characterProgress.Level <= maxLevel))
            {
                freeCharacterDisplay.gameObject.SetActive(false);
                continue;
            }

            freeCharacterDisplay.gameObject.SetActive(true);
        }
    }

    public void SaveArmy()
    {
        bool armySaved = CheckExitSaveArmy();
        if (armySaved)
        {
            armySavedPopup.SetActive(true);
        }
    }

    /// <summary>
    /// Back all scriptable objects to default numbers when account creating
    /// </summary>
    public void ShowArmySaved()
    {
        foreach (var curCharacter in allCharacters[PlayerData.ColorIndex].allAvailableCharacters)
        {
            curCharacter.characterProgress=new CharacterProgress();
            curCharacter.SetupPrimaryStances();

        }

        foreach (var curCharacter in allCharacters[PlayerData.ColorIndex].charactersToBuy)
        {
            curCharacter.characterProgress=new CharacterProgress();
            curCharacter.SetupPrimaryStances();

        }
    }
    
    public bool CheckExitSaveArmy()
    {
        List<Character> newCharacters = new List<Character>();
        foreach (var curCharacter in curCharacters)
        {
            if (curCharacter.GetCharacter())
            {
                newCharacters.Add(curCharacter.GetCharacter());
            }
            else
            {
                notCompletedPopup.SetActive(true);
                return false;
            }

        }

        _army = new Army();
        _army.SetupArmy(newCharacters.ToArray());
        PlayerData.SaveArmy(_army);
        accounts.RemoveArmyData(PlayerData.Account.AccountIndex);
        accounts.SaveArmy(PlayerData.Account);


        accounts.RemoveAvailableCharactersData(PlayerData.Account.AccountIndex);
        accounts.SaveAvailableCharacters(PlayerData.Account);
        return true;
    }

    public void SaveAndExit()
    {
        areYouWantToSaveChanges.SetActive(false);
        if (CheckExitSaveArmy())
        {
            menuPanelSwitcher.BackToMainScreen();
        }
    }

    public void ChooseCharacterToBuy(CharacterDisplay character)
    {
        areYouSureToBuyPopup.SetActive(true);
        _chosenCharacterToBuy = character;
    }

    public void ConfirmCharacterBuying()
    {
        areYouSureToBuyPopup.SetActive(false);

        CharacterDisplay newCharacterDisplay = Instantiate(characterDisplayPrefab, freeCharactersHolder);
        newCharacterDisplay.SetupCharacterDisplay(_chosenCharacterToBuy.GetCharacter());
        PlayerData.AddNewCharacter(_chosenCharacterToBuy.GetCharacter());

        charactersToBuyDisplay.Remove(_chosenCharacterToBuy);
        Destroy(_chosenCharacterToBuy.gameObject);
        freeCharactersDisplay.Add(newCharacterDisplay);
        newCharacterDisplay.onGoToTheList += AddCharacterDisplay;
        newCharacterDisplay.onGoOutOfTheList += RemoveCharacterDisplay;
        CharacterDragHandler characterDragHandler = newCharacterDisplay.GetComponent<CharacterDragHandler>();
        characterDragHandler.GetButton().onClick.AddListener(delegate { ReactOnCharacterClick(characterDragHandler); });


        accounts.RemoveAvailableCharactersData(PlayerData.Account.AccountIndex);
        accounts.SaveAvailableCharacters(PlayerData.Account);
    }
}