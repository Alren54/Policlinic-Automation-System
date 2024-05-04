using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Linq;
using Unity.VisualScripting;
using System.Globalization;

public class ButtonManager : MonoBehaviour
{
    [Header("TempVariables")]
    [SerializeField] string currentUsername;
    [SerializeField] string currentTC;

    [Header("SQL Attributes")]
    [SerializeField] DBManager DBManager;
    [SerializeField] string dateFormat = "yyyy-MM-dd";

    [Header("Panels")]
    [SerializeField] private GameObject LoginPanel;
    [SerializeField] private GameObject RegisterPanel;
    [SerializeField] private GameObject PatientMainScreenPanel;
    [SerializeField] private GameObject PatientTakeRendezvousPanel;
    [SerializeField] private GameObject PatientExaminationPanel;
    [SerializeField] private GameObject DebtsPanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject ExaminationInfoPanel;
    [SerializeField] private GameObject PayingPanel;
    [SerializeField] private GameObject CreditCardPanel;

    [Header("Input Fields")]
    [SerializeField] private List<GameObject> LoginIFields;
    [SerializeField] private List<GameObject> RegisterIFields;
    [SerializeField] private List<GameObject> SettingsIFields;
    [SerializeField] private List<GameObject> CreditCardIFields;

    [Header("Texts")]
    [SerializeField] private List<TextMeshProUGUI> PatientMainScreenPanelTexts;
    // object result = DBManager.dbManager.ExecuteQuery(4, DBManager.dbConnection, "SELECT * FROM user_info WHERE user_name = @param1", Username);

    #region Login Panel Buttons
    public void LP_LoginButton()
    {
        string tc = ConvertFromIF(LoginIFields[0]);
        string password = ConvertFromIF(LoginIFields[1]);
        if (NotNull(tc) && NotNull(password))
        {
            object a = DBManager.dbManager.ExecuteQuery(1, DBManager.dbConnection, "SELECT * FROM hasta WHERE tc = @param1 AND şifre = @param2", tc, password);
            Tuple<bool, string, string> tuple = (Tuple<bool, string, string>)a;
            
            if (tuple.Item1)
            {
                currentUsername = string.Concat(tuple.Item2, ' ', tuple.Item3);
                currentTC = string.Copy(tc);
                LoginPanel.SetActive(false);
                OnPatientMainScreenPanelEnable();
                
            }
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
        int id = UnityEngine.Random.Range(1, 10000);
        string tc = ConvertFromIF(RegisterIFields[0]);
        string name = ConvertFromIF(RegisterIFields[1]);
        string[] str = name.Split(' ');
        string surname = "";
        StringBuilder sb = new StringBuilder();
        if(str.Length > 0 )
        {
            for (int i = 0; i < str.Length - 1; i++)
            {
                sb.Append(str[i]);
                sb.Append(' ');
            }
            name = string.Copy(sb.ToString());
            surname = str[str.Length - 1];
        }
        string birthday = ConvertFromIF(RegisterIFields[2]);
        string insurance = ConvertFromIF(RegisterIFields[3]);
        string password = ConvertFromIF(RegisterIFields[4]);
        if (NotNull(tc) && NotNull(name) && NotNull(birthday) && NotNull(insurance) && NotNull(password) && NotNull(surname))
        {
            DateTime bdate = DateTime.ParseExact(birthday, dateFormat, CultureInfo.GetCultureInfo("tr-TR"));
            DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "INSERT INTO hasta VALUES(@param1, @param2, @param3, @param4, @param5, @param6, @param7)", id, name, surname, tc, password, bdate, insurance);
            currentUsername = string.Concat(name, ' ', surname);
            currentTC = string.Copy(tc);
            OnPatientMainScreenPanelEnable();
            RegisterPanel.SetActive(false);
        }
    }
    #endregion
    #region Side Panel Buttons
    public void SidePanel_TakeRendezvousButton()
    {
        CloseAllPanels();
        PatientTakeRendezvousPanel.SetActive(true);
    }

    public void SidePanel_MyExaminationsButton()
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
    #region Settings Panel Buttons
    public void SetP_UpdatePassword()
    {
        string password = ConvertFromIF(SettingsIFields[0]);
    }
    public void SetP_SaveCreditCard()
    {
        string cardName = ConvertFromIF(SettingsIFields[1]);
        string cardNo = ConvertFromIF(SettingsIFields[2]);
        string endDate = ConvertFromIF(SettingsIFields[3]);
        string CVV = ConvertFromIF(SettingsIFields[4]);
    }
    #endregion
    #region Paying Panel Buttons
    public void PP_PayWithCash()
    {

    }
    public void PP_PayWithCard()
    {
        CreditCardPanel.SetActive(true);
        PayingPanel.SetActive(false);
    }
    #endregion
    #region Credit Card Panel Buttons
    public void CC_Pay()
    {
        string cardName = ConvertFromIF(CreditCardIFields[0]);
        string cardNo = ConvertFromIF(CreditCardIFields[1]);
        string endDate = ConvertFromIF(CreditCardIFields[2]);
        string CVV = ConvertFromIF(CreditCardIFields[3]);
    }
    #endregion
    #region OnPanelEnables
    private void OnPatientMainScreenPanelEnable()
    {
        PatientMainScreenPanelTexts[0].SetText(currentUsername);
        PatientMainScreenPanelTexts[1].SetText(DateTime.Now.Date.ToString());
        PatientMainScreenPanel.SetActive(true);
    }
    #endregion
    #region Prefab Functions
    public void RendezvousPrefab(int rendezvousType, GameObject rendezvousObj)
    {
        int N = rendezvousObj.transform.childCount;
        switch (rendezvousType)
        {
            case 0: //Randevu Göstergeci Ýptalli

                break;
            case 1: //Randevu Göstergeci Ödeli

                break;
            case 2: //Randevu Göstergeci Randevu Allý

                break;
            case 3: //Randevu Göstergeci Görüntüleli

                break;
            case 4: //Randevu Göstergeci Paralý Ödenmemiþ
                int counter = 0;
                for (int i = 0; i < N; i++)
                {
                    if (rendezvousObj.transform.GetChild(i).TryGetComponent(out TextMeshProUGUI text))
                    {
                        switch (counter)
                        {
                            case 0:
                                text.SetText("Poliklinik");
                                break;
                            case 1:
                                text.SetText("Doktor Adý");
                                break;
                            case 2:
                                text.SetText("Tarih");
                                break;
                            case 3:
                                text.SetText("Fiyat");
                                break;
                        }
                        counter++;
                    }
                }
                break;
            case 5: //Randevu Göstergeci Paralý Ödenmiþ

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
