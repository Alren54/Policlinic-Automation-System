using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonManager : MonoBehaviour
{

    [Header("Panels")]
    private GameObject LoginPanel;
    private GameObject RegisterPanel;
    private GameObject PatientMainScreenPanel;
    private GameObject PatientTakeRendezvousPanel;
    private GameObject PatientExaminationPanel;
    private GameObject DebtsPanel;
    private GameObject SettingsPanel;
    private GameObject ExaminationInfoPanel;
    private GameObject PayingPanel;
    private GameObject CreditCardPanel;

    [Header("Input Fields")]
    private List<GameObject> LoginIFields;
    private List<GameObject> RegisterIFields;

    #region Login Panel Buttons
    public void LP_LoginButton()
    {
        string id = ConvertFromIF(LoginIFields[0]);
        string password = ConvertFromIF(LoginIFields[1]);
        if (NotNull(id) && NotNull(password))
        {

        }
    }

    public void LP_RegisterButton()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }
    #endregion
    #region Register Panel Buttons
    public void RP_RegisterButton()
    {
        string id = ConvertFromIF(RegisterIFields[0]);
        string name = ConvertFromIF(RegisterIFields[1]);
        string birthday = ConvertFromIF(RegisterIFields[2]);
        string insurance = ConvertFromIF(RegisterIFields[3]);
        string password = ConvertFromIF(RegisterIFields[4]);
        if(NotNull(id) && NotNull(name) && NotNull(birthday) && NotNull(insurance) && NotNull(password))
        {

        }
    }
    #endregion
    #region Side Panel Buttons
    public void SidePanel_TakeRendezvousButton()
    {
        CloseAllPanels();
        PatientTakeRendezvousPanel.SetActive(true);
    }

    public void SidePanel_ExaminationPanelButton()
    {
        CloseAllPanels();
        PatientExaminationPanel.SetActive(true);
    }

    public void SidePanel_DebtsButton()
    {
        CloseAllPanels();
        DebtsPanel.SetActive(true);
    }

    public void SidePanel_SettingsButton()
    {
        CloseAllPanels();
        SettingsPanel.SetActive(true);
    }

    public void SidePanel_ExitButton()
    {
        CloseAllPanels();
        LoginPanel.SetActive(true);
    }
    #endregion
    #region Prefab Functions
    public void RendezvousPrefab(int element)
    {
        switch(element)
        {
            case 0:
                break;
        }
    }
    #endregion
    #region General Functions
    bool NotNull(string a)
    {
        if (string.IsNullOrEmpty(a))
            return false;
        return true;
    }

    void CloseAllPanels()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        PatientMainScreenPanel.SetActive(false);
        PatientTakeRendezvousPanel.SetActive(false);
        PatientExaminationPanel.SetActive(false);
        DebtsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ExaminationInfoPanel.SetActive(false);
        PayingPanel.SetActive(false);
        CreditCardPanel.SetActive(false);
    }

    private string ConvertFromIF(GameObject Ifield)
    {
        return Ifield.GetComponent<TMP_InputField>().text.ToString();
    }
    #endregion
}
