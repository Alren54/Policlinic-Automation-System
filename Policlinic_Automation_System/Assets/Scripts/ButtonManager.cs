using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Linq;
using Unity.VisualScripting;
using System.Globalization;
using UnityEngine.UI;
using System.Reflection;

public class ButtonManager : MonoBehaviour
{
    [Header("TempVariables")]
    [SerializeField] string currentUsername;
    [SerializeField] int currentID;
    [SerializeField] int currentRendezvousID;
    [SerializeField] bool isPatient = true;
    [SerializeField] List<Tuple<string, List<DateTime>, int>> availableRendezvous = new();

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
    [SerializeField] private GameObject DoctorMainScreenPanel;
    [SerializeField] private GameObject DoExaminationPanel;
    [SerializeField] private GameObject DoctorExhibitionsPanel;
    [SerializeField] private GameObject DoctorExInfoPanel;
    [SerializeField] private GameObject TcInfoPanel;
    [SerializeField] private GameObject RegisterDocPanel;
    [SerializeField] private GameObject DocSettingsPanel;

    [Header("Objects")]
    [SerializeField] private GameObject CC;
    [SerializeField] private TMP_Dropdown poliDropdown;
    [SerializeField] private TMP_Dropdown docDropdown;
    [SerializeField] private Toggle poliToggle;
    [SerializeField] private Toggle docToggle;

    [Header("Input Fields")]
    [SerializeField] private List<GameObject> LoginIFields;
    [SerializeField] private List<GameObject> RegisterIFields;
    [SerializeField] private List<GameObject> RegisterDocIFields;
    [SerializeField] private List<GameObject> SettingsIFields;
    [SerializeField] private List<GameObject> CreditCardIFields;
    [SerializeField] private List<GameObject> DoExaminationPanelIFields;
    [SerializeField] private List<GameObject> TcInfoIFields;

    [Header("Texts")]
    [SerializeField] private List<TextMeshProUGUI> LoginTexts;
    [SerializeField] private List<TextMeshProUGUI> PatientMainScreenTexts;
    [SerializeField] private List<TextMeshProUGUI> PatientTakeRendezvousTexts;
    [SerializeField] private List<TextMeshProUGUI> PatientExaminationTexts;
    [SerializeField] private List<TextMeshProUGUI> DebtsTexts;
    [SerializeField] private List<TextMeshProUGUI> ExaminationInfoTexts;
    [SerializeField] private List<TextMeshProUGUI> PayingTexts;
    [SerializeField] private List<TextMeshProUGUI> SettingsTexts;
    [SerializeField] private List<TextMeshProUGUI> DoctorMainScreenTexts;
    [SerializeField] private List<TextMeshProUGUI> DoExaminationTexts;
    [SerializeField] private List<TextMeshProUGUI> DoctorExhibitionsTexts;
    [SerializeField] private List<TextMeshProUGUI> DoctorExInfoTexts;
    [SerializeField] private List<TextMeshProUGUI> TcInfoTexts;

    [Header("ScrollViews")]
    [SerializeField] private List<Transform> PatientMainScreenSVContents;
    [SerializeField] private Transform PatientTakeRendezvousSVContent;
    [SerializeField] private Transform PatientExaminationSVContent;
    [SerializeField] private Transform DebtsSVContent;
    [SerializeField] private Transform DoctorMainScreenSVContent;
    [SerializeField] private Transform DoctorExhibitionsSVContent;
    [SerializeField] private Transform TcInfoSVContent;

