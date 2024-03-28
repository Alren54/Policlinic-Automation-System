using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hasta
{
    private string ID { get; set; }
    private string Name { get; set; }
    private string Surname { get; set; }
    private DateTime BirthDate { get; set; }
    private bool isInsured { get; set; }
    private List<Randevu> RandevuList { get; set; }
}
