using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randevu
{
    private DateTime Date { get; set; }
    private Poliklinik Policlinic { get; set; }
    private string Hasta_ID { get; set; }
    private string Doktor_ID { get; set; }
    private bool isCancelled { get; set; }
    private string Randevu_ID { get; set; }
}