    [Header("Rendezvous Prefabs")]
    [SerializeField] private List<GameObject> RendezvousPrefabs;
    private void Start()
    {
        poliToggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged();
        });
        docToggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged();
        });
        poliDropdown.onValueChanged.AddListener(delegate {
            ToggleValueChanged();
        });
        docDropdown.onValueChanged.AddListener(delegate {
            ToggleValueChanged();
        });
    }
    public void QuitFromApp()
    {
        Application.Quit();
    }
    void ToggleValueChanged()
    {
        OnPatientTakeRendezvousPanelEnable();
    }
    #region Login Panel Buttons
    public void LP_ChangeUser()
    {
        isPatient = !isPatient;
        if (isPatient) LoginTexts[0].SetText("Hasta Giriş Yap");
        else LoginTexts[0].SetText("Doktor Giriş Yap");
    }
    public void LP_LoginButton()
    {
        string tc = ConvertFromIF(LoginIFields[0]);
        string password = ConvertFromIF(LoginIFields[1]);
        if (NotNull(tc) && NotNull(password))
        {
            object a;
            if (isPatient) a = DBManager.dbManager.ExecuteQuery(1, DBManager.dbConnection, "SELECT * FROM Hasta WHERE Tc = @param1 AND Sifre = @param2", tc, password);
            else a = DBManager.dbManager.ExecuteQuery(10, DBManager.dbConnection, "SELECT * FROM Doktor WHERE Tc = @param1 AND Sifre = @param2", tc, password);
            Tuple<bool, string, string, int> tuple = (Tuple<bool, string, string, int>)a;

            if (tuple.Item1)
            {
                currentUsername = string.Concat(tuple.Item2, ' ', tuple.Item3);
                currentID = tuple.Item4;
                LoginPanel.SetActive(false);
                if (isPatient)
                {
                    OnPatientMainScreenPanelEnable();
                    PatientMainScreenPanel.SetActive(true);
                }
                else
                {
                    OnDoctorMainScreenPanelEnable();
                    DoctorMainScreenPanel.SetActive(true);
                }
            }
        }
    }

    public void LP_RegisterButton()
    {
        LoginPanel.SetActive(false);
        if (!isPatient)
        {
            RegisterDocPanel.SetActive(true);
        }
        else RegisterPanel.SetActive(true);
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
    public void RP_DocRegisterButton()
    {
        string tc = ConvertFromIF(RegisterDocIFields[0]);
        string name = ConvertFromIF(RegisterDocIFields[1]);
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
        string password = ConvertFromIF(RegisterDocIFields[2]);
        if (NotNull(tc) && NotNull(name) && NotNull(password) && NotNull(surname))
        {
            DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "INSERT INTO Doktor (Ad, Soyad, Tc, Sifre) VALUES(@param1, @param2, @param3, @param4)", name, surname, tc, password);
            int id = (int)DBManager.dbManager.ExecuteQuery(2, DBManager.dbConnection, "SELECT Doktor_id FROM Hasta WHERE @param1 = tc", tc);
            currentUsername = string.Concat(name, ' ', surname);
            currentID = id;
            print(currentID);
            OnDoctorMainScreenPanelEnable();
            RegisterPanel.SetActive(false);
        }
    }
    #endregion
    #region Side Panel Buttons
    public void SidePanel_MainScreenButton()
    {
        CloseAllPanels();
        PatientMainScreenPanel.SetActive(true);
        OnPatientTakeRendezvousPanelEnable();
    }
    public void SidePanel_TakeRendezvousButton()
    {
        CloseAllPanels();
        PatientTakeRendezvousPanel.SetActive(true);
        OnPatientTakeRendezvousPanelEnable();
    }

    public void SidePanel_MyExaminationsButton()
    {
        CloseAllPanels();
        PatientExaminationPanel.SetActive(true);
        OnPatientExaminationPanelEnable();
    }

    public void SidePanel_DebtsButton()
    {
        CloseAllPanels();
        DebtsPanel.SetActive(true);
        OnDebtsPanelEnable();
    }

    public void SidePanel_SettingsButton()
    {
        CloseAllPanels();
        SettingsPanel.SetActive(true);
        OnSettingsPanelEnable();
    }

    public void SidePanel_ExitButton()
    {
        CloseAllPanels();
        LoginPanel.SetActive(true);
    }
    public void SidePanel_DoctorMainScreen()
    {
        CloseAllPanels();
        DoctorMainScreenPanel.SetActive(true);
        OnDoctorMainScreenPanelEnable();
    }
    public void SidePanel_DoctorExhibitions()
    {
        CloseAllPanels();
        DoctorExhibitionsPanel.SetActive(true);
        OnDoctorExhibitionsPanelEnable();
    }
    public void SidePanel_DoctorSearchWithID()
    {
        CloseAllPanels();
        TcInfoPanel.SetActive(true);
        OnTcInfoPanelEnable();
    }
    public void SidePanel_DocSettingsPanel()
    {
        CloseAllPanels();
        DocSettingsPanel.SetActive(true);
    }
    #endregion
    #region Settings Panel Buttons
    public void SetP_UpdatePassword()
    {
        string password = ConvertFromIF(SettingsIFields[0]);
        string password2 = ConvertFromIF(SettingsIFields[1]);
        if (string.Equals(password, password2))
        {
            if(isPatient)
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "UPDATE Hasta SET Sifre = @param1 WHERE Hasta_id = @param2", password, currentID);
            else
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "UPDATE Doktor SET Sifre = @param1 WHERE Doktor_id = @param2", password, currentID);

            print("is set");
        }
        else print("is not set");

    }
    public void SetP_SaveCreditCard()
    {
        string cardName = ConvertFromIF(SettingsIFields[2]);
        string cardNo = ConvertFromIF(SettingsIFields[3]);
        string endDate = ConvertFromIF(SettingsIFields[4]);
        string CVV = ConvertFromIF(SettingsIFields[5]);

        DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "UPDATE KrediKarti SET karttaki_isim = @param1 , kart_no = @param2, kart_son_kullanma_tarihi = @param3, cvv = @param4 WHERE Hasta_id = @param5 ", cardName, cardNo, DateTime.Parse(endDate), int.Parse(CVV), currentID);
    }
    #endregion
    #region Paying Panel Buttons
    public void PP_PayWithCash()
    {
        PayingPanel.SetActive(false);
        DebtsPanel.SetActive(true);
        DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "UPDATE Odeme SET isOdendi = true WHERE Odeme_id = @param1", currentRendezvousID);
        OnDebtsPanelEnable();
    }
    public void PP_PayWithCard()
    {
        PayingPanel.SetActive(false);
        CreditCardPanel.SetActive(true);
        OnCreditCardPanelEnable();
    }
    #endregion
    #region Do Examination Button
    public void DoExamination()
    {
        string Teşhis = ConvertFromIF(DoExaminationPanelIFields[0]);
        string Reçete = ConvertFromIF(DoExaminationPanelIFields[1]);
        string Rapor = ConvertFromIF(DoExaminationPanelIFields[2]);
        string Sevk = ConvertFromIF(DoExaminationPanelIFields[3]);
        string Ücret = ConvertFromIF(DoExaminationPanelIFields[4]);
        if (NotNull(Teşhis) && NotNull(Reçete) && NotNull(Rapor) && NotNull(Sevk) && NotNull(Ücret))
        {
            DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "UPDATE Muayene SET Teşhis = @param1, Reçete = @param2, Rapor = @param3, Sevk = @param4, isMuayeneEdildi = @param5 WHERE Muayene_id = @param6", Teşhis, Reçete, Rapor, Sevk, true, currentRendezvousID);
            DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "UPDATE Odeme SET Ucret = @param1 WHERE Odeme_id = @param2", float.Parse(Ücret), currentRendezvousID);
            DoctorMainScreenPanel.SetActive(true);
            OnDoctorMainScreenPanelEnable();
            DoExaminationPanel.SetActive(false);
        }
        else print("olmadı");
    }
    #endregion
    #region TCInfo Button
    public void TCInfoButton()
    {
        if (TcInfoSVContent.childCount != 0)
        {
            int N = TcInfoSVContent.childCount - 1;
            for (int i = N; i >= 0; i--)
            {
                Destroy(TcInfoSVContent.GetChild(i).gameObject);
            }
        }
        string tc = ConvertFromIF(TcInfoIFields[0]);
        int totalCount = 0;
        GameObject obj;
        List<Tuple<string, string, DateTime, int, float?>> tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT d.Ad, h.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h INNER JOIN Muayene_randevu m ON m.Hasta_id = h.Hasta_id INNER JOIN Doktor d ON m.Doktor_id = d.Doktor_id INNER JOIN Muayene mu ON m.Muayene_id = mu.Muayene_id WHERE h.Tc = @param1 AND mu.isMuayeneEdildi = @param2 ", tc, true);
        Muayene_randevu_ids.Clear();
        Muayene_randevu_ids.Add(new List<int>());

        

        int count = tempTuple.Count;
        print(count);
        if (count > 0)
        {
            TcInfoTexts[1].SetText(tempTuple[0].Item2);
            TcInfoTexts[2].SetText(tc);
        }
        for (int i = 0; i < count; i++)
        {
            obj = Instantiate(RendezvousPrefabs[8]);
            obj.transform.SetParent(TcInfoSVContent, true);
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
                            text.SetText(tempTuple[i].Item2);
                            break;
                        case 1:
                            text.SetText(tempTuple[i].Item3.ToString());
                            break;
                    }
                    counter++;
                }
                else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = totalCount;
                }
            }
            totalCount++;
        }
    }
    #endregion
    #region Credit Card Panel Buttons
    public void CC_Pay()
    {
        string cardName = ConvertFromIF(CreditCardIFields[0]);
        string cardNo = ConvertFromIF(CreditCardIFields[1]);
        string endDate = ConvertFromIF(CreditCardIFields[2]);
        string CVV = ConvertFromIF(CreditCardIFields[3]);
        DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "UPDATE Odeme SET isOdendi = true WHERE Odeme_id = @param1", currentRendezvousID);
        CreditCardPanel.SetActive(false);
        ExaminationInfoPanel.SetActive(true);
        OnExaminationInfoPanelEnable(currentRendezvousID);
    }
    #endregion
    #region OnPanelEnables
    private void OnPatientMainScreenPanelEnable()
    {
        for (int i = 0; i < PatientMainScreenSVContents.Count; i++)
        {
            if (PatientMainScreenSVContents[i].childCount != 0)
            {
                int N = PatientMainScreenSVContents[i].childCount - 1;
                for (int j = N; j >= 0; j--)
                {
                    Destroy(PatientMainScreenSVContents[i].GetChild(j).gameObject);
                }
            }
        }
        PatientMainScreenTexts[0].SetText(currentUsername);
        PatientMainScreenTexts[1].SetText(DateTime.Now.ToString());

        List<string> poliklinik = (List<string>)DBManager.dbManager.ExecuteQuery(12, DBManager.dbConnection, "SELECT p.İsim FROM Polikinlik p");
        poliDropdown.ClearOptions();
        poliDropdown.AddOptions(poliklinik);

        List<string> doc = (List<string>)DBManager.dbManager.ExecuteQuery(12, DBManager.dbConnection, "SELECT d.Ad FROM Doktor d");
        docDropdown.ClearOptions();
        docDropdown.AddOptions(doc);

        GameObject obj;
        List<Tuple<string, string, DateTime, int, float?>> tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu WHERE m.Muayene_id = mu.Muayene_id AND mu.isMuayeneEdildi = @param1 AND m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND  h.Hasta_id = @param2 AND m.Randevu_tarih > @param3", false, currentID, DateTime.Now);
        Muayene_randevu_ids.Clear();
        Muayene_randevu_ids.Add(new List<int>());

        int count = tempTuple.Count;
        for (int i = 0; i < count; i++)
        {
            obj = Instantiate(RendezvousPrefabs[0]);
            int a = (tempTuple[i].Item3.DayOfYear + tempTuple[i].Item3.Year * 365) - (DateTime.Now.DayOfYear + DateTime.Now.Year * 365);
            if (a < 7)
            {
                obj.transform.SetParent(PatientMainScreenSVContents[0], true);
            }
            else obj.transform.SetParent(PatientMainScreenSVContents[2], true);
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
                else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = tempTuple[i].Item4;
                }
            }
        }
        PatientMainScreenTexts[2].SetText($"Bu hafta {count} randevunuz bulunuyor.");

        tempTuple.Clear();
        tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h, Polikinlik p, Doktor d, Muayene_randevu m, Odeme o, Muayene mu WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND h.Hasta_id = @param1 AND m.Muayene_id = mu.Muayene_id AND mu.Odeme_id = o.Odeme_id AND o.isOdendi = false AND mu.isMuayeneEdildi = true", currentID);
        Muayene_randevu_ids.Add(new List<int>());
        int totalCount = 0;
        count = tempTuple.Count;
        for (int i = 0; i < count; i++)
        {
            obj = Instantiate(RendezvousPrefabs[1]);
            obj.transform.SetParent(PatientMainScreenSVContents[1], true);
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
                    rbc.buttonID = totalCount++;
                }

            }
        }
        PatientMainScreenTexts[3].SetText($"Ödenmemiş {count} muayeneniz var.");
        PatientMainScreenPanel.SetActive(true);
    }
    private void OnPatientTakeRendezvousPanelEnable()
    {
        if (PatientTakeRendezvousSVContent.childCount != 0)
        {
            int N = PatientTakeRendezvousSVContent.childCount - 1;
            for (int i = N; i >= 0; i--)
            {
                Destroy(PatientTakeRendezvousSVContent.GetChild(i).gameObject);
            }
        }
        PatientTakeRendezvousTexts[0].SetText(currentUsername);
        GameObject obj;



        availableRendezvous.Clear();

        int poliValue = poliDropdown.value;
        string poliStr = poliDropdown.options[poliValue].text;


        int docValue = docDropdown.value;
        string docStr = docDropdown.options[docValue].text;

        if (!poliToggle.isOn && !docToggle.isOn) availableRendezvous = (List<Tuple<string, List<DateTime>, int>>)DBManager.dbManager.ExecuteQuery(6, DBManager.dbConnection, "SELECT d.Ad, m.Randevu_tarih, d.Doktor_id FROM Polikinlik p, Doktor d, Muayene_randevu m WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id ORDER BY d.Doktor_id");
        else if(poliToggle.isOn && !docToggle.isOn) availableRendezvous = (List<Tuple<string, List<DateTime>, int>>)DBManager.dbManager.ExecuteQuery(6, DBManager.dbConnection, "SELECT d.Ad, m.Randevu_tarih, d.Doktor_id FROM Polikinlik p, Doktor d, Muayene_randevu m WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND p.İsim = @param1 ORDER BY d.Doktor_id", poliStr);
        else if(!poliToggle.isOn && docToggle.isOn) availableRendezvous = (List<Tuple<string, List<DateTime>, int>>)DBManager.dbManager.ExecuteQuery(6, DBManager.dbConnection, "SELECT d.Ad, m.Randevu_tarih, d.Doktor_id FROM Polikinlik p, Doktor d, Muayene_randevu m WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND d.ad = @param1 ORDER BY d.Doktor_id", docStr);
        else availableRendezvous = (List<Tuple<string, List<DateTime>, int>>)DBManager.dbManager.ExecuteQuery(6, DBManager.dbConnection, "SELECT d.Ad, m.Randevu_tarih, d.Doktor_id FROM Polikinlik p, Doktor d, Muayene_randevu m WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND d.ad = @param1 AND p.İsim = @param2 ORDER BY d.Doktor_id", docStr, poliStr);
        
        Muayene_randevu_ids.Clear();
        Muayene_randevu_ids.Add(new List<int>());
        int count = availableRendezvous.Count;
        int total = 0;
        for (int i = 0; i < count; i++)
        {
            foreach (var date in availableRendezvous[i].Item2)
            {
                obj = Instantiate(RendezvousPrefabs[2]);
                obj.transform.SetParent(PatientTakeRendezvousSVContent, true);
                int N = obj.transform.childCount;
                int counter = 0;
                for (int j = 0; j < N; j++)
                {
                    if (obj.transform.GetChild(j).TryGetComponent(out TextMeshProUGUI text))
                    {
                        switch (counter)
                        {
                            case 0:
                                text.SetText(date.ToString());
                                break;
                            case 1:
                                text.SetText(availableRendezvous[i].Item1);
                                break;
                        }
                        counter++;
                    }
                    else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                    {
                        rbc.buttonManager = this;
                        rbc.buttonID = total;
                    }
                }
                total++;
            }
        }
    }
    private void OnPatientExaminationPanelEnable()
    {
        if (PatientExaminationSVContent.childCount != 0)
        {
            int N = PatientExaminationSVContent.childCount - 1;
            for (int i = N; i >= 0; i--)
            {
                Destroy(PatientExaminationSVContent.GetChild(i).gameObject);
            }
        }
        PatientExaminationTexts[0].SetText(currentUsername);
        int buttonCounter = 0;
        GameObject obj;
        List<Tuple<string, string, DateTime, int, float?>> tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu, Odeme o WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND h.Hasta_id = @param1 AND m.Muayene_id = mu.Muayene_id AND mu.isMuayeneEdildi = @param2 AND mu.Odeme_id = o.Odeme_id AND o.isOdendi = @param3 ", currentID, true, true);
        Muayene_randevu_ids.Clear();
        Muayene_randevu_ids.Add(new List<int>());
        int count = tempTuple.Count;
        for (int i = 0; i < count; i++)
        {
            obj = Instantiate(RendezvousPrefabs[3]);
            obj.transform.SetParent(PatientExaminationSVContent, true);
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
                else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = buttonCounter;
                    buttonCounter++;
                }
            }
        }
    }
    private void OnExaminationInfoPanelEnable(int rendezvousID)
    {
        currentRendezvousID = rendezvousID;
        ExaminationInfoTexts[0].SetText(currentUsername);

        List<string> tempTuple = (List<string>)DBManager.dbManager.ExecuteQuery(8, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id, mu.Teşhis, mu.Reçete, mu.Rapor, mu.Sevk, o.Ucret FROM Odeme o, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu WHERE m.Muayene_randevu_id = @param1 AND m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Muayene_id = mu.Muayene_id AND mu.Odeme_id = o.Odeme_id", rendezvousID);
        for (int i = 0; i < tempTuple.Count; i++)
        {
            print(tempTuple[i]);
            ExaminationInfoTexts[i + 1].SetText(tempTuple[i]);
        }
    }
    private void OnPayingPanelEnable(int rendezvousID)
    {
        currentRendezvousID = rendezvousID;
        PayingTexts[0].SetText(currentUsername);

        List<string> tempTuple = (List<string>)DBManager.dbManager.ExecuteQuery(8, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id, mu.Teşhis, mu.Reçete, mu.Rapor, mu.Sevk, o.Ucret FROM Odeme o, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu WHERE m.Muayene_randevu_id = @param1 AND m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Muayene_id = mu.Muayene_id AND mu.Odeme_id = o.Odeme_id", rendezvousID);
        for (int i = 0; i < tempTuple.Count; i++)
        {
            PayingTexts[i + 1].SetText(tempTuple[i]);
        }
    }
    private void OnDoExaminationPanelEnable(int rendezvousID)
    {
        currentRendezvousID = rendezvousID;
        DoExaminationTexts[0].SetText(currentUsername);

        List<string> tempTuple = (List<string>)DBManager.dbManager.ExecuteQuery(11, DBManager.dbConnection, "SELECT h.Ad, h.Tc, m.Randevu_tarih, m.Muayene_randevu_id FROM Muayene_randevu m, Hasta h WHERE m.Muayene_randevu_id = @param1 AND h.Hasta_id = m.Hasta_id ", rendezvousID);
        for (int i = 0; i < tempTuple.Count; i++)
        {
            DoExaminationTexts[i + 1].SetText(tempTuple[i]);
        }
    }
    private void OnDebtsPanelEnable()
    {
        if (DebtsSVContent.childCount != 0)
        {
            int N = DebtsSVContent.childCount - 1;
            for (int i = N; i >= 0; i--)
            {
                Destroy(DebtsSVContent.GetChild(i).gameObject);
            }
        }

        DebtsTexts[0].SetText(currentUsername);
        int buttonCounter = 0;
        GameObject obj;
        List<Tuple<string, string, DateTime, int, float?, bool>> tempTuple = (List<Tuple<string, string, DateTime, int, float?, bool>>)DBManager.dbManager.ExecuteQuery(7, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id, o.Ucret, o.isOdendi FROM Odeme o, Hasta h, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND h.Hasta_id = @param1 AND m.Muayene_id = mu.Muayene_id AND mu.Odeme_id = o.Odeme_id AND mu.isMuayeneEdildi = @param2", currentID, true);
        Muayene_randevu_ids.Clear();
        Muayene_randevu_ids.Add(new List<int>());
        int count = tempTuple.Count;
        print(count);
        for (int i = 0; i < count; i++)
        {
            if (!tempTuple[i].Item6) obj = Instantiate(RendezvousPrefabs[4]);
            else obj = Instantiate(RendezvousPrefabs[5]);
            obj.transform.SetParent(DebtsSVContent, true);
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
                else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = buttonCounter;
                    buttonCounter++;
                }
            }
        }
    }
    private void OnCreditCardPanelEnable()
    {
        List<string> tempTuple = (List<string>)DBManager.dbManager.ExecuteQuery(9, DBManager.dbConnection, "SELECT k.karttaki_isim, k.kart_no, k.kart_son_kullanma_tarihi, k.cvv FROM KrediKarti k, Hasta h WHERE h.Hasta_id = @param1", currentID);

        CreditCardIFields[0].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[0]);
        CreditCardIFields[1].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[1]);
        CreditCardIFields[2].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[2]);
        CreditCardIFields[3].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[3]);
    }
    private void OnSettingsPanelEnable()
    {
        SettingsTexts[0].SetText(currentUsername);
        if (isPatient)
        {
            List<string> tempTuple = (List<string>)DBManager.dbManager.ExecuteQuery(9, DBManager.dbConnection, "SELECT k.karttaki_isim, k.kart_no, k.kart_son_kullanma_tarihi, k.cvv FROM KrediKarti k, Hasta h WHERE h.Hasta_id = @param1 AND k.Hasta_id = h.Hasta_id", currentID);
            CC.SetActive(true);
            SettingsIFields[2].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[0]);
            SettingsIFields[3].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[1]);
            SettingsIFields[4].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[2]);
            SettingsIFields[5].GetComponent<TMP_InputField>().SetTextWithoutNotify(tempTuple[3]);
        }
        else
        {
            CC.SetActive(false);
        }

    }
    private void OnDoctorMainScreenPanelEnable()
    {
        if (DoctorMainScreenSVContent.childCount != 0)
        {
            int N = DoctorMainScreenSVContent.childCount - 1;
            for (int i = N; i >= 0; i--)
            {
                Destroy(DoctorMainScreenSVContent.GetChild(i).gameObject);
            }
        }
        DoctorMainScreenTexts[0].SetText(currentUsername);
        DoctorMainScreenTexts[1].SetText(DateTime.Now.ToString());
        int totalCount = 0;
        GameObject obj;
        List<Tuple<string, string, DateTime, int, float?>> tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT mu.Teşhis, h.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu WHERE m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Hasta_id = h.Hasta_id AND  d.Doktor_id = @param1 AND m.Randevu_tarih > @param2 AND m.Muayene_id = mu.Muayene_id ", currentID, DateTime.Today.AddDays(-1));
        Muayene_randevu_ids.Clear();
        Muayene_randevu_ids.Add(new List<int>());
        int count = tempTuple.Count;
        print(count);
        for (int i = 0; i < count; i++)
        {
            if (string.Compare(tempTuple[i].Item1, "Yok") == 0)
            {
                obj = Instantiate(RendezvousPrefabs[7]);
                print(tempTuple[i].Item1);
            }
            else
            {
                obj = Instantiate(RendezvousPrefabs[6]);
                print(tempTuple[i].Item1);
            }
            obj.transform.SetParent(DoctorMainScreenSVContent, true);
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
                            text.SetText(tempTuple[i].Item2);
                            break;
                        case 1:
                            text.SetText(tempTuple[i].Item3.ToString());
                            break;
                    }
                    counter++;
                }
                else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = totalCount;
                }
            }
            totalCount++;
        }
        DoctorMainScreenTexts[2].SetText($"Bugün {count} randevunuz bulunuyor.");
    }
    private void OnDoctorExhibitionsPanelEnable()
    {
        if (DoctorExhibitionsSVContent.childCount != 0)
        {
            int N = DoctorExhibitionsSVContent.childCount - 1;
            for (int i = N; i >= 0; i--)
            {
                Destroy(DoctorExhibitionsSVContent.GetChild(i).gameObject);
            }
        }
        DoctorExhibitionsTexts[0].SetText(currentUsername);
        int totalCount = 0;
        GameObject obj;
        List<Tuple<string, string, DateTime, int, float?>> tempTuple = (List<Tuple<string, string, DateTime, int, float?>>)DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection, "SELECT mu.Teşhis, h.Ad, m.Randevu_tarih, m.Muayene_randevu_id FROM Hasta h INNER JOIN Muayene_randevu m ON m.Hasta_id = h.Hasta_id INNER JOIN Doktor d ON m.Doktor_id = d.Doktor_id INNER JOIN Muayene mu ON m.Muayene_id = mu.Muayene_id WHERE d.Doktor_id = @param1 AND mu.isMuayeneEdildi = @param2 ", currentID, true);
        Muayene_randevu_ids.Clear();
        Muayene_randevu_ids.Add(new List<int>());
        int count = tempTuple.Count;
        print(count);
        for (int i = 0; i < count; i++)
        {
            obj = Instantiate(RendezvousPrefabs[8]);
            obj.transform.SetParent(DoctorExhibitionsSVContent, true);
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
                            text.SetText(tempTuple[i].Item2);
                            break;
                        case 1:
                            text.SetText(tempTuple[i].Item3.ToString());
                            break;
                    }
                    counter++;
                }
                else if (obj.transform.GetChild(j).TryGetComponent(out RendezvousButtonControl rbc))
                {
                    rbc.buttonManager = this;
                    rbc.buttonID = totalCount;
                }
            }
            totalCount++;
        }
    }
    private void OnDoctorExInfoPanelEnable(int rendezvousID)
    {
        currentRendezvousID = rendezvousID;
        DoctorExInfoTexts[0].SetText(currentUsername);

        List<string> tempTuple = (List<string>)DBManager.dbManager.ExecuteQuery(8, DBManager.dbConnection, "SELECT p.İsim, d.Ad, m.Randevu_tarih, m.Muayene_randevu_id, mu.Teşhis, mu.Reçete, mu.Rapor, mu.Sevk, o.Ucret FROM Odeme o, Polikinlik p, Doktor d, Muayene_randevu m, Muayene mu WHERE m.Muayene_randevu_id = @param1 AND m.Doktor_id = d.Doktor_id AND d.Polikinlik_id = p.Polikinlik_id AND m.Muayene_id = mu.Muayene_id AND mu.Odeme_id = o.Odeme_id", rendezvousID);
        for (int i = 0; i < tempTuple.Count; i++)
        {
            print(tempTuple[i]);
            DoctorExInfoTexts[i + 1].SetText(tempTuple[i]);
        }
    }
    private void OnTcInfoPanelEnable()
    {
        if (TcInfoSVContent.childCount != 0)
        {
            int N = TcInfoSVContent.childCount - 1;
            for (int i = N; i >= 0; i--)
            {
                Destroy(TcInfoSVContent.GetChild(i).gameObject);
            }
        }
        TcInfoTexts[0].SetText(currentUsername);
    }
    #endregion
    #region Prefab Functions
    public void RendezvousPrefab(int rendezvousType, int rendezvousID, GameObject rendezvousObject)
    {
        int pressedButton;
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
                pressedButton = Muayene_randevu_ids[1][rendezvousID];
                OnPayingPanelEnable(pressedButton);
                print("Ödemeye girildi");
                return;
            case 2:
                int dateTimeLocation = 0;
                int totalProgressionLow = 0;
                int totalProgressionHigh = 0;
                int i = 0;
                for (i = 0; i < availableRendezvous.Count; i++)
                {
                    totalProgressionHigh += availableRendezvous[i].Item2.Count;
                    if (rendezvousID < totalProgressionHigh)
                    {
                        dateTimeLocation = rendezvousID - totalProgressionLow; // 5, 7, 8, 4; 14 || 
                        break;
                    }
                    totalProgressionLow += availableRendezvous[i].Item2.Count;
                }
                print("ekleniyor");
                print(availableRendezvous[i].Item2[dateTimeLocation]);
                int muayeneCount = (int)DBManager.dbManager.ExecuteQuery(2, DBManager.dbConnection, "SELECT COUNT(*) FROM Muayene");
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "INSERT INTO Odeme (Ucret, isOdendi) VALUES (-1, @param1) ", false);
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "INSERT INTO Muayene (Teşhis, Reçete, Rapor, Sevk, isMuayeneEdildi, Odeme_id) VALUES (@param1, NULL, NULL, NULL,@param2, @param3) ", "Yok", false, muayeneCount + 1);
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "INSERT INTO Muayene_randevu (Hasta_id, Doktor_id, Randevu_tarih, Muayene_id) VALUES(@param1, @param2, @param3, @param4) ", currentID, availableRendezvous[i].Item3, availableRendezvous[i].Item2[dateTimeLocation], muayeneCount + 1);
                Destroy(rendezvousObject);
                break;
            case 3:
                print(rendezvousID);
                pressedButton = Muayene_randevu_ids[0][rendezvousID];
                OnExaminationInfoPanelEnable(pressedButton);
                ExaminationInfoPanel.SetActive(true);
                PatientExaminationPanel.SetActive(false);
                break;
            case 4:
                DebtsPanel.SetActive(false);
                PayingPanel.SetActive(true);
                pressedButton = Muayene_randevu_ids[0][rendezvousID];
                print(rendezvousID);
                foreach(var a in Muayene_randevu_ids[0])
                {
                    print(a);
                }
                OnPayingPanelEnable(pressedButton);
                print("Ödemeye girildi 2");
                break;
            case 5:
                DebtsPanel.SetActive(false);
                ExaminationInfoPanel.SetActive(true);
                pressedButton = Muayene_randevu_ids[0][rendezvousID];
                OnExaminationInfoPanelEnable(pressedButton);
                break;
            case 6:
                pressedButton = Muayene_randevu_ids[0][rendezvousID];
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "DELETE FROM Muayene_randevu WHERE Muayene_randevu_id = @param1 ", pressedButton);
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "DELETE FROM Muayene WHERE Muayene_id = @param1 ", pressedButton);
                DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "DELETE FROM Odeme WHERE Odeme_id = @param1 ", pressedButton);
                Destroy(rendezvousObject);
                break;
            case 7:
                DoctorMainScreenPanel.SetActive(false);
                DoExaminationPanel.SetActive(true);
                pressedButton = Muayene_randevu_ids[0][rendezvousID];
                OnDoExaminationPanelEnable(pressedButton);
                print("Ödemeye girildi");
                break;
            case 8:
                print(rendezvousID);
                pressedButton = Muayene_randevu_ids[0][rendezvousID];
                OnDoctorExInfoPanelEnable(pressedButton);
                DoctorExInfoPanel.SetActive(true);
                DoctorExhibitionsPanel.SetActive(false);
                TcInfoPanel.SetActive(false);
                break;
            default: break;
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
        DoctorMainScreenPanel.SetActive(false);
        DoExaminationPanel.SetActive(false);
        DoctorExhibitionsPanel.SetActive(false);
        DoctorExInfoPanel.SetActive(false);
        TcInfoPanel.SetActive(false);
        RegisterDocPanel.SetActive(false);
    }

    private string ConvertFromIF(GameObject Ifield)
    {
        return Ifield.GetComponent<TMP_InputField>().text.ToString();
    }
    #endregion
}