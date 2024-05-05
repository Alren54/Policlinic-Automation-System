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
    [SerializeField] int currentID;

    [Header("SQL Attributes")]
    [SerializeField] DBManager DBManager;
    [SerializeField] string dateFormat = "yyyy-MM-dd";
    [SerializeField] List<List<int>> Muayene_randevu_ids = new();

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
    [SerializeField] private List<TextMeshProUGUI> PatientMainScreenTexts;

    [Header("ScrollViews")]
    [SerializeField] private List<Transform> PatientMainScreenSVContent;

    [Header("Rendezvous Prefabs")]
    [SerializeField] private List<GameObject> RendezvousPrefabs;
    // object result = DBManager.dbManager.ExecuteQuery(4, DBManager.dbConnection, "SELECT * FROM user_info WHERE user_name = @param1", Username);

    #region Login Panel Buttons
    public void LP_LoginButton()
    {
        string tc = ConvertFromIF(LoginIFields[0]);
        string password = ConvertFromIF(LoginIFields[1]);
        if (NotNull(tc) && NotNull(password))
        {
            object a = DBManager.dbManager.ExecuteQuery(1, DBManager.dbConnection, "SELECT * FROM Hasta WHERE Tc = @param1 AND Sifre = @param2", tc, password);
            Tuple<bool, string, string, int> tuple = (Tuple<bool, string, string, int>)a;

            if (tuple.Item1)
            {
                currentUsername = string.Concat(tuple.Item2, ' ', tuple.Item3);
                currentID = tuple.Item4;
                print(currentID);
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
        string tc = ConvertFromIF(RegisterIFields[0]);
        string name = ConvertFromIF(RegisterIFields[1]);
        string[] str = name.Split(' ');
        string surname = "";
        StringBuilder sb = new StringBuilder();
        if (str.Length > 0)
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
            int ins = int.Parse(insurance);
            DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "INSERT INTO Hasta (Ad, Soyad, Tc, Sifre, Dogum_tarihi, Sigorta_id) VALUES(@param1, @param2, @param3, @param4, @param5, @param6)", name, surname, tc, password, bdate, ins);
            int id = (int)DBManager.dbManager.ExecuteQuery(2, DBManager.dbConnection, "SELECT Hasta_ID FROM Hasta WHERE @param1 = tc", tc);
            currentUsername = string.Concat(name, ' ', surname);
            currentID = id;
            print(currentID);
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
        PatientMainScreenTexts[0].SetText(currentUsername);
        PatientMainScreenTexts[1].SetText(DateTime.Now.ToString());

        GameObject obj;
        List<Tuple<string, string, DateTime, int, float?>> tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h, Polikinlik p, Doktor d, Muayene_randevu m WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND h.Hasta_id = @param1", currentID);
        Muayene_randevu_ids.Add(new List<int>());
        int count = tempTuple.Count;
        for (int i = 0; i < count; i++)
        {
            obj = Instantiate(RendezvousPrefabs[0]);
            int a = (tempTuple[i].Item3.DayOfYear + tempTuple[i].Item3.Year * 365) - (DateTime.Now.DayOfYear + DateTime.Now.Year * 365);
            if(a < 7)
            {
                obj.transform.SetParent(PatientMainScreenSVContent[0], true);
            }
            else obj.transform.SetParent(PatientMainScreenSVContent[2], true);
            Muayene_randevu_ids[0].Add(tempTuple[i].Item4);
            int N = obj.transform.childCount;
            int counter = 0;
            for (int j = 0; j < N; j++)
            {
                if (obj.transform.GetChild(j).TryGetComponent(out TextMeshProUGUI text))
                {
                    switch (counter)
                    {
                        case 0:
                            text.SetText(tempTuple[i].Item1);
                            break;
                        case 1:
                            text.SetText(tempTuple[i].Item2);
                            break;
                        case 2:
                            text.SetText(tempTuple[i].Item3.ToString());
                            break;
                        case 3:
                            text.SetText(tempTuple[i].Item5.ToString());
                            break;
                    }
                    counter++;
                }
                else if(obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = tempTuple[i].Item4;
                }
            }
        }
        PatientMainScreenTexts[2].SetText($"Bu hafta {count} randevunuz bulunuyor.");

        tempTuple.Clear();
        tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h, Polikinlik p, Doktor d, Muayene_randevu m, Odeme o, Muayene mu WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND h.Hasta_id = @param1 AND m.Muayene_id = mu.Muayene_id AND mu.Odeme_id = o.Odeme_id AND o.isOdendi = false", currentID);
        Muayene_randevu_ids.Add(new List<int>());
        count = tempTuple.Count;
        for (int i = 0; i < count; i++)
        {
            obj = Instantiate(RendezvousPrefabs[1]);
            obj.transform.SetParent(PatientMainScreenSVContent[1], true);
            Muayene_randevu_ids[1].Add(tempTuple[i].Item4);
            int N = obj.transform.childCount;
            int counter = 0;
            for (int j = 0; j < N; j++)
            {
                if (obj.transform.GetChild(j).TryGetComponent(out TextMeshProUGUI text))
                {
                    switch (counter)
                    {
                        case 0:
                            text.SetText(tempTuple[i].Item1);
                            break;
                        case 1:
                            text.SetText(tempTuple[i].Item2);
                            break;
                        case 2:
                            text.SetText(tempTuple[i].Item3.ToString());
                            break;
                        case 3:
                            text.SetText(tempTuple[i].Item5.ToString());
                            break;
                    }
                    counter++;
                }
                else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = tempTuple[i].Item4;
                }

            }
        }


        PatientMainScreenPanel.SetActive(true);
    }
    #endregion
    #region Prefab Functions
    public void RendezvousPrefab(int rendezvousType, int rendezvousID, GameObject rendezvousObject)
    {
        switch (rendezvousType)
        {
            case 0:
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "DELETE FROM Muayene_randevu WHERE Muayene_id = @param1 ", rendezvousID);
                Destroy(rendezvousObject);
                print("İptal Edildi");
                return;
            case 1:
                PatientMainScreenPanel.SetActive(false);
                PayingPanel.SetActive(true);
                print("Ödemeye girildi");
                return;
        }



        //case 4: //Randevu Göstergeci Paralý Ödenmemiþ
        //temp = DBManager.dbManager.ExecuteQuery(4, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, mu.Ucret FROM Hasta h, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND h.Hasta_id = @param1 AND mu.Muayene_id = m.Muayene_id;", currentID);
        //tempTuple = (Tuple<string, string, DateTime, float?>)temp;
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