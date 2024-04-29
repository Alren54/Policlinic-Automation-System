using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randevu
{
    public DateTime Date { get; set; }
    public Poliklinik Policlinic { get; set; }
    public string Hasta_ID { get; set; }
    public string Doktor_ID { get; set; }
    public bool isCancelled { get; set; }
    public string Randevu_ID { get; set; }
}
